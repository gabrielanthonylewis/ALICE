using UnityEngine;

namespace ALICE.Weapon.Gun
{
    public class Sniper : Gun
    {
        [SerializeField] private GameObject scopeGO = null;


        public override void OnDropped()
        {
            base.OnDropped();

            // Stop looking down the scope.
            this.scopeGO.SetActive(false);
        }

        protected override Vector3 GetFireVector()
        {
            Vector3 randomVector = Vector3.zero;

            if (this.isAiming)
            {
                randomVector = new Vector3(Random.Range(-0.05f, 0.05f),
                    Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f)) * 10.0f;
            }
            
            return randomVector; 
        }

        protected override Vector3 GetFireForwardVector()
        {
            return (this.isAiming) ? this.scopeGO.transform.forward : Camera.main.transform.forward;
        }

        protected override Vector3 GetFireRayPosition()
        {
            return (this.isAiming) ? this.scopeGO.transform.position : Camera.main.transform.position;
        }

        protected override float GetFireDelay()
        {
            return 1.0f;
        }      
    }
}
