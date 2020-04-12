using UnityEngine;

namespace ALICE.Weapon.Gun
{
    // todo: do i actually need this?
    public enum WeaponType
    {
        NULL, AssaultRifle, Shotgun, Pistol, Sniper
    };
       
    public class Gun : Weapon
    {
        private enum FireType
        {
            NULL, Single, Auto, Burst, Sniper
        };

        [SerializeField] private int clipSize = 30; // magSize
        [SerializeField] private ParticleSystem MuzzleFlash = null;
        [SerializeField] private GameObject muzzleflashgo = null;
        [SerializeField] private TextMesh ClipDisplayText = null;
        [SerializeField] private WeaponType _WeaponType = WeaponType.NULL;
        [SerializeField] private FireType _FireType = FireType.NULL;
        [SerializeField] private bool useRayBullet = true; // hasProjectileAmmo

        private int currentClip = 0; // remainingAmmo

        public override void OnDropped()
        {
            base.OnDropped();

            // If the weapon is dropped then firing has stopped so stop & hide the Muzzle Flash.
            MuzzleFlash.Stop();
            muzzleflashgo.SetActive(false);
        }
    }
}
