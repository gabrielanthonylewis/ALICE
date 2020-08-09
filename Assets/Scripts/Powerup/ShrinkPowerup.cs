using UnityEngine;

public class ShrinkPowerup : Powerup
{
    [SerializeField] private Vector3 minSize = new Vector3(0.2f, 0.2f, 0.2f);
    [SerializeField] private float shrinkMultiplier = 0.9f; 

    public override void AffectObject(Transform target)
    {
        if (target == null || target.tag != this.affectedObjectTag)
            return;

        this.Shrink(target);

        if(target.localScale.magnitude <= this.minSize.magnitude)
            this.PlayCompleteSound();
    }
              
    private void Shrink(Transform target)
    {
        if(target == null)
            return;

        target.localScale = Vector3.Max(target.localScale * this.shrinkMultiplier, this.minSize);
    }

}
