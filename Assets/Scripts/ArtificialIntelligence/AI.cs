using UnityEngine;

public class AI : MonoBehaviour 
{
	[SerializeField] private LayerMask targetLayerMask;
	[SerializeField] private float detectionFOV = 60.0f;
	[SerializeField] private float detectionDiameter = 12.5f;
	[SerializeField] private AIWeaponController weaponController = null;

	private SphereCollider detectionCollider = null;
	private Transform target = null;
	private AIMovementBase movementBase = null;

	private void Start()
	{
		this.movementBase = this.GetComponent<AIMovementBase>();
		this.detectionCollider = this.GetComponent<SphereCollider>();
		this.detectionCollider.radius = this.detectionDiameter / 2.0f;
	}

	private void Update () 
	{
		bool isTargetSpotted = (this.target != null && this.IsTargetSeen(this.target.position));

		// TODO: if not spotted then need to rotate slowly back and forth in an 180 degree arc

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
