using UnityEngine;

public class AutoPickup : MonoBehaviour
{
    [SerializeField] private int pickupLayer = 8;
    [SerializeField] private Inventory inventory = null;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer != pickupLayer)
            return;

        Pickup pickup = other.gameObject.GetComponent<Pickup>();
        if(pickup != null)
        {
            if(other.gameObject.GetComponent<Gun>())
                other.gameObject.GetComponent<Gun>().TryTakeAmmo(this.inventory.gameObject);
            else
                pickup.OnPickup(this.inventory.gameObject);
        }        
    }
}
