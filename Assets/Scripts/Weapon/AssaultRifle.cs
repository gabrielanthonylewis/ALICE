using UnityEngine;
using System.Collections;

namespace ALICE.Weapon.Gun
{
    public enum PowerUp
    {
        NULL, Shrink, Transparency
    };

    public class AssaultRifle : Gun
    {
        // todo: Move to weapon as all weapons should allow the powerup?
        [SerializeField] private ParticleSystem shrinkPS = null;
        [SerializeField] private ParticleSystem transparencyPS = null;
        [SerializeField] private AudioClip powerupSound = null;
        private PowerUp powerup = PowerUp.NULL;

        protected override Vector3 GetFireVector()
        {
            Vector3 randomVector = Vector3.zero;

            if (!this.isAiming)
                randomVector = new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f));

            return randomVector;
        }

        protected override Vector3 GetFireForwardVector()
        {
            return Camera.main.transform.forward;
        }

        protected override Vector3 GetFireRayPosition()
        {
            return Camera.main.transform.position;
        }

        protected override float GetFireDelay()
        {
            return 0.1f;
        }
    }
}
