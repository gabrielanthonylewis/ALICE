using UnityEngine;

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

    }
}
