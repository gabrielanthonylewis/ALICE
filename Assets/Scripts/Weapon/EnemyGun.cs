using UnityEngine;

public class EnemyGun : Gun
{
    public new AIWeaponController weaponController { protected get; set; }

    protected override Vector3 GetFireForwardVector()
    { 
        return this.transform.forward;
    }

    protected override Vector3 GetFireRayPosition() 
    {
        return this.transform.position;
    }

    public override void OnReloadInput()
    {
        this.Reload(this.magSize);
    }
}
