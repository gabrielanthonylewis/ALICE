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
        }

        if(Input.GetKey(KeyCode.G))
            this.PrepareGrenade();
        else if(Input.GetKeyUp(KeyCode.G))
            this.ThrowGrenade();

        // Return the hitMarker's size back to it's orginal size. 
        hitMarker.sizeDelta = Vector2.Lerp(hitMarker.sizeDelta, new Vector2(4, 4), Time.deltaTime * 20f);
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

    /*
    private bool tiltRight = false, tiltLeft = false;

    void Update ()
    {
        // If the game is paused then halt all of the behaviour.
        if(Time.timeScale == 0) 
            return;

        // Attempt to change weapon depending on the key pressed (1, 2 or 3).
        if (currentWeapon && !currentWeapon.GetAnimation ().isPlaying) 
        {
            if (Input.GetKeyDown (KeyCode.Alpha1) || Input.GetKeyDown (KeyCode.Alpha2) || Input.GetKeyDown (KeyCode.Alpha3))
                fireRou = false;

            if (Input.GetKeyDown (KeyCode.Alpha1))
                Inventory.instance.EquipWeapon (0);
            if (Input.GetKeyDown (KeyCode.Alpha2))
                Inventory.instance.EquipWeapon (1);
            if (Input.GetKeyDown (KeyCode.Alpha3))
                Inventory.instance.EquipWeapon (2);
        }

        // If there is no Current weapon then weapon behaviour is not possible so return.
        if(currentWeapon == null) return;
               
        // Melee
        if (Input.GetKeyDown (KeyCode.V)) 
        {
            // If idle then can melee.
            if(currentWeapon.GetAnimation ().isPlaying == false)
            {
                fireRou = false;

                // Play a different Melee _Animation depending on whether or not the current weapon is a Sniper.
                if(currentWeapon.GetWeaponType() == Weapon.GunType.Sniper)
                    currentWeapon.GetAnimation ().Play ("meleeSniper");
                else
                    currentWeapon.GetAnimation ().Play ("melee");

                // If an object is hit then apply force and reduce it's health.
                RaycastHit hit;
                if (Physics.Raycast (this.transform.position, this.transform.forward, out hit, 2f))
                {				
                    // Apply force to hit object. "* (1f / Time.timeScale)" counters the slomo effect affecting the power of the throw.
                    if (hit.transform.GetComponent<Rigidbody> ())
                        hit.transform.GetComponent<Rigidbody> ().AddForce (this.transform.forward * 20000f * Time.deltaTime *  (1f / Time.timeScale));

                    if (hit.transform.GetComponent<Destructable> ())
                    {
                        // Increase the size of the hitMarker to show that an object with health has been hit.
                        hitMarker.sizeDelta = new Vector2(10,10);
                        hit.transform.GetComponent<Destructable> ().ManipulateHealth (5f);
                    }

                }

                // Aiming is interupted so set it to false.
                isAiming = false;
            }
        }

        // If weapon exists and NOT reloading...
        if (currentWeapon != null && !currentWeapon.GetAnimation ().IsPlaying ("reloadads") 
            // (Allows the player the aim down sight whilst shooting the gun but not when doing anything else like changing fire mode)
            && ((currentWeapon.GetAnimation ().IsPlaying ("recoil") || (currentWeapon.GetAnimation ().IsPlaying ("recoilads"))
                || !currentWeapon.GetAnimation().isPlaying)))
        {
            // Tilt Right OR back to the normal state depending on current tilt state.
            if (Input.GetKeyDown (KeyCode.E)) 
            {
                tiltRight = !tiltRight;

                // Play backwards/forwards depending on the current tilt state.
                if (tiltRight == true)
                    currentWeapon.GetAnimation () ["tiltRight"].speed = 1;
                else
                    currentWeapon.GetAnimation () ["tiltRight"].speed = -1;

                currentWeapon.GetAnimation ().Play ("tiltRight");
            }

            // Tilt Left OR back to the normal state depending on current tilt state.
            if (Input.GetKeyDown (KeyCode.Q)) 
            {
                tiltLeft = !tiltLeft;

                // Play backwards/forwards depending on the current tilt state.
                if (tiltLeft == true)
                    currentWeapon.GetAnimation () ["tiltLeft"].speed = 1;
                else
                    currentWeapon.GetAnimation () ["tiltLeft"].speed = -1;

                currentWeapon.GetAnimation ().Play ("tiltLeft");
            }

        }
    }
    */
}
