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
    }
}
