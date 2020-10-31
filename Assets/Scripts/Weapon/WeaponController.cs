using UnityEngine;
using ALICE.Weapon;

// The WeaponController script provides/give access to (through input and references) all of the functionallity to operate the current weapon.
public class WeaponController : MonoBehaviour
{
	[SerializeField] private Weapon	currentWeapon = null;
    [SerializeField] private RectTransform hitMarker = null;
    [SerializeField] private GameObject grenadePrefab = null;
    
    public Inventory inventory { get; private set; }

    private GameObject tempGrenade = null;
    private float initialThrowStrength = 250.0f;
    private float currentThrowStrength;

    private void Start()
    {
        this.inventory = this.GetComponent<Inventory>();
    }

    private void Update()
    {
        // Return the hitMarker's size back to it's orginal size. 
        this.hitMarker.sizeDelta = Vector2.Lerp(this.hitMarker.sizeDelta, new Vector2(4, 4), Time.deltaTime * 20f);

        // If the game is paused then halt all of the behaviour.
        if(Time.timeScale == 0) 
           return;

        if(Input.GetKey(KeyCode.G))
            this.PrepareGrenade();
        else if(Input.GetKeyUp(KeyCode.G))
            this.ThrowGrenade();

        if(this.currentWeapon != null)
        {
            bool isFireDownOnce = false;
            if(Input.GetKeyDown(KeyCode.Mouse0))
                isFireDownOnce = true;
            if(Input.GetKey(KeyCode.Mouse0))
                this.currentWeapon.OnFireInput(isFireDownOnce);

            if(Input.GetKeyDown(KeyCode.Mouse1))
                this.currentWeapon.OnAimInput();

            if(Input.GetKeyDown(KeyCode.R))
                this.currentWeapon.OnReloadInput();

            if(Input.GetKeyDown(KeyCode.B))
                this.currentWeapon.OnChangeFireTypeInput();

            if(Input.GetKeyDown (KeyCode.T))
                this.currentWeapon.OnSwitchPowerupInput();

            if(Input.GetKeyDown (KeyCode.V)) 
                this.currentWeapon.OnMeleeInput();
        }
    }

    public Weapon GetCurrentWeapon()
    {
        return this.currentWeapon;
    }

    private void OnHit()
    {
        this.hitMarker.sizeDelta = new Vector2(10, 10);
    }

    public void EquipWeapon(Weapon weapon)
    {    
        if (this.currentWeapon == weapon)
            return;

        // If already using a weapon turn it off.
        if (this.currentWeapon != null)
        {
            this.currentWeapon.StopAllActivity();
            this.currentWeapon.gameObject.SetActive(false);
        }

        // Show weapon.
        this.currentWeapon = weapon;
        this.currentWeapon.weaponController = this;
        this.currentWeapon.onHitEvent.AddListener(this.OnHit);
        this.currentWeapon.onDroppedEvent.AddListener(this.OnWeaponDropped);
        this.currentWeapon.gameObject.SetActive(true);
    }

    private void OnWeaponDropped()
    {
        this.currentWeapon = null;
    }

    private void PrepareGrenade()
    {
        if(this.inventory.GetGrenades() <= 0)
            return;
        
        // Ready the grenade.
        if (Input.GetKeyDown (KeyCode.G)) 
        {
            this.currentThrowStrength = this.initialThrowStrength;
            this.tempGrenade = Instantiate(this.grenadePrefab, this.transform.position + this.transform.forward, this.grenadePrefab.transform.rotation) as GameObject;
            this.tempGrenade.GetComponent<Rigidbody>().useGravity = false;
            this.tempGrenade.transform.GetChild(0).GetComponent<Collider>().enabled = false;
        }

        this.tempGrenade.transform.position = this.transform.position + this.transform.forward / 1.6f;

        // Increase the distance of the grenade whilst the player is holding it down. 
        // "* (1f / Time.timeScale)" counters the slomo effect affecting the power of the throw.
        this.currentThrowStrength = Mathf.Min(this.currentThrowStrength + 50f * Time.deltaTime * (1f / Time.timeScale), 500.0f);
    }

    private void ThrowGrenade()
    {
        if(this.tempGrenade == null)
            return;

        this.tempGrenade.transform.GetChild(0).GetComponent<Collider>().enabled = true;
        this.tempGrenade.GetComponent<Rigidbody>().useGravity = true;
        // Throw the grenade. "* (1f / Time.timeScale)" counters the slomo effect affecting the power of the throw.
        this.tempGrenade.GetComponent<Rigidbody>().AddForce(this.transform.forward * this.currentThrowStrength, ForceMode.Force);

        this.inventory.ManipulateGrenades(-1);
        this.tempGrenade = null;
        this.currentThrowStrength = this.initialThrowStrength;
    }
}
