using UnityEngine;
using UnityEngine.UI;
using ALICE.Weapon;

public class Inventory: MonoBehaviour 
{	
	private Weapon[] weapons = new Weapon[3];
	private int ammoCount = 300;
	private int grenadeCount = 3;
	private int magSize = 30;
	private Text grenadeCountText = null;
	private Text clipCountText = null;
	private WeaponController weaponController = null;
	private Transform mainCameraTransform = null;

	private void Awake()
	{
		// Default Values.
		this.ammoCount = 300;
		this.grenadeCount = 3;

		this.grenadeCountText = GameObject.FindGameObjectWithTag ("GrenadesText").GetComponent<Text>();
		this.clipCountText = GameObject.FindGameObjectWithTag ("ClipsText").GetComponent<Text>();
		this.weaponController = GameObject.FindObjectOfType<WeaponController>();

		this.UpdateUI ();
	}

	public void UpdateUI()
	{
		this.grenadeCountText.text = this.grenadeCount.ToString();
        this.clipCountText.text = this.GetClipCount().ToString();
    }

	private int GetClipCount()
	{
		return Mathf.Max(Mathf.CeilToInt(this.ammoCount / this.magSize), 0);
	}

	public Weapon EquipWeapon(int slotIndex)
	{
        return weaponController.EquipWeapon(this.weapons[slotIndex]);
    }

    public bool AddWeapon(Weapon weapon)
    {
        if(this.HasWeapon(weapon))
            return false;

        // Check through all of the Guns until an empty spot is found.
        for (int i = 0; i < this.weapons.Length; i++)
        {
            if (this.weapons[i] != null)
				continue;

			// Add weapon to spot.
			this.weapons[i] = weapon;
			this.weaponController.PickupWeapon(weapons[i]);

			return true;
        }

        // An empty spot hasn't been found so return false (weapon not added).
        return false;
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
	
	public Weapon DropWeapon(Weapon weapon)
	{
		if(weapon == null)
            return null;

        for (int i = 0; i < this.weapons.Length; i++)
        {
            if(this.weapons[i] != weapon)
				continue;

			this.weapons[i] = null;

			// If another gun is in the Inventory then equip it.
			return this.EquipNextHeldGun(i);
        }

        return null;
	}

    private Weapon EquipNextHeldGun(int slotIndex)
    {
        // Check index after.
        for(int i = slotIndex + 1; i < this.weapons.Length; i++)
        {
            if(this.weapons[i] != null)
				return this.EquipWeapon(i);
        }

        // If not found loop around and check from start.
        for(int i = 0; i < slotIndex; i++)
        {
            if (this.weapons[i] != null)
            	return this.EquipWeapon(i);
        }

        return null;
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
