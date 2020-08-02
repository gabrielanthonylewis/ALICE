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

            if(Input.GetKey(KeyCode.Mouse0))
                currentWeapon.OnFireInput();

            if(Input.GetKeyDown(KeyCode.Mouse1))
                currentWeapon.OnAimInput();

            if(Input.GetKeyDown(KeyCode.R))
                currentWeapon.OnReloadInput();
        }

        // Return the hitMarker's size back to it's orginal size. 
        hitMarker.sizeDelta = Vector2.Lerp(hitMarker.sizeDelta, new Vector2(4, 4), Time.deltaTime * 20f);
    }  

    /*
     * IEnumerator Fire()
    {
        // Offset of the ray creating a Recoil effect.
        Vector3 randomVector = Vector3.zero;

        if (currentWeapon.GetFireType () == Weapon.FireType.Burst) 
        {
            // Shoot a burst of 4 bullets.
            for (int i = 0; i < 4; i++) 
            {
                // If not aiming down the sight add some recoil (less than if fully automatic).
                if (!isAiming)
                    randomVector = new Vector3 (Random.Range (-0.03f, 0.03f), Random.Range (-0.03f, 0.03f), Random.Range (-0.03f, 0.0f));

                // Fire a bullet.
                FireBullet(randomVector);

                // Wait for _Animation to finish before firing again.
                do
                {
                    yield return null;
                } 
                while ( currentWeapon.GetAnimation ().isPlaying );

                // Stop emitting the Muzzle Flash (as not firing).
                currentWeapon.GetMuzzleFlashPS ().enableEmission = false;
                currentWeapon.GetMuzzleFlashPS().playbackSpeed = 1f *(1f / Time.timeScale);
            }

            // Turn off the Muzzle Flash as completely done with it.
            currentWeapon.GetMuzzleFlashGO ().SetActive (false);
        } 
        else 
        {
          
            if (currentWeapon && currentWeapon.GetFireType () == Weapon.FireType.Auto) 
            {
                // Wait for _Animation to finish before firing again.
                do
                {
                    yield return null;
                }
                while (currentWeapon && currentWeapon.GetAnimation ().isPlaying );
            }
            else if(currentWeapon.GetFireType() == Weapon.FireType.Sniper)
            { 
                // Wait an extended period of time (game balance reasons).
                yield return new WaitForSeconds (1.5f *(1f / Time.timeScale));
            }
            // No delay for Assault rifle but delay for a single shot pistol for example.
            else if (currentWeapon.GetWeaponType() != Weapon.GunType.AssaultRifle 
                        && currentWeapon.GetFireType () == Weapon.FireType.Single)
            {
                yield return new WaitForSeconds (0.1f *(1f / Time.timeScale));
            }

            // Turn off the Muzzle Flash as completely done with it.
            if(currentWeapon)
                currentWeapon.GetMuzzleFlashGO ().SetActive (false);
        }
    }
     */

    /*
    // The ParticleSystem to be instantiated upon a bullet hitting an object.
    [SerializeField] private ParticleSystem ObjectHitParticle;

    // Fire rate changing sound clip. 
    [SerializeField] private AudioClip FireRateSound;

    // GameObject to be instantiated upon throwing a grenade.
    [SerializeField] private GameObject _GrenadePrefab = null;

    // Traks whether the player is tiliting left or right.
    private bool tiltRight = false, tiltLeft = false;

    // A reference to a potential grenade to be thrown.
    private GameObject tempGrenade = null;

    // A mutliplier used to increase the distance the grenade is thrown.
    private float _grenadeThrowMulti = 250f;

    void Update ()
    {
        // If the game is paused then halt all of the behaviour.
        if(Time.timeScale == 0) 
            return;

        // Ready a Grenade if there is one in the Inventory.
        if(Inventory.instance.GetGrenades() > 0)
        {
            if (Input.GetKey (KeyCode.G)) 
            {
                // Ready the grenade.
                if (Input.GetKeyDown (KeyCode.G)) 
                {
                    _grenadeThrowMulti = 250f;
                    tempGrenade = Instantiate(_GrenadePrefab, this.transform.position + this.transform.forward, _GrenadePrefab.transform.rotation) as GameObject;
                    tempGrenade.GetComponent<Rigidbody>().useGravity = false;
                    tempGrenade.transform.GetChild(0).GetComponent<Collider>().enabled = false;
                }

                // Update the grenades position.
                if(tempGrenade)
                    tempGrenade.transform.position = this.transform.position + this.transform.forward /1.6f;

                // Increase the distance of the grenade whilst the player is holding it down. "* (1f / Time.timeScale)" counters the slomo effect affecting the power of the throw.
                _grenadeThrowMulti += 50f * Time.deltaTime * (1f / Time.timeScale);

                // Limit the distance the grenade can be thrown.
                if(_grenadeThrowMulti > 500f)
                    _grenadeThrowMulti = 500f;
            }

            // Throw the grenade.
            if(Input.GetKeyUp(KeyCode.G))
            {
                if(tempGrenade)
                {
                    tempGrenade.transform.GetChild(0).GetComponent<Collider>().enabled = true;
                    tempGrenade.GetComponent<Rigidbody>().useGravity = true;
                    // Throw the grenade. "* (1f / Time.timeScale)" counters the slomo effect affecting the power of the throw.
                    tempGrenade.GetComponent<Rigidbody>().AddForce(this.transform.forward * _grenadeThrowMulti * (1f / Time.timeScale) , ForceMode.Force);
                    // Remove one grenade from the Inventory. 
                    Inventory.instance.ManipulateGrenades(-1); 
                }
            }
        }

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

        // Switch Power Up
        if (Input.GetKeyDown (KeyCode.T))
            currentWeapon.SwitchPowerUp();

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

            // Change the Fire Type (Fully Automatic, Burst and Single shot).
            if(Input.GetKeyDown(KeyCode.B) && !fireRou) // if not firing
            {
                // Functionallity only availiable for the Assault Rifle.
                if(currentWeapon.GetWeaponType() == Weapon.GunType.AssaultRifle)
                {
                    audioSource.clip = FireRateSound;
                    audioSource.Play();
                    currentWeapon.NextFireType();

                    // Aiming is interupted so set it to false.
                    isAiming = false;
                }
            }

        }
    }
    */
}
