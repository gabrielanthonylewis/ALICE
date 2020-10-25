using UnityEngine;

public class PickupGrenades: PickupBonus<int>
{
	public override void OnPickup(GameObject interactor)
    {
		Inventory.instance.ManipulateGrenades(this.amount);
        base.OnPickup(interactor);
    }
}
