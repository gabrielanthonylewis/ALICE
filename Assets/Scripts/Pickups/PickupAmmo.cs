using UnityEngine;

public class PickupAmmo: PickupBonus<int>
{
    public override void OnPickup(GameObject interactor)
    {
		Inventory.instance.ManipulateAmmo(this.amount);
        base.OnPickup(interactor);
    }
}