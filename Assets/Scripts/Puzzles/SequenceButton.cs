using UnityEngine;
using System.Collections;

public class SequenceButton : MonoBehaviour, IInteractable 
{
	[SerializeField] private SequenceController sequenceController;
	[SerializeField] private Color flashColour;

	private MeshRenderer meshRenderer;
	private Color defaultColour;
	private bool isCoroutineRunning = false;

	private void Awake()
	{
		this.meshRenderer = this.GetComponent<MeshRenderer>();
		this.defaultColour = this.meshRenderer.material.color;
	}

	public void OnInteract(GameObject interactor)
	{
		if(this.CanAddAttempt())
		{
			this.Flash();
			this.sequenceController.AddButtonAttempt(this);
		}
	}

	private bool CanAddAttempt()
	{
		return (this.sequenceController.CanAddAttempt() && !this.isCoroutineRunning);
	}

	public void Flash()
	{
		this.StartCoroutine(this.FlashRou());
	}

	private IEnumerator FlashRou()
	{
		this.isCoroutineRunning = true;

		this.meshRenderer.material.color = this.flashColour;
		yield return new WaitForSeconds(0.4f);
		this.meshRenderer.material.color = this.defaultColour;

		this.isCoroutineRunning = false;
	}
}
