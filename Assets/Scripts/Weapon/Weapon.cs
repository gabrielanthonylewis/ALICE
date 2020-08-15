using UnityEngine;
using UnityEngine.Events;

/*
 * Following Halo as inspiration.
 * If you melee with a Melee Weapon in Halo it just attacks.
 * If you aim with a Melee Weapon it zooms in with binoculars.
 */
namespace ALICE.Weapon
{
    public class Weapon: MonoBehaviour
    {
        [SerializeField] protected int damage = 1;
        [SerializeField] private int meleeDamage = 5;
        [SerializeField] private float meleeForce = 2.0f;
        [SerializeField] private float meleeRange = 2.0f;
        
        [HideInInspector] public UnityEvent onHitEvent = new UnityEvent();
        
        private Powerup[] powerups = {};
        private int currentPowerupIndex = -1;
        protected Animator animator = null;
        protected AudioSource audioSource;
        protected float range = 35.0f;        
        protected bool isAiming = false;

        public virtual void OnDropped() {}
        public virtual void OnFireInput(bool isDownOnce) {}
        public virtual void OnReloadInput() {}
        public virtual void OnChangeFireTypeInput() {}

        private void Awake()
        {
            this.animator = this.GetComponent<Animator>();
            this.audioSource = this.GetComponent<AudioSource>();  
            this.powerups = this.GetComponents<Powerup>();
        }
        
        public bool IsBusy()
        {
            if (this.animator == null)
                return false;

            return (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1 ||
                this.animator.IsInTransition(0));
        }

        public virtual void OnAimInput()
        {
            isAiming = !isAiming;

            this.animator.SetTrigger("ads");
        }

        public void OnMeleeInput()
        {
            this.animator.SetTrigger("melee");

            // If an object is hit then apply force and reduce it's health.
            RaycastHit hit;
            if (Physics.Raycast (this.transform.position, this.transform.forward, out hit, this.meleeRange))
            {
                hit.transform.GetComponent<Rigidbody>()?.AddForce(this.transform.forward * this.meleeForce);

                Destructable hitDestructable = hit.transform.GetComponent<Destructable>();
                if (hitDestructable != null)
                {
                    hitDestructable.ManipulateHealth (this.meleeDamage);
                    this.onHitEvent?.Invoke();
                }
            }
        }

        public void OnSwitchPowerupInput()
        {
            if(this.currentPowerupIndex >= 0)
                this.powerups[this.currentPowerupIndex].SetParticleActive(false);

            this.currentPowerupIndex++;
            if(this.currentPowerupIndex >= this.powerups.Length)
                this.currentPowerupIndex = -1;
            else
                this.powerups[this.currentPowerupIndex].SetParticleActive(true);
        }

        public virtual void StopAllActivity()
        {
            if(this.animator != null)
                this.animator.Play("idle", -1, 0.0f);
            this.isAiming = false;
        
            foreach(Powerup powerup in this.powerups)
                powerup.SetParticleActive(false);
            this.currentPowerupIndex = -1;
        }

        protected bool IsPowerupActive()
        {
            return (this.currentPowerupIndex >= 0);
        }

        protected Powerup GetActivePowerup()
        {
            return this.powerups[this.currentPowerupIndex];
        }
    }
}