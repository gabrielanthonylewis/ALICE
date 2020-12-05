using UnityEngine;

public class PlatformButton : MonoBehaviour, IInteractable 
{
	[SerializeField] private Platform platform = null;

	private new Animation animation = null;

	private void Start()
	{
		this.animation = this.GetComponent<Animation>();
	}
	
	public void OnInteract(GameObject interactor)
	{
		this.animation?.Play(); 
		this.platform?.Activate();
	}
}
