using UnityEngine;
using UnityEngine.UI;
using ALICE.Weapon;

public class Inventory: MonoBehaviour 
{	
	private Weapon[] weapons = new Weapon[3];

	[SerializeField] private int ammoCount = 300;
	[SerializeField] private int grenadeCount = 3;
	[SerializeField] private int magSize = 30;
	[SerializeField] private Text grenadeCountText = null;
	[SerializeField] private Text clipCountText = null;
    [SerializeField] private float dropWeaponStrength = 10.0f;


	private WeaponController weaponController = null;

	private void Awake()
	{
		this.weaponController = GameObject.FindObjectOfType<WeaponController>();

		this.UpdateUI();
	}

	private void Update()
	{
		if (Input.GetKeyDown (KeyCode.Alpha1))
            this.EquipWeapon(0);
        if (Input.GetKeyDown (KeyCode.Alpha2))
            this.EquipWeapon(1);
        if (Input.GetKeyDown (KeyCode.Alpha3))
            this.EquipWeapon(2);

		if (Input.GetKeyDown(KeyCode.X))
			this.DropWeapon(this.weaponController.GetCurrentWeapon(), true);
	}

	private void DropWeapon(Weapon weapon, bool shouldEquipNextWeapon = false)
	{
		if(weapon == null)
			return;

		// Throw weapon forwards.
		Rigidbody weaponRigidbody = weapon.transform.GetComponent<Rigidbody>();
		if (weaponRigidbody != null)
			weaponRigidbody.AddForce(Camera.main.transform.forward * this.dropWeaponStrength);

		weapon.OnDropped();

		for (int i = 0; i < this.weapons.Length; i++)
		{
			if(this.weapons[i] != weapon)
				continue;

			this.weapons[i] = null;

			if(shouldEquipNextWeapon)
				this.EquipNextHeldGun(i);

			break;
		}
	}

	private void UpdateUI()
	{
		this.grenadeCountText.text = this.grenadeCount.ToString();
        this.clipCountText.text = this.GetClipCount().ToString();
    }

	private int GetClipCount()
	{
		return Mathf.Max(Mathf.CeilToInt(this.ammoCount / this.magSize), 0);
	}

	private void EquipWeapon(int slotIndex)
	{
        this.weaponController.EquipWeapon(this.weapons[slotIndex]);
    }

    public bool TryAddWeapon(Weapon weapon)
    {
        if(this.HasWeapon(weapon))
            return false;

        // Check through all of the Guns until an empty spot is found.
        for (int i = 0; i < this.weapons.Length; i++)
        {
            if (this.weapons[i] != null)
				continue;

			this.AddWeapon(weapon, i);

			return true;
        }

        // An empty spot hasn't been found so return false (weapon not added).
        return false;
    }

	private void AddWeapon(Weapon weapon, int slotIndex)
	{
		this.weapons[slotIndex] = weapon;
		this.weapons[slotIndex].OnPickedUp();

		// Equip weapon if the Player isn't holding a weapon.
		if (this.weaponController.GetCurrentWeapon() == null)
			this.EquipWeapon(slotIndex);
	}

    public bool HasWeapon(Weapon weapon)
    {
		if(weapon == null)
			return false;

		foreach(Weapon inventoryWeapon in this.weapons)
		{
			if(inventoryWeapon == weapon)
				return true;
		}

        return false;
    }	

    private void EquipNextHeldGun(int slotIndex)
    {
        // Check index after.
        for(int i = slotIndex + 1; i < this.weapons.Length; i++)
        {
            if(this.weapons[i] != null)
				this.EquipWeapon(i);
        }

        // If not found loop around and check from start.
        for(int i = 0; i < slotIndex; i++)
        {
            if (this.weapons[i] != null)
            	this.EquipWeapon(i);
        }
    }

    public int GetAmmo()
	{	
		return this.ammoCount;
	}

	public void SetAmmo(int value)
	{
		this.ammoCount = Mathf.Max(value, 0);
		this.UpdateUI();
    }

	public void ManipulateAmmo(int value)
	{
		this.SetAmmo(this.ammoCount + value);
    }

	/**
	 * Will attempt to take the desired ammo from the inventory.
	 * If there is not enough ammo it will take as much as possible.
	 */
	public int TryTakeAmmo(int ammo)
	{
		int diff = Mathf.Abs(this.ammoCount - ammo);
		int ammoToTake = Mathf.Min(diff, ammo);

		this.ManipulateAmmo(-ammoToTake);

		return ammoToTake;
	}
	
	public int GetGrenades()
	{
		return this.grenadeCount;
	}
	
	public void SetGrenades(int value)
	{
		this.grenadeCount = Mathf.Max(value, 0);
		this.UpdateUI();	
	}
	
	public void ManipulateGrenades(int value)
	{
		this.SetGrenades(this.grenadeCount + value);
	}
	
}
