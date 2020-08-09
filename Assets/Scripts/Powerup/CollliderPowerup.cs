using UnityEngine;

public class CollliderPowerup : Powerup
{
    [SerializeField] private float minAlpha = 0.4f;
    [SerializeField] private float reduceAlphaAmount = 0.05f;

    public override void AffectObject(Transform target)
    {
        if (target == null || target.tag != this.affectedObjectTag)
            return;

        MeshRenderer targetMeshRenderer = target.GetComponent<MeshRenderer>();
        this.ReduceAlpha(targetMeshRenderer, this.reduceAlphaAmount);

        if(targetMeshRenderer.material.color.a <= this.minAlpha)
        {
            this.DisableCollider(target.GetComponent<Collider>());
            this.PlayCompleteSound();
        }
    }

    private void ReduceAlpha(MeshRenderer meshRenderer, float amount)
    {
        if(meshRenderer == null)
            return;

        Color newColour = meshRenderer.material.color;
        newColour.a = Mathf.Max(newColour.a - amount, this.minAlpha);
        meshRenderer.material.color = newColour;
    }

    private void DisableCollider(Collider collider)
    {
        if(collider == null)
            return;

        collider.enabled = false;
    }

}
