using UnityEngine;
using ALICE.Weapon;

public class AIWeaponController : MonoBehaviour 
{
	[SerializeField] private Weapon	currentWeapon = null;

    private Transform target = null;

	private void Update()
	{
		// Aim gun towards the player.
		if(this.target != null)
		{
			this.transform.LookAt(this.target.transform.position, this.transform.up);
			this.currentWeapon.OnFireInput(false);
		}
	}

	public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
