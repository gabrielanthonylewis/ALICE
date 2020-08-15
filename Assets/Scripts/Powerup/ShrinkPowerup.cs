using UnityEngine;

public class ShrinkPowerup : Powerup
{
    [SerializeField] private Vector3 minSize = new Vector3(0.2f, 0.2f, 0.2f);
    [SerializeField] private float shrinkMultiplier = 0.9f; 

    public override bool AffectObject(Transform target)
    {
        if (target == null || target.tag != this.affectedObjectTag)
            return false;

        if(this.Shrink(target))
        {
            if(target.localScale.magnitude <= this.minSize.magnitude)
                this.PlayCompleteSound();

            return true;
        }

        return false;
    }
              
    private bool Shrink(Transform target)
    {
        if(target == null)
            return false;

        target.localScale = Vector3.Max(target.localScale * this.shrinkMultiplier, this.minSize);

        return true;
    }

}
