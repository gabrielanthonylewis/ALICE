using UnityEngine;

namespace ALICE.Weapon.Gun
{
    // todo: do i actually need this as I have different classes soo?
    public enum GunType
    {
        NULL, AssaultRifle, Shotgun, Pistol, Sniper
    };
       
    public class Gun : Weapon
    {
        private enum FireType
        {
            NULL, Single, Auto, Burst, Sniper
        };

        [SerializeField] private int magSize = 30;
        [SerializeField] private ParticleSystem muzzleFlashPS = null;
        [SerializeField] private TextMesh ammoText = null;
        [SerializeField] private GunType gunType = GunType.NULL;
        [SerializeField] private FireType fireType = FireType.NULL;
        [SerializeField] private bool useProjectiles = true;
        [SerializeField] private AudioClip fireSound = null;

        private AudioSource audioSource;
        private int remainingAmmo = 0;


        private void Start()
        {
            this.audioSource = this.GetComponent<AudioSource>();

            // Current Clip fully loaded.
            this.SetRemainingAmmo(this.magSize);
        }

        public override void OnFireInput()
        {
            base.OnFireInput();

            if(this.remainingAmmo > 0)
            {
                // If the weapon has a scope then fire it from that position.
                /*if (this.GetScope() != null)
                {
                    forward = this.GetScope().gameObject.transform.forward;
                    rayPos = this.GetScope().transform.position;
                }

                // Dramatically bigger offset if the sniper is being used (hip fire).
                if (this.GetWeaponType() == Weapon.GunType.Sniper)
                    randomVector *= 10f;
                    */

                // Different recoil _Animation played depending on if Aiming or not and if the Sniper is being used (more dramatic).
                /*if (ads)
                {
                    currentWeapon.GetAnimation()["recoilads"].speed = (1f / Time.timeScale);
                    currentWeapon.GetAnimation().Play("recoilads");
                }
                else if (currentWeapon.GetWeaponType() != Weapon.GunType.Sniper)
                {
                    currentWeapon.GetAnimation()["recoil"].speed = (1f / Time.timeScale);
                    currentWeapon.GetAnimation().Play("recoil");

                }
                else if (currentWeapon.GetWeaponType() == Weapon.GunType.Sniper)
                {
                    currentWeapon.GetAnimation()["recoilSniper"].speed = (1f / Time.timeScale);
                    currentWeapon.GetAnimation().Play("recoilSniper");
                }*/

                Vector3 forward = Camera.main.transform.forward;
                Vector3 pos = Camera.main.transform.position;

                this.FireBullet(Vector3.zero, pos, forward);
            }
        }

        public void FireBullet(Vector3 randomVector, Vector3 rayPos, Vector3 forward)
        {
            // Activate and Play the Muzzle Flash as the gun is now firing.
            //this.muzzleFlashPS.main.simulationSpeed = 1f * (1f / Time.timeScale); // todo: use this for slomo im assuming?
            this.EnableMuzzleFlash(true);

            this.audioSource.PlayOneShot(this.fireSound);

            // todo: no point in hasProjectile then?
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
                        hit.transform.GetComponent<Rigidbody>().AddForce(this.transform.forward * 10000f * Time.deltaTime);// todo: should i be using time.deltatime here?
                }

                if (hit.transform.GetComponent<Destructable>())
                {
                    // Deal damage to the hit object (depends on the damage of the weapon).
                    hit.transform.GetComponent<Destructable>().ManipulateHealth(this.damage);
                }
            }

            // Reduce the current gun's clip by 1.
            this.SetRemainingAmmo(this.remainingAmmo - 1);
        }

        public override void OnDropped()
        {
            base.OnDropped();

            // If the weapon is dropped then firing has stopped so stop & hide the Muzzle Flash.
            this.EnableMuzzleFlash(false);
        }

        private void OnEmptyMag()
        {
            this.EnableMuzzleFlash(false);
        }

        private void SetRemainingAmmo(int ammo)
        {
            this.remainingAmmo = ammo;

            if (this.ammoText)
                this.ammoText.text = this.remainingAmmo.ToString();

            if (this.remainingAmmo <= 0)
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
