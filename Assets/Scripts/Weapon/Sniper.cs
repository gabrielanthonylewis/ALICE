using UnityEngine;

namespace ALICE.Weapon.Gun
{
    public class Sniper : Gun
    {
        [SerializeField] private GameObject scopeGO = null;
        [SerializeField] private GameObject bodyGO = null;


        public override void OnDropped()
        {
            base.OnDropped();

            this.scopeGO.SetActive(false);
            this.bodyGO.SetActive(true);
        }

        public override void OnAimInput()
        {
            base.OnAimInput();

            this.scopeGO.SetActive(this.isAiming);
            this.bodyGO.SetActive(!this.isAiming);
        }

        protected override Vector3 GetFireVector()
        {
            return (this.isAiming) ? Vector3.zero :
                this.GetRandomFireVector(this.hipFireOffsetRange, this.hipFireOffsetMultiplier); 
        }

        protected override Vector3 GetFireForwardVector()
        {
            return Camera.main.transform.forward;
        }

        protected override Vector3 GetFireRayPosition()
        {
            return Camera.main.transform.position;
        } 
    }
}
