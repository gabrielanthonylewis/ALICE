using UnityEngine;
using ALICE.Weapon;

// The AI Weapon Controller script manages the weapon specfic behaviour automatiaclly for the AI.
// It deal with Reloading and firing as well as Spotting the targets through multiple ray casts.
public class AIWeaponController : MonoBehaviour 
{
	[SerializeField] private Weapon	currentWeapon = null;
    private Transform target = null;

	private void Update()
	{
		// "Look" (aim gun) towards the player.
		if(this.target != null)
		{
			this.transform.LookAt (this.target.transform.position, transform.up);

			this.currentWeapon.OnFireInput(false);
		}
	}

	public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
