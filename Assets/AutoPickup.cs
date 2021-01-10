using UnityEngine;

public class AutoPickup : MonoBehaviour
{
    [SerializeField] private int pickupLayer = 8;
    [SerializeField] private GameObject interactor = null;
    [SerializeField] private Inventory inventory = null;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer != pickupLayer)
            return;

        if(other.gameObject.GetComponent<Pickup>() == null)
            return;

        Gun gun = other.gameObject.GetComponent<Gun>(); 
        if(gun != null)
        {
            // If gun type exists then add ammo, otherwise do nothing.
            if(this.inventory.HasWeaponType(gun))
            {
                this.inventory.ManipulateAmmo(gun.GetRemainingAmmo());
                GameObject.Destroy(gun.gameObject);
            }
        }
        else
            other.gameObject.GetComponent<Pickup>().OnPickup(this.interactor);
    }
}
