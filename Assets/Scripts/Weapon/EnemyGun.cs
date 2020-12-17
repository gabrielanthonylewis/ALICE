using UnityEngine;

public class EnemyGun : Gun
{
    protected override Vector3 GetFireForwardVector()
    { 
        return this.transform.forward;
    }

    protected override Vector3 GetFireRayPosition() 
    {
        return this.transform.position;
    }
}
