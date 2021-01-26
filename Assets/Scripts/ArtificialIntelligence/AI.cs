using UnityEngine;

public class AI : MonoBehaviour 
{
	[SerializeField] private LayerMask targetLayerMask;
	[SerializeField] private float detectionFOV = 60.0f;
	[SerializeField] private float detectionDiameter = 12.5f;
	[SerializeField] private float rotationAngle = 45.0f;
	[SerializeField] private float rotationDuration = 2.0f;
	[SerializeField] private AIWeaponController weaponController = null;

	private AIMovementBase movementBase = null;
	private Transform target = null;
	private SphereCollider detectionCollider = null;
	private bool wasSpotted = false;
	private bool isTargetSpotted = false;
	private Quaternion fromRotation;
	private Quaternion toRotation;
	private float rotationTime = 0.0f;
	
	private void Start()
	{
		this.movementBase = this.GetComponent<AIMovementBase>();
		this.detectionCollider = this.GetComponent<SphereCollider>();
		this.detectionCollider.radius = this.detectionDiameter / 2.0f;

		this.fromRotation = this.transform.rotation *
			Quaternion.AngleAxis(-(this.rotationAngle / 2.0f), Vector3.up);
		this.toRotation = this.transform.rotation *
			Quaternion.AngleAxis(this.rotationAngle / 2.0f, Vector3.up);
		
		// Divide by 2 as will be directly inbetween these two rotation points.
		this.rotationTime = this.rotationDuration / 2.0f;
	}

	private void Update() 
	{
		this.wasSpotted = isTargetSpotted;
		this.isTargetSpotted = (this.target != null && this.IsTargetSeen(this.target));

		if(!this.isTargetSpotted)
		{
			// If target no longer spotted then continue looking from current position.
			if(this.wasSpotted)
			{
				this.rotationTime = 0.0f;
			}

			// Rotate slowly back and forth, looking for the player.
			this.UpdateIdleLookRotation(Time.deltaTime);
		}

		if(this.movementBase != null)
			this.movementBase.SetTarget((this.isTargetSpotted) ? this.target : null);

		if(this.weaponController != null)
			this.weaponController.SetTarget((this.isTargetSpotted) ? this.target : null);

		if(isTargetSpotted)
		{
			// Look towards the target (whilst preventing the body rotating upwards and downwards) 
			Vector3 targetPos = target.transform.position;
			targetPos.y = this.transform.position.y;
			this.transform.LookAt(targetPos, transform.up);
		}
	}

	private void UpdateIdleLookRotation(float dt)
	{
		this.rotationTime += Time.deltaTime;

		this.transform.rotation = Quaternion.Lerp(this.fromRotation,
			this.toRotation, this.rotationTime / this.rotationDuration);

		if(this.rotationTime >= this.rotationDuration)
		{
			Quaternion tempToRotation = this.toRotation;
			this.toRotation = this.fromRotation;
			this.fromRotation = tempToRotation;
			this.rotationTime = 0.0f;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if((1<<other.gameObject.layer) == this.targetLayerMask.value)
			this.target = other.transform;
	}

	private void OnTriggerExit(Collider other)
	{
		if((1<<other.gameObject.layer) == this.targetLayerMask.value)
			this.target = null;
	}

	private bool IsTargetSeen(Transform target)
    {
		float distance = Vector3.Distance(this.transform.position, target.position);
		bool isCloseProximity = distance <= 2.0f;
		bool isTargetWithinFOV = this.IsWithinFOV(target.position);
		if(!isTargetWithinFOV && !isCloseProximity)
			return false;

		RaycastHit hit;
		bool rayIntersectsWithTarget = Physics.Linecast(this.transform.position,
			target.position, out hit, ~0, QueryTriggerInteraction.Ignore);
		bool hasHitTarget = (hit.transform == target.transform);

		return hasHitTarget;
    }

	private bool IsWithinFOV(Vector3 position)
	{
		Vector3 targetDirection = position - this.transform.position;
		float angle = Vector3.Angle(targetDirection, this.transform.forward);

		return (angle <= this.detectionFOV);
	}
}
