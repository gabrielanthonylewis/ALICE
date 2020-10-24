using UnityEngine;
using ALICE.Weapon;

public class PlayerInteraction : MonoBehaviour 
{
	[SerializeField] private LayerMask layermask;
	[SerializeField] private AudioClip pickupSound;
	[SerializeField] private float rayDistance = 4.0f;

	private AudioSource audioSource = null;

	private void Start()
	{
		this.audioSource = this.GetComponent<AudioSource>();

		// Hide cursor (A hit marker is present).
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update() 
	{
		RaycastHit hit;
		if (Physics.Raycast (this.transform.position, this.transform.forward, out hit, this.rayDistance, this.layermask))
		{
			if(hit.transform.gameObject.layer == 8)
				this.PickupRay(hit);
			if(hit.transform.gameObject.layer == 9)
				this.InteractRay(hit);
		}
	}

	private void PickupRay(RaycastHit hit)
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
			// If the hit object is a Weapon, add it to the Inventory.
			if(hit.transform.gameObject.GetComponent<Weapon>() != null)
				Inventory.instance.AddWeapon(hit.transform.gameObject.GetComponent<Weapon>());

			// If the script has a bonus... (it must be value based)
			if(hit.transform.gameObject.GetComponent<PickUpBonus>())
			{
				// If ammo is rewarded, add the ammo to the Inventory and destroy the pick up object.
				if(hit.transform.gameObject.GetComponent<PickUpBonus>().ammo > 0)
				{
					Inventory.instance.ManipulateAmmo(hit.transform.gameObject.GetComponent<PickUpBonus>().ammo);
					Destroy(hit.transform.gameObject);
				}

				// If health is rewarded, add the health to the Player and destroy the pick up object.
				if(hit.transform.gameObject.GetComponent<PickUpBonus>().health > 0)
				{
					if(this.transform.parent.GetComponent<Destructable>().ManipulateHealth(-hit.transform.gameObject.GetComponent<PickUpBonus>().health))
						Destroy(hit.transform.gameObject);
					else
						return; // return so the pick up sound clip isn't played.
				}

				// If grenades are rewarded, add the grenades to the Inventory and destroy the pick up object.
				if(hit.transform.gameObject.GetComponent<PickUpBonus>().grenades > 0)
				{
					Inventory.instance.ManipulateGrenades(hit.transform.gameObject.GetComponent<PickUpBonus>().grenades);
					Destroy(hit.transform.gameObject);
				}
			}

			// Play the pick up sound clip.
			audioSource.clip = pickupSound;
			audioSource.Play();
		}
	}

	private void InteractRay(RaycastHit hit)
	{	
		if(Input.GetKeyDown(KeyCode.F))
		{
			// If the hit object is a button, "push" it.
			if(hit.transform.GetComponent<ButtonBehaviour>())
			{
				hit.transform.GetComponent<ButtonBehaviour>().Push();
			}

			// If the hit object has an OpenGate component, open the corresponding gate.
			if(hit.transform.gameObject.GetComponent<OpenGate>())
				hit.transform.gameObject.GetComponent<OpenGate>().Open();

			// If the hit object has a PiecePuzzleController component, "play" the puzzle and remove self from the game temporarily.
			if(hit.transform.gameObject.GetComponent<PiecePuzzleController>())
			{
				if(hit.transform.gameObject.GetComponent<PiecePuzzleController>().Play(this.transform.parent))
					this.transform.parent.gameObject.SetActive(false);
			}

			// If a Sequence Controller is hit, play it's sequence.
			if(hit.transform.gameObject.GetComponent<SequenceController>())
				hit.transform.gameObject.GetComponent<SequenceController>().PlaySequence();

			// If a Sequence button itself is hit and it's not busy, "push" it.
			if(hit.transform.gameObject.GetComponent<SequenceButton>())
			{
				if(!hit.transform.gameObject.GetComponent<SequenceButton>().GetBusy())
					hit.transform.gameObject.GetComponent<SequenceButton>().UserPush();
			}

			// If a hit object is rotatable and snaps (auto), rotate the object to the next rotation.
			if(hit.transform.gameObject.GetComponent<Rotatable>())
			{	
				if(hit.transform.gameObject.GetComponent<Rotatable>().GetAuto())
					hit.transform.gameObject.GetComponent<Rotatable>().Rotate();
			}
		
			// If the hit object is a Door, etner it.
			if(hit.transform.gameObject.GetComponent<Door>())
				hit.transform.gameObject.GetComponent<Door>().Enter();
		}

		// If the F key is active and the hit object is rotatable and doesn't snap (!auto), rotate the object.
		if (Input.GetKey (KeyCode.F))
		{
			if(hit.transform.gameObject.GetComponent<Rotatable>())
			{
				if(!hit.transform.gameObject.GetComponent<Rotatable>().GetAuto())
					hit.transform.gameObject.GetComponent<Rotatable>().Rotate();
			}
		}
	}
}
