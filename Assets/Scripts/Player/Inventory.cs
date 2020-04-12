using UnityEngine;
using UnityEngine.UI;
using ALICE.Weapon;
using ALICE.Weapon.Gun;

// The Inventory script is a Singleton implementation that holds the ammo count, an array of guns, grenade count etc.
// Also providing Accessors to these values and functionality to drop, add or equipt a weapon.
public class Inventory 
{
	// Singleton pattern implementation so that there can only be ONE inventory used throughout the game.
	#region Singleton Pattern implementation
	protected Inventory() { }
	
	private static Inventory _instance = null;
	
	public static Inventory instance
	{
		get
		{
			if (Inventory._instance == null)
			{
				Inventory._instance = new Inventory();
			}
			return Inventory._instance;
		}
	}
	#endregion
	
	// List of guns (maximum of 3)
	private Weapon[] Guns = new Weapon[3];

	// Total ammount of Assault Rifle ammo (NOTE that this represents all ammo at this point).
	private int _AR_ammo = 300;

	// Reference to the Player's WeaponController component.
	private WeaponController _WeaponController = null;

	// Ammount of grenades (default at 3)
	private int _Grenades = 3;

	// Reference to the Main Camera's Transform component (optimisation purposes).
	private Transform _MainCamera = null;

	// Reference to the Grendades UI text component (to show the player how many grenades they have).
	private Text _GrenadesUIText = null;

	// Reference to the Clips UI text component (to show the player how many clips/mags they have).
	private Text _ClipsUIText = null;

	// Initalisation function (cannot use Start() or Awake() as not inherited from MonoBehaviour)  
	public void Initialise()
	{
		// Default Values.
		_AR_ammo = 300;
		_Grenades = 3;

		// Get reference to the Grenades UI Text component.
		if (!_GrenadesUIText)
			_GrenadesUIText = GameObject.FindGameObjectWithTag ("GrenadesText").GetComponent<Text>();

		// Get reference to the Clips UI Text component.
		if (!_ClipsUIText)
			_ClipsUIText = GameObject.FindGameObjectWithTag ("ClipsText").GetComponent<Text>();

		UpdateUI ();
	}

	public void UpdateUI()
	{
		// Get reference to the Grenades UI Text component.
		if (!_GrenadesUIText)
			_GrenadesUIText = GameObject.FindGameObjectWithTag ("GrenadesText").GetComponent<Text>();

		// Get reference to the Clips UI Text component.
		if (!_ClipsUIText)
			_ClipsUIText = GameObject.FindGameObjectWithTag ("ClipsText").GetComponent<Text>();

		// Display how many grenades the player has.
		_GrenadesUIText.text = _Grenades.ToString();

        // Calculates how many full clips there are..
        _ClipsUIText.text = Mathf.Max(Mathf.CeilToInt(_AR_ammo / 30f), 0).ToString();
    }

	public Weapon EquipWeapon(int slotIndex)
	{
        WeaponController weaponController = GameObject.FindObjectOfType<WeaponController>();
        return weaponController.EquipWeapon(Guns[slotIndex]);
    }

    public bool AddWeapon(Weapon weapon)
    {
        // If the weapon is already in the Inventory then don't add it!
        if (this.HasWeapon(weapon))
            return false;

        // Check through all of the Guns until an empty spot is found.
        for (int i = 0; i < Guns.Length; i++)
        {
            // If spot in Inventory is empty.
            if (Guns[i] == null)
            {
                // Add weapon to spot, parent it and position it correctly.
                Guns[i] = weapon;

                WeaponController weaponController = GameObject.FindObjectOfType<WeaponController>();
                weaponController.PickupWeapon(Guns[i]);

                return true;
            }
        }

        // An empty spot hasn't been found so return false (weapon not added).
        return false;
    }

    public bool HasWeapon(Weapon weapon)
    {
        if (weapon == null)
            return false;

        for (int i = 0; i < Guns.Length; i++)
        {
            if (Guns[i] == weapon)
                return true;
        }

        return false;
    }	
	
	public Weapon DropWeapon(Weapon weapon)
	{
		if(weapon == null)
            return null;

        for (int i = 0; i < Guns.Length; i++)
        {
            if (Guns[i] == weapon)
            {
                Guns[i] = null;

                // If another gun is in the Inventory then equip it.
                return this.EquipNextHeldGun(i);
            }
        }

        return null;
	}

    private Weapon EquipNextHeldGun(int slotIndex)
    {
        // Check after
        for (int i = slotIndex + 1; i < Guns.Length; i++)
        {
            if (Guns[i] != null)
                return this.EquipWeapon(i);
        }

        // If not found loop around and check from start
        for (int i = 0; i < slotIndex; i++)
        {
            if (Guns[i] != null)
                return this.EquipWeapon(i);
        }

        return null;
    }

    public int GetAmmo(WeaponType weaponType)
	{	// Calculates how many full clips there are..
		if(_ClipsUIText)
            _ClipsUIText.text = Mathf.Max(Mathf.CeilToInt(_AR_ammo / 30f), 0).ToString();
        switch (weaponType) {
		case WeaponType.AssaultRifle:
			return _AR_ammo;
			
		case WeaponType.Pistol:
			Debug.Log("Inventory.cs/GetAmmo(): TODO - Pistol Case");
			break;
			
		case WeaponType.Shotgun:
			Debug.Log("Inventory.cs/GetAmmo(): TODO - Shotgun Case");
			break;
			
		case WeaponType.Sniper:
			Debug.Log("Inventory.cs/SetAmmo(): TODO - Sniper Case");
			return _AR_ammo;
			
			
		default:
			Debug.LogError("Inventory.cs/GetAmmo(): Invalid Weapon Type!");
			return -1;
		}

		return -1;
	}

	public void SetAmmo(WeaponType weaponType, int value)
	{
		switch (weaponType) {
		case WeaponType.AssaultRifle:

			_AR_ammo = value;

			// Enforce lower bound limit.
			if(_AR_ammo < 0)
				_AR_ammo = 0;

			break;
			
		case WeaponType.Pistol:
			Debug.Log("Inventory.cs/SetAmmo(): TODO - Pistol Case");
			break;
			
		case WeaponType.Shotgun:
			Debug.Log("Inventory.cs/SetAmmo(): TODO - Shotgun Case");
			break;
			
		case WeaponType.Sniper:
			Debug.Log("Inventory.cs/SetAmmo(): TODO - Sniper Case");

			_AR_ammo = value;

			// Enforce lower bound limit.
			if(_AR_ammo < 0)
				_AR_ammo = 0;

			break;
			
		default:
			Debug.LogError("Inventory.cs/SetAmmo(): Invalid Weapon Type!");
			break;
		}

		// Get reference to the Clips UI Text component.
		if (!_ClipsUIText)
			_ClipsUIText = GameObject.FindGameObjectWithTag ("ClipsText").GetComponent<Text>();

        // Calculates how many full clips there are..
        _ClipsUIText.text = Mathf.Max(Mathf.CeilToInt(_AR_ammo / 30f), 0).ToString();
    }

	public void ManipulateAmmo(WeaponType weaponType, int value)
	{

		switch (weaponType) 
		{
			case WeaponType.AssaultRifle:
		
				_AR_ammo += value;

                _ClipsUIText.text = Mathf.Max(Mathf.CeilToInt(_AR_ammo / 30f), 0).ToString();

                // Enforce lower bound limit.
                if (_AR_ammo < 0)
				{
					_AR_ammo = 0;
					_ClipsUIText.text = "0"; // Ensure Clips Text displays 0.
				}

				break;
				
			case WeaponType.Pistol:
				Debug.Log("Inventory.cs/ManupulateAmmo(): TODO - Pistol Case");
				break;
				
			case WeaponType.Shotgun:
				Debug.Log("Inventory.cs/ManupulateAmmo(): TODO - Shotgun Case");
				break;
				
			case WeaponType.Sniper:
				//Debug.Log("Inventory.cs/ManupulateAmmo(): TODO - Sniper Case");

				_AR_ammo += value;

				// Enforce lower bound limit.
				if (_AR_ammo < 0)
				{
					_AR_ammo = 0;
					_ClipsUIText.text = "0"; // Ensure Clips Text displays 0.
				}
				break;
				
			default:
				Debug.LogError("Inventory.cs/ManupulateAmmo(): Invalid Weapon Type!");
				break;
		}

		// Get reference to the Clips UI Text component.
		if (!_ClipsUIText) 
		{
			if(GameObject.FindGameObjectWithTag ("ClipsText"))
				_ClipsUIText = GameObject.FindGameObjectWithTag ("ClipsText").GetComponent<Text> ();
		}

		// Calculates how many full clips there are..
		if(_ClipsUIText)
            _ClipsUIText.text = Mathf.Max(Mathf.CeilToInt(_AR_ammo / 30f), 0).ToString();
    }
	
	public int GetGrenades()
	{
		return _Grenades;
	}
	
	public void SetGrenades(int value)
	{
		_Grenades = value;

		// Get reference to the Grenades UI Text component.
		if (!_GrenadesUIText)
			_GrenadesUIText = GameObject.FindGameObjectWithTag ("GrenadesText").GetComponent<Text>();
		
		// Display how many grenades the player has.
		_GrenadesUIText.text = _Grenades.ToString();
	}
	
	public void ManipulateGrenades(int value)
	{
		_Grenades += value;

		// Enforce lower bound limit.
		if(_Grenades < 0)
			_Grenades = 0;

		// Get reference to the Grenades UI Text component.
		if (!_GrenadesUIText)
			_GrenadesUIText = GameObject.FindGameObjectWithTag ("GrenadesText").GetComponent<Text>();
		
		// Display how many grenades the player has.
		_GrenadesUIText.text = _Grenades.ToString();
	}
	
}
