using UnityEngine;

public class OpenGate : MonoBehaviour, IInteractable 
{
	[SerializeField] private Animation openAnimation;

	private bool isOpened = false;

	public void OnInteract(GameObject interactor)
	{
		if (!this.isOpened) 
		{
			if(this.openAnimation != null)
				this.openAnimation.Play();

			this.isOpened = true;
		}
	}
}
