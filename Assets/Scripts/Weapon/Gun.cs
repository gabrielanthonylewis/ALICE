using UnityEngine;
using System.Collections;

namespace ALICE.Weapon.Gun
{      
    public class Gun : Weapon
    {
        private enum FireType
        {
            Auto = 0,
            Semi = 1,
            Single = 2
        };

        [SerializeField] private int magSize = 30;
        [SerializeField] private ParticleSystem muzzleFlashPS = null;
        [SerializeField] private TextMesh ammoText = null;
        [SerializeField] private FireType fireType = FireType.Auto;
        [SerializeField] private bool useProjectiles = true;
        [SerializeField] private AudioClip fireSound = null;
        [SerializeField] private AudioClip reloadSound = null;
        [SerializeField] private AudioClip fireTypeSound;

        private Powerup[] powerups = {};
        private int currentPowerupIndex = -1;
        private AudioSource audioSource;
        private int remainingMagAmmo = 0;
        protected bool isFiring = false;
        protected bool isAiming = false;

        protected virtual Vector3 GetFireVector() { return Vector3.zero; }
        protected virtual Vector3 GetFireForwardVector() { return Vector3.zero; }
        protected virtual Vector3 GetFireRayPosition() { return Vector3.zero; }
        protected virtual float GetFireDelay() { return 0.0f; } // TODO: put on Gun.cs?
        

        private void Start()
        {
            this.audioSource = this.GetComponent<AudioSource>();
            this.powerups = this.GetComponents<Powerup>();

            // Current Clip fully loaded.
            this.SetRemainingAmmo(this.magSize);
        }


        public override void OnFireInput(bool isDownOnce)
        {
            // Have to press down to fire a single bullet or burst.
            if(!isDownOnce && this.fireType == FireType.Semi ||
                !isDownOnce && this.fireType == FireType.Single)
            {
                    return;
            }

            bool isReloading = (this.animator.GetCurrentAnimatorStateInfo(0).IsName("reload") ||
                                this.animator.GetCurrentAnimatorStateInfo(0).IsName("reloadads"));
            if (this.isFiring || isReloading)     
                return;

            if (this.remainingMagAmmo > 0)
                this.StartCoroutine(this.Fire());
        }

        private IEnumerator Fire()
        {
            this.isFiring = true;

            int bulletsToShoot = (this.fireType == FireType.Semi) ? 4 : 1;
            for(int i = 0; i < bulletsToShoot; i++)
            {
                if(this.remainingMagAmmo <= 0)
                    break;

                Vector3 randomVector = this.GetFireVector();
                Vector3 forward = this.GetFireForwardVector();
                Vector3 pos = this.GetFireRayPosition();
                float waitDelay = this.GetFireDelay();

                this.FireBullet(randomVector, pos, forward);

                yield return new WaitForSeconds(waitDelay);
            }

            // Auto reload
            if(this.remainingMagAmmo <= 0)
                this.OnReloadInput();

            this.isFiring = false;
        }

        public void FireBullet(Vector3 randomVector, Vector3 rayPos, Vector3 forward)
        {
            this.animator.SetTrigger("shotFired");

            // TODO: use this for slomo im assuming?
            //this.muzzleFlashPS.main.simulationSpeed = 1f * (1f / Time.timeScale); 
            this.EnableMuzzleFlash(true);

            this.audioSource.PlayOneShot(this.fireSound);

            // If the weapon has a projectile to fire then Fire it.
            if (this.transform.GetComponent<FireObject>())
                this.transform.GetComponent<FireObject>().Fire(rayPos);
            else
            {
                // Shoot a bullet via raycasting.
                RaycastHit hit;
                if (Physics.Raycast(rayPos, forward + randomVector, out hit, range))
                {
                    this.onHitEvent?.Invoke();

                    // Spawn a hit particle (if one exists) where the bullet hit (on the surface).
                    //  Instantiate (ObjectHitParticle, hit.point - transform.forward * 0.02f, Quaternion.Euler (hit.normal));

                    if(this.currentPowerupIndex >= 0)
                    {
                        this.powerups[this.currentPowerupIndex].AffectObject(hit.transform);
                        // if (Shrink(hit.transform, 1f))
                        //    hitMarker.sizeDelta = new Vector2(10, 10);
                    }

                    // Add a forwards force (from the players perspective) to the hit object.
                    if (hit.transform.tag != "Enemy")
                    {
                        if (hit.transform.GetComponent<Rigidbody>())
                            hit.transform.GetComponent<Rigidbody>().AddForce(this.transform.forward * 10000f * Time.deltaTime);// TODO: should i be using time.deltatime here?
                    }

                    if (hit.transform.GetComponent<Destructable>())
                        hit.transform.GetComponent<Destructable>().ManipulateHealth(this.damage);
                }
            }

            this.SetRemainingAmmo(this.remainingMagAmmo - 1);
        }

        public override void OnChangeFireTypeInput()
        {
            // Change the Fire Type (Fully Automatic, Burst and Single shot).
            if(this.isFiring)
                return;
            
            // Functionallity only availiable for the Assault Rifle.
            // TODO: if(currentWeapon.GetWeaponType() == Weapon.GunType.AssaultRifle)
            {
                audioSource.clip = fireTypeSound;
                audioSource.Play();
                this.NextFireType();
            }
        }

        public void NextFireType()
        {
            this.fireType = (FireType)Mathf.Repeat((int)this.fireType + 1,
                System.Enum.GetValues(typeof(FireType)).Length);

            this.animator.SetInteger("fireType", (int)this.fireType);
        }

        public override void OnReloadInput()
        {
            base.OnReloadInput();

            bool isMagFull = (this.remainingMagAmmo == this.magSize);
            bool canReload = (!isMagFull && Inventory.instance.GetAmmo() > 0);

            if(!canReload)
                return;

            // If bullets still in clip, add it back to the Inventory's ammo.
            if (this.remainingMagAmmo > 0)
                Inventory.instance.ManipulateAmmo(this.remainingMagAmmo);

            // If there is ammo, add it to the clip (even if can't fill).
            int newAmmo = Inventory.instance.TryTakeAmmo(this.magSize);
            if(newAmmo > 0)
            {
                this.remainingMagAmmo = newAmmo;

                if (ammoText)
                    ammoText.text = remainingMagAmmo.ToString();

                this.StopCoroutine(this.Fire());

                this.animator.SetTrigger("reload");
                this.audioSource.PlayOneShot(this.reloadSound);
            }
        }

        public override void OnAimInput()
        {
            isAiming = !isAiming;

            // TODO: Move into Sniper class
            // If the current weapon is a sniper then activate the Scope.
            //if(this.fireType == FireType.Sniper)
            //    currentWeapon.GetScope().SetActive(isAiming);	

            this.animator.SetTrigger("ads");
        }

        private void SetRemainingAmmo(int ammo)
        {
            this.remainingMagAmmo = ammo;

            if (this.ammoText)
                this.ammoText.text = this.remainingMagAmmo.ToString();
        }

        public override void OnMeleeInput()
        {
            this.animator.SetTrigger("melee");

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
                    //hitMarker.sizeDelta = new Vector2(10,10);
                    hit.transform.GetComponent<Destructable> ().ManipulateHealth (5f);
                }
            }
        }

        public override void OnSwitchPowerupInput()
        {
            if(this.currentPowerupIndex >= 0)
                this.powerups[this.currentPowerupIndex].SetParticleActive(false);

            this.currentPowerupIndex++;
            if(this.currentPowerupIndex >= this.powerups.Length)
                this.currentPowerupIndex = -1;
            else
                this.powerups[this.currentPowerupIndex].SetParticleActive(true);
        }
    
        private void EnableMuzzleFlash(bool enable)
        {
            if (enable)
                this.muzzleFlashPS.Play();
            else
                this.muzzleFlashPS.Stop();

            this.muzzleFlashPS.gameObject.SetActive(enable);
        }

    }
}
