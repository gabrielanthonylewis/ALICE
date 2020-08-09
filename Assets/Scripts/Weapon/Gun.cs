using UnityEngine;
using System.Collections;

public enum PowerUp
{
    NULL = 0,
    Shrink = 1,
    Transparency = 2
};

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
        [SerializeField] private ParticleSystem shrinkPS = null;
        [SerializeField] private ParticleSystem transparencyPS = null;
        [SerializeField] private AudioClip powerupSound = null;
        private PowerUp powerup = PowerUp.NULL;

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
            //TODO: May need to do this so animator reacts to slomo 
            //this.animator.speed = (1.0f / Time.deltaTime);
            this.animator.SetTrigger("shotFired");

            // Activate and Play the Muzzle Flash as the gun is now firing.
            //this.muzzleFlashPS.main.simulationSpeed = 1f * (1f / Time.timeScale); // TODO: use this for slomo im assuming?
            this.EnableMuzzleFlash(true);

            this.audioSource.PlayOneShot(this.fireSound);

            // TODO: no point in hasProjectile then?
            // If the weapon has a projectile to fire then Fire it.
            if (this.transform.GetComponent<FireObject>())
            {
                this.transform.GetComponent<FireObject>().Fire(rayPos);
                return;
            }

            // If the gun fires a ray bullet...
            RaycastHit hit;
            // If the bullet hits an object add a force and deal damage.
            if (Physics.Raycast(rayPos, forward + randomVector, out hit, range))
            {
                if (this.onHitEvent != null)
                    this.onHitEvent.Invoke();

                // Spawn a hit particle (if one exists) where the bullet hit (on the surface).
                //if (ReduceAlpha)
                //	Instantiate (ObjectHitParticle, hit.point - transform.forward * 0.02f, Quaternion.Euler (hit.normal));

                if (powerup == PowerUp.Shrink)
                {
                    this.Shrink(hit.transform, 1f);
                    // if (Shrink(hit.transform, 1f))
                    //    hitMarker.sizeDelta = new Vector2(10, 10);
                    return;
                }
                if (powerup == PowerUp.Transparency)
                {
                    this.ReduceAlpha(hit.transform, 0.05f);
                    //if (ReduceAlpha(hit.transform, 0.05f))
                    //    hitMarker.sizeDelta = new Vector2(10, 10);
                    return;
                }

                // Add a forwards force (from the players perspective) to the hit object.
                if (hit.transform.tag != "Enemy")
                {
                    if (hit.transform.GetComponent<Rigidbody>())
                        hit.transform.GetComponent<Rigidbody>().AddForce(this.transform.forward * 10000f * Time.deltaTime);// TODO: should i be using time.deltatime here?
                }

                if (hit.transform.GetComponent<Destructable>())
                {
                    // Deal damage to the hit object (depends on the damage of the weapon).
                    hit.transform.GetComponent<Destructable>().ManipulateHealth(this.damage);
                }
            }

            // Reduce the current gun's clip by 1.
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
            // Start/Stop Aiming Down the Gun's Sight depending on the current state.
            isAiming = !isAiming;

            // TODO: Move into Sniper class
            // If the current weapon is a sniper then activate the Scope.
            //if(this.fireType == FireType.Sniper)
            //    currentWeapon.GetScope().SetActive(isAiming);	

            this.animator.SetTrigger("ads");
        }

        public override void OnDropped()
        {
            base.OnDropped();
        }

        private void OnEmptyMag()
        {
        }

        private void SetRemainingAmmo(int ammo)
        {
            this.remainingMagAmmo = ammo;

            if (this.ammoText)
                this.ammoText.text = this.remainingMagAmmo.ToString();

            if (this.remainingMagAmmo <= 0)
                this.OnEmptyMag();
        }

        private void EnableMuzzleFlash(bool enable)
        {
            if (enable)
                this.muzzleFlashPS.Play();
            else
                this.muzzleFlashPS.Stop();

            this.muzzleFlashPS.gameObject.SetActive(enable);
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
            this.SwitchPowerUp();
        }

        public void SwitchPowerUp()
        {
            this.powerup = (PowerUp)Mathf.Repeat((int)this.powerup + 1,
                System.Enum.GetValues(typeof(PowerUp)).Length);

            shrinkPS.gameObject.SetActive(powerup == PowerUp.Shrink);
            transparencyPS.gameObject.SetActive(powerup == PowerUp.Transparency);
        }
              
        private bool Shrink(Transform target, float val)
        {
            if (target.tag != "Resizable")
                return false;

            if (target.GetComponent<Transform>().localScale.x < 0.2f
                && target.GetComponent<Transform>().localScale.y < 0.2f
                && target.GetComponent<Transform>().localScale.z < 0.2f
               )
            {
                // Play power up sound to indicate no more can be done.
                if (powerupSound)
                {
                    if (Camera.main.GetComponent<AudioSource>())
                    {
                        Camera.main.GetComponent<AudioSource>().clip = powerupSound;
                        Camera.main.GetComponent<AudioSource>().Play();
                    }
                }
                return false;
            }

            // Downscale object.
            target.GetComponent<Transform>().localScale /= 1.1f;

            return true;
        }

        private bool ReduceAlpha(Transform target, float val)
        {
            if (target.tag != "Transparent")
                return false;

            Color tempCol = target.GetComponent<MeshRenderer>().material.color;
            tempCol.a -= val;

            // Disable the collider.
            if (tempCol.a <= 0.4f)
            {
                target.GetComponent<Collider>().enabled = false;
                // Play power up sound to indicate no more can be done.
                if (powerupSound)
                {
                    if (Camera.main.GetComponent<AudioSource>())
                    {
                        Camera.main.GetComponent<AudioSource>().clip = powerupSound;
                        Camera.main.GetComponent<AudioSource>().Play();
                    }
                }
                return false;
            }

            target.GetComponent<MeshRenderer>().material.color = tempCol;

            return true;
        }
    }
}
