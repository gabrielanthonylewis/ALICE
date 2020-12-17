using UnityEngine;

public class WeaponController : MonoBehaviour
{
	[SerializeField] private Weapon	currentWeapon = null;
    [SerializeField] private RectTransform hitMarker = null;
    
    public Inventory inventory { get; private set; }
    private Vector2 minHitMarkerSize;

    private void Start()
    {
        this.minHitMarkerSize = this.hitMarker.sizeDelta;
        this.inventory = this.GetComponent<Inventory>();
    }

    private void Update()
    {
        if(Time.timeScale == 0) 
           return;

        // Return the hitMarker's size back to its orginal size. 
        this.hitMarker.sizeDelta = Vector2.Lerp(this.hitMarker.sizeDelta,
            this.minHitMarkerSize, Time.deltaTime * 20.0f);

        this.HandleWeaponInput();
    }

    private void OnHit()
    {
        this.hitMarker.sizeDelta = this.minHitMarkerSize * 2.0f;
    }

    private void HandleWeaponInput()
    {
        if(this.currentWeapon == null)
            return;

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

    public Weapon GetCurrentWeapon()
    {
        return this.currentWeapon;
    }
}
