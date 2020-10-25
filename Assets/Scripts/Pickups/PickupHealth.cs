using UnityEngine;

public class PickupHealth: PickupBonus<int>
{
	public override void OnPickup(GameObject interactor)
    {
		Destructable interactorsHealth = interactor.GetComponentInParent<Destructable>();
		if(interactorsHealth != null && interactorsHealth.ManipulateHealth(-this.amount))
			base.OnPickup(interactor);
    }
}
