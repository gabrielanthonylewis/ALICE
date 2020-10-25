using UnityEngine;

// The OpenGate script plays an _Animation to open a "gate" type object when the function is called.
public class OpenGate : MonoBehaviour, IInteractable 
{
	// Open gate _Animation.
	[SerializeField] private Animation OpenGateAnim;

	// Is the gate opened?
	private bool isOpened = false;

	public void OnInteract(GameObject interactor)
	{
		// Open gate if not already opened.
		if (!isOpened) 
		{
			if(OpenGateAnim)
				OpenGateAnim.Play ();

			isOpened = true;
		}
	}
}
