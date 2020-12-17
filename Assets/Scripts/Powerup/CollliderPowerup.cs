using UnityEngine;

/**
 * Will reduce the alpha of an object to a point.
 * This will then disable the collider of the object.
 */
public class CollliderPowerup : Powerup
{
    [SerializeField] private float minAlpha = 0.4f;
    [SerializeField] private float reduceAlphaAmount = 0.05f;

    protected override bool AffectObject(Transform target)
    {
        // Reduce target material's alpha. 
        MeshRenderer targetMeshRenderer = target.GetComponent<MeshRenderer>();
        if(targetMeshRenderer != null)
        {
            Color newColour = targetMeshRenderer.material.color;
            newColour.a = Mathf.Max(newColour.a - this.reduceAlphaAmount, this.minAlpha);
            targetMeshRenderer.material.color = newColour;

            if(targetMeshRenderer.material.color.a <= this.minAlpha)
                this.OnActionComplete(target);       

            return true;
        }

        return false;
    }

    protected override void OnActionComplete(Transform target)
    {
        base.OnActionComplete();

        // Disable target's collider.
        Collider targetCollider = target.GetComponent<Collider>();
        if(targetCollider != null)
            targetCollider.enabled = false;
    }

}
