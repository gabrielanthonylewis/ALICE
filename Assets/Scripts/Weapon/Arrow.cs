using UnityEngine;
using System.Collections;

// The Arrow script deal with the projectile behaviour of an arrow,
// stopping when it hits an object and dealing damage.
public class Arrow : MonoBehaviour
{
	
	// Reference to Rigidbody component (optimisation)
	private Rigidbody _Rigidbody = null;

	private bool hasCollided = false;

	void Start()
	{
		// Assign reference to Rigidbody component.
		_Rigidbody = this.GetComponent<Rigidbody> ();
	}

	void OnTriggerEnter(Collider other)
	{
		if(this.hasCollided)
			return;

		if(other.tag == "IgnoreCollision")
			return;

		this.hasCollided = true;
	
		this.GetComponent<Collider> ().enabled = false;

		// Stick the arrow in the hit object.
		_Rigidbody.isKinematic = true;
		_Rigidbody.useGravity = false;
			
		if (other.GetComponent<Destructable>()) 
			other.GetComponent<Destructable>().ManipulateHealth(5f);
	}
}
