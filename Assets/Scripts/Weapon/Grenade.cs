using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour 
{
	[SerializeField] private int damage = 10;
	[SerializeField] private float explosionDelay = 5f;
	[SerializeField] private SphereCollider damageZone = null;
	
	private void Awake()
	{
		this.damageZone = this.GetComponent<SphereCollider>();
		this.damageZone.enabled = false;
	}

	private void Start()
	{
		this.StartCoroutine(this.WaitThenExplode(this.explosionDelay));
	}

	private IEnumerator WaitThenExplode(float seconds)
	{
		yield return new WaitForSeconds(seconds);

		// Play the explosion sound clip.
		if(this.GetComponent<AudioSource>())
			this.GetComponent<AudioSource>().Play();

		// Enable trigger collider so that objects inside it can take damage.
		this.damageZone.enabled = true;

		/* Wait a small period of time so that the trigger
		 * can deal damage and force to the surrounding objects. */
		yield return new WaitForSeconds(0.1f);

		// If grenade is attatched to an arrow then destroy it as well.
		if(this.GetComponentInParent<Arrow>() != null)
			GameObject.Destroy(this.transform.parent.gameObject);
		
		GameObject.Destroy(this.gameObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.isTrigger)
			return;

		// Add and explosion force to the object within the trigger.
		if(other.GetComponent<Rigidbody>())
		{
			other.GetComponent<Rigidbody>().AddExplosionForce(1000.0f * Time.deltaTime,
				this.transform.position, this.damageZone.radius);
		}

		// Damage the object within the trigger.
		if(other.GetComponent<Destructable>())
			other.GetComponent<Destructable>().ManipulateHealth(-this.damage);
	}
}
