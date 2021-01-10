using UnityEngine;

public class PlatformButton : MonoBehaviour, IInteractable 
{
	[SerializeField] private Platform platform = null;

	private new Animation animation = null;

	private void Start()
	{
		this.animation = this.GetComponent<Animation>();
	}
	
	public void OnInteract(GameObject interactor, bool isDownOnce)
	{
		if(this.animation != null)
			this.animation.Play();
			 
		if(this.platform != null)
			this.platform.Activate();
	}
}
