using UnityEngine;
using UnityEngine.Events;

namespace ALICE.Weapon
{
    // The Weapon script contains weapon specific funtionallity such as reloading and changing the fire type.
    // It also stores the information such as the damage of the bullets and the current bullets in the clip.
    public class Weapon: MonoBehaviour
    {
        [SerializeField] protected int damage = 1;
        protected Animator animator = null;
        protected float range = 35.0f;

        [HideInInspector] public UnityEvent onHitEvent = new UnityEvent();

        public virtual void OnDropped() {}
        public virtual void OnFireInput(bool isDownOnce) {}
        public virtual void OnReloadInput() {}
        public virtual void OnAimInput() {}
        public virtual void OnChangeFireTypeInput() {}
        public virtual void OnSwitchPowerupInput() {}
        public virtual void OnMeleeInput() {}

        private void Awake()
        {
            this.animator = this.GetComponent<Animator>();    
        }
        
        public bool IsBusy()
        {
            if (this.animator == null)
                return false;

            return (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1 
                || this.animator.IsInTransition(0));
        }
    }
}