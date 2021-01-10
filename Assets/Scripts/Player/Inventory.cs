using System;
using UnityEngine;
using UnityEngine.UI;
using ALICE.Checkpoint;

[RequireComponent(typeof(WeaponController))]
public class Inventory: MonoBehaviour 
{	
	[SerializeField] private int ammoCount = 300;
	[SerializeField] private int grenadeCount = 3;
	[SerializeField] private int magSize = 30;
	[SerializeField] private Text grenadeCountText = null;
	[SerializeField] private Text clipCountText = null;
    [SerializeField] private float dropWeaponStrength = 10.0f;

	private WeaponController weaponController = null;
	private Weapon[] weapons = new Weapon[3];

	private void Awake()
	{
		this.weaponController = this.GetComponent<WeaponController>();
		this.RefreshUI();
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Alpha1))
            this.EquipWeapon(0);
        else if(Input.GetKeyDown(KeyCode.Alpha2))
            this.EquipWeapon(1);
        else if(Input.GetKeyDown(KeyCode.Alpha3))
            this.EquipWeapon(2);

		if(Input.GetKeyDown(KeyCode.X))
			this.DropWeapon(this.weaponController.GetCurrentWeapon(), true);
	}

	private void RefreshUI()
	{
		this.grenadeCountText.text = this.grenadeCount.ToString();
        this.clipCountText.text = this.GetClipCount().ToString();
    }

	private int GetClipCount()
	{
		return Mathf.Max(Mathf.CeilToInt(this.ammoCount / this.magSize), 0);
	}

    public bool TryAddWeapon(Weapon weapon)
    {
        if(weapon == null || this.HasWeapon(weapon))
            return false;

		int availableIndex = Array.IndexOf<Weapon>(this.weapons, null);
		bool hasAvailableIndex = (availableIndex > -1);

		if(hasAvailableIndex)
			this.AddWeapon(weapon, availableIndex);

        return hasAvailableIndex;
    }

	public bool HasWeapon(Weapon weapon)
    {
		return (weapon == null) ? false :
			Array.Exists<Weapon>(this.weapons, element => element == weapon);
    }

	public bool HasWeaponType(Weapon weapon)
	{		
		if(weapon == null)
			return false;
		
		if(Array.Exists<Weapon>(this.weapons, element => Type.Equals(element, weapon)))
			return true;
		
		string weaponTypeName = weapon.GetType().FullName.Replace("Enemy", "");
		for(int i = 0; i < this.weapons.Length; i++)
		{
			if(this.weapons[i] == null)
				continue;
				
			if(this.weapons[i].GetType().FullName.Replace("Enemy", "") == weaponTypeName)
				return true;
		}

		return false;
	}

	private void AddWeapon(Weapon weapon, int slotIndex)
	{
		if(slotIndex < 0 || slotIndex >= this.weapons.Length)
			return;

		this.weapons[slotIndex] = weapon;
		this.weapons[slotIndex].OnPickedUp();

		// Equip weapon if the Player isn't holding a weapon.
		if (this.weaponController.GetCurrentWeapon() == null)
			this.EquipWeapon(slotIndex);
	}

	private void EquipWeapon(int slotIndex)
	{
        this.weaponController.EquipWeapon(this.weapons[slotIndex]);
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

	public InventoryData GetInventoryData()
	{
		string[] weaponNames = new string[this.weapons.Length];
		for(int i = 0; i < this.weapons.Length; i++)
		{
			weaponNames[i] = (this.weapons[i] == null) ? "" :
				this.weapons[i].GetResourceName();
		}

		return new InventoryData(this.ammoCount, this.grenadeCount, weaponNames);
	}

	public void LoadInventory(InventoryData data)
	{
		// Create and add weapons.
		for(int i = 0; i < data.weaponNames.Length; i++)
		{
			if(data.weaponNames[i] == "")
				continue;

			GameObject newWeapon = GameObject.Instantiate<GameObject>(
				Resources.Load<GameObject>("Weapons/" + data.weaponNames[i]));
			if(!this.TryAddWeapon(newWeapon.GetComponent<Weapon>()))
				GameObject.Destroy(newWeapon);
		}

		this.ammoCount = data.ammo;
		this.grenadeCount = data.grenades;
		this.RefreshUI();
	}

    public int GetAmmo()
	{	
		return this.ammoCount;
	}

	private void SetAmmo(int value)
	{
		this.ammoCount = Mathf.Max(value, 0);
		this.RefreshUI();
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
	
	private void SetGrenades(int value)
	{
		this.grenadeCount = Mathf.Max(value, 0);
		this.RefreshUI();	
	}
	
	public void ManipulateGrenades(int value)
	{
		this.SetGrenades(this.grenadeCount + value);
	}
	
}
