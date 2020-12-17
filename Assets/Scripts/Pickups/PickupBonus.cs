using UnityEngine;

public class PickupBonus<T> : Pickup
{
	[SerializeField] protected T amount;

	public override void OnPickup(GameObject interactor)
	{
		base.OnPickup(interactor);

		Destroy(this.gameObject);
	}
}
