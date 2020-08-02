using UnityEngine;
using System.Collections;

namespace ALICE.Weapon.Gun
{      
    public class Gun : Weapon
    {
        private enum FireType
        {
            NULL, Single, Auto, Burst, Sniper
        };

        [SerializeField] private int magSize = 30;
        [SerializeField] private ParticleSystem muzzleFlashPS = null;
        [SerializeField] private TextMesh ammoText = null;
        [SerializeField] private FireType fireType = FireType.NULL;
        [SerializeField] private bool useProjectiles = true;
        [SerializeField] private AudioClip fireSound = null;
        [SerializeField] private AudioClip reloadSound = null;

        private AudioSource audioSource;
        private int remainingMagAmmo = 0;
        protected bool isFiring = false;
        protected bool isAiming = false;
        
        private void Start()
        {
            this.audioSource = this.GetComponent<AudioSource>();

            // Current Clip fully loaded.
            this.SetRemainingAmmo(this.magSize);
        }

        protected virtual Vector3 GetFireVector() { return Vector3.zero; }
        protected virtual Vector3 GetFireForwardVector() { return Vector3.zero; }
        protected virtual Vector3 GetFireRayPosition() { return Vector3.zero; }
        protected virtual float GetFireDelay() { return 0.0f; } // TODO: put on Gun.cs?

        private IEnumerator Fire()
        {
            this.isFiring = true;

            //TODO: May need to do this so animator reacts to slomo 
            //this.animator.speed = (1.0f / Time.deltaTime);

            this.animator.SetTrigger("shotFired");

            Vector3 randomVector = this.GetFireVector();
            Vector3 forward = this.GetFireForwardVector();
            Vector3 pos = this.GetFireRayPosition();
            float waitDelay = this.GetFireDelay();

            this.FireBullet(randomVector, pos, forward);

            yield return new WaitForSeconds(waitDelay);

            // Auto reload
            if(this.remainingMagAmmo <= 0)
                this.OnReloadInput();

            this.isFiring = false;
        }

        public void FireBullet(Vector3 randomVector, Vector3 rayPos, Vector3 forward)
        {
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
                //if (ObjectHitParticle)
                //	Instantiate (ObjectHitParticle, hit.point - transform.forward * 0.02f, Quaternion.Euler (hit.normal));

                /*if (powerup == PowerUp.Shrink)
                {
                    if (Shrink(hit.transform, 1f))
                        hitMarker.sizeDelta = new Vector2(10, 10);
                    return;
                }
                if (powerup == PowerUp.Transparency)
                {
                    if (ReduceAlpha(hit.transform, 0.05f))
                        hitMarker.sizeDelta = new Vector2(10, 10);
                    return;
                }*/

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

        public override void OnFireInput()
        {
            base.OnFireInput();

            bool isReloading = this.animator.GetCurrentAnimatorStateInfo(0).IsName("reload");
            if (this.isFiring || isReloading)
                return;

            if (this.remainingMagAmmo > 0)
                this.StartCoroutine(this.Fire());
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
    }
}
