using UnityEngine;

/**
 * Shrink the target up until the specified minimum size.
 */
public class ShrinkPowerup : Powerup
{
    [SerializeField] private Vector3 minSize = new Vector3(0.2f, 0.2f, 0.2f);
    [SerializeField] private float shrinkMultiplier = 0.9f; 

    protected override bool AffectObject(Transform target)
    {
        target.localScale = Vector3.Max(target.localScale * this.shrinkMultiplier, this.minSize);

        if(target.localScale.magnitude <= this.minSize.magnitude)
            this.OnActionComplete();

        return true;
    }

}
