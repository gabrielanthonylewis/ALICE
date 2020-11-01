using UnityEngine;

// The FireObject script provides the functionallity to fire (instantiate and add force) an object.
public class FireObject : MonoBehaviour 
{
	[SerializeField] private GameObject projectile = null;
	[SerializeField] private Transform spawnPosition = null;
	[SerializeField] private float force = 400.0f;

	public void Fire()
	{
		GameObject newProjectile = GameObject.Instantiate<GameObject>(this.projectile,
			this.spawnPosition.position, this.transform.rotation);

		newProjectile.GetComponent<Rigidbody>()?.AddForce(this.transform.forward * this.force);
	}

}
