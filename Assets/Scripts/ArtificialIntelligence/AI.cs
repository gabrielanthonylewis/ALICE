using UnityEngine;
using System.Collections;
using ALICE.Weapon;

// The AI script provides all of the non-weapon behaviours such as looking at the target,
// following a Roam Path or even charging towards the player depending on what is required.
public class AI : MonoBehaviour 
{
	[SerializeField] private string targetTag = "Player";

	// (Optional) If true signifies that the AI will proceed towards the player once spotted.
	[SerializeField] private bool shouldAdvance = false;
	
	// If true the AI object will face the player.
	[SerializeField] bool LookAtTarget = true;

	[SerializeField] private Weapon	currentWeapon = null;

	[SerializeField] private float detectionFOV = 60.0f;
	[SerializeField] private float detectionDiameter = 12.5f;
	private SphereCollider detectionCollider = null;

	private AIWeaponController _AIWeaponController = null;

	private Transform target = null;
	
	void Start()
	{
		_AIWeaponController = this.GetComponentInChildren<AIWeaponController> ();
		this.detectionCollider = this.GetComponent<SphereCollider>();
		this.detectionCollider.radius = this.detectionDiameter / 2.0f; // radius is half the diameter of a circle
	}

	void Update () 
	{
		// TODO: All AI have the same logic for shooting and looking at the player.

		if(this.HasDetectedPlayer())
		{
			if (LookAtTarget)
			{
				// Look towards the target (but preventing the body rotating upwards and downwards) 
				Vector3 targetPos = target.transform.position;
				targetPos.y = this.transform.position.y;
				this.transform.LookAt (targetPos, transform.up);
			}

			// TODO: Movement is unique so should be a seperate component added on.
			// Move towards player if spotted and should advance.
			if (this.shouldAdvance) 
				this.transform.position = Vector3.MoveTowards (transform.position, target.transform.position, Time.deltaTime);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == this.targetTag)
			this.target = other.transform;
	}

	private void OnTriggerExit(Collider other)
	{
		if(other.tag == this.targetTag)
			this.target = null;
	}

	public bool HasDetectedPlayer()
    {
		if(this.target == null)
			return false;

		// FOV
		Vector3 targetDirection = this.target.position - this.transform.position;
		float angle = Vector3.Angle(targetDirection, this.transform.forward);
		
		if(angle > this.detectionFOV)
			return false;

		// TODO: Use layermask and then dont have to do check can just return the physics line and thats it as it will do true/false
		// Check for obstruction.
		RaycastHit hit;
		if(Physics.Linecast(this.transform.position, this.target.position, out hit))
		{
			if(hit.transform == this.target)
				return true;
		}

        return false;
    }
}
