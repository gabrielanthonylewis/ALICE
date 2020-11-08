using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightControl : MonoBehaviour
{
	private new Light light = null;

    private void Start()
    {
        this.light = this.GetComponent<Light>();
    }

	private void Update()
    {
        this.light.intensity = Random.Range(4.0f, 5.0f);
	}
}
