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
        [SerializeField] private ParticleSystem _ShrinkPS = null;
        [SerializeField] private ParticleSystem _TransparencyPS = null;
        [SerializeField] private AudioClip _PowerUpSound = null;
        private PowerUp _PowerUp = PowerUp.NULL;

    }
}
