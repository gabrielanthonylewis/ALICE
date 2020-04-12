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

        private int remainingAmmo = 0;


        private void Start()
        {
            // Current Clip fully loaded.
            this.SetRemainingAmmo(this.magSize);
        }

        public override void OnFireInput()
        {
            base.OnFireInput();

            Debug.Log("FIRE");
        }

        public override void OnDropped()
        {
            base.OnDropped();

            // If the weapon is dropped then firing has stopped so stop & hide the Muzzle Flash.
            this.EnableMuzzleFlash(false);
        }

        private void SetRemainingAmmo(int ammo)
        {
            this.remainingAmmo = ammo;

            if (this.ammoText)
                this.ammoText.text = this.remainingAmmo.ToString();
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
