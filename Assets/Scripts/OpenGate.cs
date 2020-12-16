using UnityEngine;

public class OpenGate : MonoBehaviour, IInteractable 
{
	[SerializeField] private Animation openAnimation;

	private bool isOpened = false;

	public void OnInteract(GameObject interactor, bool isDownOnce)
	{
		this.Open();
	}

	public void Open()
	{
		if (!this.isOpened) 
		{
			if(this.openAnimation != null)
				this.openAnimation.Play();

			this.isOpened = true;
		}
	}
}
