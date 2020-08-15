using UnityEngine;

namespace ALICE.Weapon.Gun
{
    public class Sniper : Gun
    {
        [SerializeField] private GameObject scopeGO = null;


        public override void OnDropped()
        {
            base.OnDropped();

            this.scopeGO.SetActive(false);
        }

        public override void OnAimInput()
        {
            base.OnAimInput();

            this.scopeGO.SetActive(this.isAiming);
        }

        protected override Vector3 GetFireVector()
        {
            return (this.isAiming) ? Vector3.zero :
                this.GetRandomFireVector(this.hipFireOffsetRange, this.hipFireOffsetMultiplier); 
        }

        protected override Vector3 GetFireForwardVector()
        {
            return (this.isAiming) ? this.scopeGO.transform.forward : Camera.main.transform.forward;
        }

        protected override Vector3 GetFireRayPosition()
        {
            return (this.isAiming) ? this.scopeGO.transform.position : Camera.main.transform.position;
        } 
    }
}
