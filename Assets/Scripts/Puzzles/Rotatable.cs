using UnityEngine;

public class Rotatable : MonoBehaviour, IInteractable
{
	[SerializeField] private Vector3 rotationAxis = Vector3.up;
	[SerializeField] private float rotationMultiplier = 50.0f;

	public void OnInteract(GameObject interactor, bool isDownOnce)
	{
		this.transform.RotateAround(this.transform.position, this.rotationAxis,
			this.rotationMultiplier * Time.deltaTime);
	}
}
