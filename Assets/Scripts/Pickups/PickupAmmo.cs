using UnityEngine;

public class PickupAmmo: PickupBonus<int>
{
    public override void OnPickup(GameObject interactor)
    {
        if(interactor.GetComponent<Inventory>() != null)
            interactor.GetComponent<Inventory>().ManipulateAmmo(this.amount);
            
        base.OnPickup(interactor);
    }
}