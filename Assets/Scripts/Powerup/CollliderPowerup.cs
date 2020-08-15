using UnityEngine;

public class CollliderPowerup : Powerup
{
    [SerializeField] private float minAlpha = 0.4f;
    [SerializeField] private float reduceAlphaAmount = 0.05f;

    public override bool AffectObject(Transform target)
    {
        if (target == null || target.tag != this.affectedObjectTag)
            return false;

        MeshRenderer targetMeshRenderer = target.GetComponent<MeshRenderer>();
        if(this.ReduceAlpha(targetMeshRenderer, this.reduceAlphaAmount))
        {
            if(targetMeshRenderer.material.color.a <= this.minAlpha)
            {
                this.DisableCollider(target.GetComponent<Collider>());               
                this.PlayCompleteSound(); 
            }

            return true;
        }

        return false;
    }

    private bool ReduceAlpha(MeshRenderer meshRenderer, float amount)
    {
        if(meshRenderer == null)
            return false;

        Color newColour = meshRenderer.material.color;
        newColour.a = Mathf.Max(newColour.a - amount, this.minAlpha);
        meshRenderer.material.color = newColour;
    
        return true;
    }

    private void DisableCollider(Collider collider)
    {
        if(collider == null)
            return;

        collider.enabled = false;
    }

}
