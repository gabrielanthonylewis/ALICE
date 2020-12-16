using UnityEngine;

public interface IPickup : IInteractable
{
	void OnPickup(GameObject interactor);
}

public class Pickup: MonoBehaviour, IPickup
{
	[SerializeField] private AudioClip pickupSound;

	public void OnInteract(GameObject interactor, bool isDownOnce)
	{
		this.OnPickup(interactor);
	}

	public virtual void OnPickup(GameObject interactor)
	{
		Camera.main.GetComponent<AudioSource>().PlayOneShot(this.pickupSound);
	}
}