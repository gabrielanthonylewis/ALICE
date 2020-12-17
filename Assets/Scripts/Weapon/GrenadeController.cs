using UnityEngine;

public class GrenadeController : MonoBehaviour
{
    [SerializeField] private GameObject grenadePrefab = null;

    private Inventory inventory;
    private GameObject tempGrenade = null;
    private float initialThrowStrength = 250.0f;
    private readonly float maxThrowStrength = 500.0f;
    private readonly float strengthMultiplier = 50.0f;
    private float currentThrowStrength;

    private void Start()
    {
        this.inventory = this.GetComponent<Inventory>();
    }

    private void Update()
    {
        if(Time.timeScale == 0) 
           return;

        this.HandleGrenadeInput();
    }

    private void HandleGrenadeInput()
    {
        if(this.inventory.GetGrenades() <= 0)
            return;

        if(Input.GetKeyDown(KeyCode.G)) 
            this.PrepareGrenade();         
        
        if(Input.GetKey(KeyCode.G))
            this.HoldGrenade();
        else if(Input.GetKeyUp(KeyCode.G))
            this.ThrowGrenade();
    }

    private void PrepareGrenade()
    {
        this.currentThrowStrength = this.initialThrowStrength;

        this.tempGrenade = Instantiate(this.grenadePrefab, this.transform.position +
            this.transform.forward, this.grenadePrefab.transform.rotation) as GameObject;

        this.tempGrenade.GetComponent<Rigidbody>().useGravity = false;
        this.tempGrenade.transform.GetChild(0).GetComponent<Collider>().enabled = false;
    }

    private void HoldGrenade()
    {
        if(this.tempGrenade == null)
            return;
        
        Vector3 positionOffset = this.transform.forward / 1.6f;
        this.tempGrenade.transform.position = this.transform.position + positionOffset;

        this.currentThrowStrength = Mathf.Min(this.currentThrowStrength +
            this.strengthMultiplier * Time.deltaTime, this.maxThrowStrength);
    }

    private void ThrowGrenade()
    {
        if(this.tempGrenade == null)
            return;

        this.tempGrenade.transform.GetChild(0).GetComponent<Collider>().enabled = true;
        this.tempGrenade.GetComponent<Rigidbody>().useGravity = true;
        this.tempGrenade.GetComponent<Rigidbody>().AddForce(this.transform.forward *
            this.currentThrowStrength, ForceMode.Force);

        this.inventory.ManipulateGrenades(-1);
        this.tempGrenade = null;
        this.currentThrowStrength = this.initialThrowStrength;
    }
}
