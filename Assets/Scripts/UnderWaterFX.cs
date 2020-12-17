using UnityEngine;

// Manages the underwater effect when entering/leaving the trigger collider.
public class UnderWaterFX : MonoBehaviour
{
    [SerializeField] private Color underwaterFogColour;
    [SerializeField] private float underwaterFogDensity = 0.66f;
    [SerializeField] private string targetTag = "Player";

    private Color defaultFogColour;
    private float defaultFogDensity;
    private bool defaultFogVisibility;
    
    private void Start()
    {
        this.defaultFogColour = RenderSettings.fogColor;
        this.defaultFogDensity = RenderSettings.fogDensity;
        this.defaultFogVisibility = RenderSettings.fog;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != this.targetTag)
            return;

        this.SetFXState(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != this.targetTag)
            return;

        this.SetFXState(false);
    }

    private void SetFXState(bool state)
    {
        RenderSettings.fogColor = (state) ? this.underwaterFogColour : this.defaultFogColour;
        RenderSettings.fogDensity = (state) ? this.underwaterFogDensity : this.defaultFogDensity;
        RenderSettings.fog = (state) ? true : this.defaultFogVisibility;
    }

    private void OnDisable()
    {
        this.SetFXState(false);
    }
 
}
