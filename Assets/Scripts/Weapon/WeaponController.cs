using UnityEngine;
using System.Collections;
using ALICE.Weapon;
using System;

// The WeaponController script provides/give access to (through input and references) all of the functionallity to operate the current weapon.
// This includes reloading, changing the firing mode, aiming, tilting, throwing grenades, switching weapons etc.
public class WeaponController : MonoBehaviour
{
	[SerializeField] private Weapon	currentWeapon = null;
    [SerializeField] private RectTransform hitMarker = null;
    [SerializeField] private GameObject grenadePrefab = null;

    private GameObject tempGrenade = null;
    private float initialThrowStrength = 250.0f;
    private float currentThrowStrength;


    public void PickupWeapon(Weapon weapon)
    {
        Transform mainCameraTransform = Camera.main.transform;

        // Hide weapon (not equiped on default).
        weapon.gameObject.SetActive(false);

        // Turn off colliders and physics.
        if (weapon.transform.GetComponent<BoxCollider>())
            weapon.transform.GetComponent<BoxCollider>().enabled = false;
        if (weapon.transform.GetComponent<Rigidbody>())
            weapon.transform.GetComponent<Rigidbody>().isKinematic = true;

        weapon.name = "Gun";
        weapon.transform.SetParent(mainCameraTransform);
        weapon.transform.localRotation = Quaternion.identity;
        weapon.transform.localPosition = Vector3.zero;

        // Set the weapon's children's layers to "GunLayer" so that the gun will not clip through objects (from the player's perspective).
        Transform[] children = weapon.GetComponentsInChildren<Transform>();
        for (int j = 0; j < children.Length; j++)
            children[j].gameObject.layer = 10;

        // Automatically equip weapon if the Player hasn't already got a weapon equipted.
        if (this.currentWeapon == null)
            EquipWeapon(weapon);
    }

    private void OnHit()
    {
        hitMarker.sizeDelta = new Vector2(10, 10);
    }

    public Weapon EquipWeapon(Weapon weapon)
    {
        // If the gun is already active then return.
        if (this.currentWeapon == weapon)
            return this.currentWeapon;

        if (this.currentWeapon != null)
        {
            this.currentWeapon.gameObject.SetActive(false);
            this.currentWeapon.onHitEvent.RemoveListener(this.OnHit);
        }

        // Activate/show new gun, parenting and positioning the gun in the correct position.
        this.currentWeapon = weapon;
        this.currentWeapon.transform.SetParent(Camera.main.transform);
        this.currentWeapon.transform.localRotation = Quaternion.identity;
        this.currentWeapon.transform.localPosition = Vector3.zero;
        this.currentWeapon.transform.gameObject.SetActive(true);

        this.currentWeapon.onHitEvent.AddListener(this.OnHit);

        return this.currentWeapon;
    }

    private bool DropWeapon(Weapon weapon)
    {
        if (!Inventory.instance.HasWeapon(weapon))
            return false;

        Transform mainCameraTransform = Camera.main.transform;

        // Unparent the weapon and re-enable the colliders and physics.
        weapon.transform.SetParent(null);

        if (weapon.transform.GetComponent<BoxCollider>())
            weapon.transform.GetComponent<BoxCollider>().enabled = true;

        if (weapon.transform.GetComponent<Rigidbody>())
        {
            weapon.transform.GetComponent<Rigidbody>().isKinematic = false;
            // "Throw" weapon forwards.
            // todo: add current movement velocity (convert to force?) to this so always pushed forward at same distance
            // m,aybe use velocity here and not force
            weapon.transform.GetComponent<Rigidbody>().AddForce(mainCameraTransform.forward * 10000.0f * Time.deltaTime);
        }

        // Set the weapon's children's layers to 0 (default) so that the player cannot see the weapon through objects.
        Transform[] children = weapon.GetComponentsInChildren<Transform>();
        for (int j = 0; j < children.Length; j++)
            children[j].gameObject.layer = 0;

        // Sets the weapon's layer to "PickUp" so that the player can pick it back up.
        weapon.gameObject.layer = 8;

        weapon.onHitEvent.RemoveListener(this.OnHit);

        weapon.OnDropped();

        // If another gun is in the Inventory then equip it, otherwise will be null.
        this.currentWeapon = Inventory.instance.DropWeapon(weapon);
        
        return true;
    }

    private void Update()
    {
        // If the game is paused then halt all of the behaviour.
        if(Time.timeScale == 0) 
           return;

        if(currentWeapon != null)
        {
            if (Input.GetKeyDown(KeyCode.X))
                this.DropWeapon(this.currentWeapon);

            bool isFireDownOnce = false;
            if(Input.GetKeyDown(KeyCode.Mouse0))
                isFireDownOnce = true;
            if(Input.GetKey(KeyCode.Mouse0))
                currentWeapon.OnFireInput(isFireDownOnce);

            if(Input.GetKeyDown(KeyCode.Mouse1))
                currentWeapon.OnAimInput();

            if(Input.GetKeyDown(KeyCode.R))
                currentWeapon.OnReloadInput();

            if(Input.GetKeyDown(KeyCode.B))
                currentWeapon.OnChangeFireTypeInput();

            if (Input.GetKeyDown (KeyCode.T))
                currentWeapon.OnSwitchPowerupInput();

            if (Input.GetKeyDown (KeyCode.V)) 
                currentWeapon.OnMeleeInput();
        }

        if (Input.GetKeyDown (KeyCode.Alpha1))
            this.SwitchWeapon(0);
        if (Input.GetKeyDown (KeyCode.Alpha2))
            this.SwitchWeapon(1);
        if (Input.GetKeyDown (KeyCode.Alpha3))
            this.SwitchWeapon(2);

        if(Input.GetKey(KeyCode.G))
            this.PrepareGrenade();
        else if(Input.GetKeyUp(KeyCode.G))
            this.ThrowGrenade();

        // Return the hitMarker's size back to it's orginal size. 
        hitMarker.sizeDelta = Vector2.Lerp(hitMarker.sizeDelta, new Vector2(4, 4), Time.deltaTime * 20f);
    }


    private void SwitchWeapon(int index)
    {
        /* TODO: Cancel whatever weapon is doing and then switch.
         * pherhaps do Any state to switch and have an event in switch which does the switch.
         * therefore it will finish whatever it is doing. However I will need to stop the 
         * burst loop through code otherwise it will fire whilst I am switching. */

        Inventory.instance.EquipWeapon (index);
    }

    private void PrepareGrenade()
    {
        if(Inventory.instance.GetGrenades() <= 0)
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
        this.tempGrenade.GetComponent<Rigidbody>().AddForce(this.transform.forward * this.currentThrowStrength * (1f / Time.timeScale) , ForceMode.Force);

        Inventory.instance.ManipulateGrenades(-1);
        this.tempGrenade = null;
        this.currentThrowStrength = this.initialThrowStrength;
    }
}

 /*
        // TODO: put in AIWeaponController?
        // (Optional) AI ammo count (doesn't have a seperate inventory).
        [SerializeField] private int AiAmmo = 300;
        // is AI? (decided automatically).
        private bool isAI = false;

        // TODO: controller
        // Dependent on whether the Reload Coroutine is being run.
        private bool reloadRou = false;

        void Start()
        {

            // Automatically decide if weapon is owned by an AI.
            if (this.transform.parent == null)
                isAI = false;
            else if (this.transform.parent.GetComponent<AIWeaponController>())
                isAI = true;
            else
                isAI = false;
               
            // Manipulate ammo (because we initially load the clip).
            if (isAI)
                AIManipulateAmmo(-magSize);
            else
                Inventory.instance.ManipulateAmmo(gunType, -magSize);
        }

        public bool Reload()
        {
            // Play Reload animtion.
            if (isAI)
            {

                if (!reloadRou)
                    StartCoroutine("ReloadRou");

                return true;
            }

            return true;
        }

        IEnumerator ReloadRou()
        {
            reloadRou = true;

            // Deactivate muzzle flash.
            muzzleFlashGO.SetActive(false);

            // Play Reload _Animation and wait for it to be complete.
            _Animation.Play("reload");
            yield return new WaitForSeconds(_Animation["reload"].length);

            // Update clip and AI Ammo (taking into account the case where AI ammo cannot fill the clip).
            if (AiAmmo < magSize)
            {
                remainingAmmo = AiAmmo;
                AiAmmo = 0;
            }
            else
            {
                remainingAmmo = magSize;
                AiAmmo -= magSize;
            }

            reloadRou = false;
        }

        public void AIManipulateAmmo(int value)
        {
            AiAmmo += value;
            if (AiAmmo < 0)
                AiAmmo = 0;
        }
        */