using UnityEngine;
using UnityEngine.Events;

/*
 * Following Halo as inspiration.
 * If you melee with a Melee Weapon in Halo it just attacks.
 * If you aim with a Melee Weapon it zooms in with binoculars.
 */
namespace ALICE.Weapon
{
    public class Weapon: Pickup
    {
        [SerializeField] protected int damage = 1;
        [SerializeField] private int meleeDamage = 5;
        [SerializeField] private float meleeForce = 2.0f;
        [SerializeField] private float meleeRange = 2.0f;
        [SerializeField] private float normalFOV = 75.0f;
        [SerializeField] private float zoomFOV = 50.0f;
        [SerializeField] protected float range = 35.0f;
        
        [HideInInspector] public UnityEvent onHitEvent = new UnityEvent();
        [HideInInspector] public UnityEvent onDroppedEvent = new UnityEvent();

        public WeaponController weaponController { protected get; set; }

        private int pickupLayer = 8;
        private int ignoreLayer = 2;

        private Powerup[] powerups = {};
        private int currentPowerupIndex = -1;
        protected Animator animator = null;
        protected AudioSource audioSource;
          
        protected bool isAiming = false;

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
            this.isAiming = !this.isAiming;

            Camera.main.fieldOfView = (this.isAiming) ? this.zoomFOV : this.normalFOV;

            this.animator.SetTrigger("ads");
        }

        public virtual void OnDropped()
        {
            // Unparent the weapon and re-enable the colliders and physics.
            this.transform.SetParent(null);
            if(this.transform.GetComponent<BoxCollider>())
                this.transform.GetComponent<BoxCollider>().enabled = true;
            if(this.transform.GetComponent<Rigidbody>())
                this.transform.GetComponent<Rigidbody>().isKinematic = false;

            // Set the weapon's children's layers to 0 (default) so that the player cannot see the weapon through objects.
            Transform[] children = this.transform.GetComponentsInChildren<Transform>();
            for (int j = 0; j < children.Length; j++)
                children[j].gameObject.layer = 0;

            Camera.main.fieldOfView = this.normalFOV;

            this.gameObject.layer = this.pickupLayer;

            this.onHitEvent.RemoveAllListeners();
            this.onDroppedEvent?.Invoke();
            this.onDroppedEvent.RemoveAllListeners();
            this.weaponController = null;
        }

        public override void OnPickup(GameObject interactor)
        {
            if(interactor.GetComponent<Inventory>() != null)
                interactor.GetComponent<Inventory>().TryAddWeapon(this);
        }

        public void OnPickedUp()
        {
            // Hide weapon (not equiped on default).
			this.gameObject.SetActive(false);

			// Turn off colliders and physics.
			if (this.transform.GetComponent<BoxCollider>())
				this.transform.GetComponent<BoxCollider>().enabled = false;
			if (this.transform.GetComponent<Rigidbody>())
				this.transform.GetComponent<Rigidbody>().isKinematic = true;

			this.name = "Gun";
			this.transform.SetParent(Camera.main.transform);
			this.transform.localRotation = Quaternion.identity;
			this.transform.localPosition = Vector3.zero;
            this.gameObject.layer = this.ignoreLayer;


			// Set the weapon's children's layers to "GunLayer" so that the gun will not clip through objects (from the player's perspective).
			Transform[] children = this.GetComponentsInChildren<Transform>();
			for (int j = 0; j < children.Length; j++)
				children[j].gameObject.layer = 10;
        }

        public void OnMeleeInput()
        {
            this.animator.SetTrigger("melee");

            /* If an object is hit then apply force and reduce it's health.
             * ~0 means every layer and triggers are ignored.*/
            RaycastHit hit;
            if (Physics.Raycast (this.transform.position, this.transform.forward, out hit, this.meleeRange, ~0, QueryTriggerInteraction.Ignore))
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
                this.powerups[this.currentPowerupIndex].SetActive(false);

            this.currentPowerupIndex++;
            if(this.currentPowerupIndex >= this.powerups.Length)
                this.currentPowerupIndex = -1;
            else
                this.powerups[this.currentPowerupIndex].SetActive(true);
        }

        public virtual void StopAllActivity()
        {
            if(this.animator != null)
                this.animator.Play("idle", -1, 0.0f);
            this.isAiming = false;
        
            foreach(Powerup powerup in this.powerups)
                powerup.SetActive(false);
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