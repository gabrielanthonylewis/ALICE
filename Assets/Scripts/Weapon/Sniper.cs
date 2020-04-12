using UnityEngine;

namespace ALICE.Weapon.Gun
{
    public class Sniper : Gun
    {
        [SerializeField] private GameObject scopeGO = null;


        public override void OnDropped()
        {
            base.OnDropped();

            // If the weapon is a Sniper and scoped then stop looking down the scope.
            this.scopeGO.SetActive(false);
        }
    }
}
