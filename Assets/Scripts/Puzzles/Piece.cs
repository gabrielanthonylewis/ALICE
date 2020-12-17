using UnityEngine;

// Like Vector2 but for integer indexes.
public class GridIndex
{
    public GridIndex(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    public int row;
    public int col;
}

public class Piece : MonoBehaviour
{
    public GridIndex index { private set; get; }
    public bool isEmpty { private set; get; }
    public new Collider collider { private set; get; }
    public Material material
    { 
        private set { this.meshRenderer.material = value; }
        get { return this.meshRenderer.material; }
    }

    private MeshRenderer meshRenderer = null;
    private Material originalMaterial = null;
    private Texture originalTexture = null;

    private void Awake()
    {
        this.collider = this.GetComponent<Collider>();
        this.meshRenderer = this.GetComponent<MeshRenderer>();
    }

    public void SetGridIndex(GridIndex gridIndex)
    {
        this.index = gridIndex;
    }

    public void SetInitialTexture(Texture texture)
    {
        this.originalMaterial = this.meshRenderer.material;
        this.originalTexture = texture;

        this.SetTexture(texture);
    }

    public void SetTexture(Texture texture)
    {
        this.meshRenderer.material = this.originalMaterial;
        this.meshRenderer.material.mainTexture = texture;
        this.isEmpty = false;
    }

    public void SetEmpty(Material material)
    {
        this.meshRenderer.material = material;
        this.meshRenderer.material.mainTexture = this.originalTexture;
        this.isEmpty = true;
    }
}
