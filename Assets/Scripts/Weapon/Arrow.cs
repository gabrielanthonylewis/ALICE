using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour
{
	[SerializeField] private int damage = 5;

	private void OnTriggerEnter(Collider other)
	{
		if(other.isTrigger)
			return;

		if(other.tag == "IgnoreCollision")
			return;
	
		// Stick the arrow into the object.
		Rigidbody rigidbody = this.GetComponent<Rigidbody>();
		rigidbody.isKinematic = true;
		rigidbody.useGravity = false;

		// Deal damage.	
		if(other.GetComponent<Destructable>()) 
			other.GetComponent<Destructable>().ManipulateHealth(-this.damage);

		this.GetComponent<Collider>().enabled = false;

		this.StartCoroutine(this.WaitThenDestroy(5.0f));
	}

	private IEnumerator WaitThenDestroy(float delay)
	{
		yield return new WaitForSeconds(delay);

		GameObject.Destroy(this.gameObject);
	}
}
