using UnityEngine;

public class PickupGrenades: PickupBonus<int>
{
	public override void OnPickup(GameObject interactor)
    {
        if(interactor.GetComponent<Inventory>() != null)
            interactor.GetComponent<Inventory>().ManipulateGrenades(this.amount);

        base.OnPickup(interactor);
    }
}
