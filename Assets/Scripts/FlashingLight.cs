using UnityEngine;

[RequireComponent(typeof (Light))]
public class FlashingLight : MonoBehaviour 
{
	[SerializeField] private float duration = 1.0f;
	[SerializeField] private float minIntensity = 0.0f;
	[SerializeField] private float minRange = 10.0f;

	private new Light light = null;
	private float maxIntensity;
	private float maxRange;
	private float targetIntensity;
	private float targetRange;
	private float lerpT;
	private float halfDuration;

	private void Start()
	{
		this.light = this.GetComponent<Light>();
		this.maxIntensity = this.light.intensity;
		this.maxRange = this.light.range;
		this.targetIntensity = this.maxIntensity;
		this.targetRange = this.maxRange;
		this.lerpT = Random.Range(0.0f, 1.0f);
		this.halfDuration = this.duration / 2.0f;
	}

	private void Update()
	{
		// Lerp intensity and range.
		this.lerpT = Mathf.Min(this.lerpT + (Time.deltaTime / this.halfDuration), 1.0f);
		this.light.intensity = Mathf.Lerp(this.light.intensity, this.targetIntensity, this.lerpT);
		this.light.range = Mathf.Lerp(this.light.range, this.targetRange, this.lerpT);

		// If finished lerping then switch targets.
		if(this.lerpT == 1.0f)
		{
			this.targetIntensity = (this.targetIntensity == this.maxIntensity)
				? this.minIntensity : this.maxIntensity;
		
			this.targetRange = (this.targetRange == this.maxRange)
				? this.minRange : this.maxRange;
		
			this.lerpT = 0.0f;
		}
	}

}
