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
	private Quaternion initialRotation;
	private Quaternion targetRotation;
	private float currentRotationAngle;
	private float rotationTime = 0.0f;
	
	private void Start()
	{
		this.movementBase = this.GetComponent<AIMovementBase>();
		this.detectionCollider = this.GetComponent<SphereCollider>();
		this.detectionCollider.radius = this.detectionDiameter / 2.0f;

		this.currentRotationAngle = this.rotationAngle;
		this.initialRotation = this.transform.localRotation;
		this.targetRotation = this.initialRotation *
			Quaternion.AngleAxis(this.currentRotationAngle, Vector3.up);
		this.rotationTime = this.rotationDuration / 2.0f;
	}

	private void Update() 
	{
		this.wasSpotted = isTargetSpotted;
		this.isTargetSpotted = (this.target != null && this.IsTargetSeen(this.target.position));

		if(!isTargetSpotted)
		{
			// If target no longer spotted then continue looking from current position.
			if(this.wasSpotted)
			{
				this.initialRotation = this.transform.localRotation;
				this.rotationTime = 0.0f;
			}

			// Rotate slowly back and forth, looking for the player.
			this.UpdateIdleLookRotation(Time.deltaTime);
		}

		if(this.movementBase != null)
			this.movementBase.SetTarget((isTargetSpotted) ? this.target : null);

		if(this.weaponController != null)
			this.weaponController.SetTarget((isTargetSpotted) ? this.target : null);

		if(isTargetSpotted)
		{
			// Look towards the target (whilst preventing the body rotating upwards and downwards) 
			Vector3 targetPos = target.transform.position;
			targetPos.y = this.transform.position.y;
			this.transform.LookAt (targetPos, transform.up);
		}
	}

	private void UpdateIdleLookRotation(float dt)
	{
		this.rotationTime += Time.deltaTime;

		this.transform.localRotation = Quaternion.Lerp(this.initialRotation,
			this.targetRotation, this.rotationTime / this.rotationDuration);

		if(this.rotationTime >= this.rotationDuration)
		{
			this.currentRotationAngle = - this.currentRotationAngle;
			this.initialRotation = this.transform.localRotation;
			this.targetRotation =  this.initialRotation * 
				Quaternion.AngleAxis(this.currentRotationAngle * 2.0f, Vector3.up);
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

	private bool IsTargetSeen(Vector3 targetPosition)
    {
		bool isTargetWithinFOV = this.IsWithinFOV(targetPosition);
		bool isTargetObstruced = !Physics.Linecast(this.transform.position,
			targetPosition, this.targetLayerMask);

		return (isTargetWithinFOV && !isTargetObstruced);
    }

	private bool IsWithinFOV(Vector3 position)
	{
		Vector3 targetDirection = position - this.transform.position;
		float angle = Vector3.Angle(targetDirection, this.transform.forward);

		return (angle <= this.detectionFOV);
	}
}
