using UnityEngine;
using UnityEngine.Events;

public class BeamTowerController : MonoBehaviour
{
	[SerializeField] private ParticleSystem beam = null;
	[SerializeField] private UnityEvent onPowered = new UnityEvent();

	private bool isPowered = false;

	private void Start()
	{
		this.isPowered = (this.beam != null && this.beam.gameObject.activeSelf);
	}

	public bool IsPowered()
	{
		return this.isPowered;
	}

	public void SetPoweredState(bool isPowered)
	{
		this.isPowered = isPowered;

		if(isPowered == true)
			this.onPowered.Invoke();

		this.SetBeamVisibility(isPowered);
	}

	private void SetBeamVisibility(bool isPowered)
	{
		if(this.beam == null)
			return;

		this.beam.gameObject.SetActive(isPowered);
	}
}
