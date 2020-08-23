using UnityEngine;
using System.Collections;

// The FireObject script provides the functionallity to fire (instantiate and add force) an object.
public class FireObject : MonoBehaviour 
{
	// Projectile to fire.
	[SerializeField] private GameObject Projectile = null;
	[SerializeField] private Transform spawnPosition = null;
	[SerializeField] private float force = 400.0f;

	public void Fire()
	{
		// Fire the projectile at the position "spawnPos".
		GameObject projectile = Instantiate(Projectile, spawnPosition.position + this.transform.forward, this.transform.rotation) as GameObject;

		// Add force to the object using Unity's Physics engine to simulate Projectile Motion.
		projectile.GetComponent<Rigidbody>().AddForce(this.transform.forward * this.force);

	}

}
