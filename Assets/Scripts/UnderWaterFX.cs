using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages the underwater effect 
// dependent on the player entering/leaving the trigger zone
public class UnderWaterFX : MonoBehaviour
{
    [SerializeField] private Color _underwaterColor;

    // default settings to revert back to
    private Color _defaultColor;
    private float _defaultFogDensity;
    private bool _defaultFogState;
    

    void Start()
    {
        _defaultColor = RenderSettings.fogColor;
        _defaultFogDensity = RenderSettings.fogDensity;
        _defaultFogState = RenderSettings.fog;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
            return;

        UnderWater();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player")
            return;

        AboveWater();
    }

    void OnDisable()
    {
        AboveWater();
    }


    private void UnderWater()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = _underwaterColor;
        RenderSettings.fogDensity = 0.66f;
    }

    private void AboveWater()
    {
        RenderSettings.fogColor = _defaultColor;
        RenderSettings.fogDensity = _defaultFogDensity;
        RenderSettings.fog = _defaultFogState;
    }

 
}
