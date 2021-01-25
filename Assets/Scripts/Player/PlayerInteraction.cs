using UnityEngine;

public class PlayerInteraction : MonoBehaviour 
{
	[SerializeField] private LayerMask layermask;
	[SerializeField] private float rayDistance = 4.0f;
	[SerializeField] private GameObject interactPrompt;

	private void Update() 
	{
		// Checks if an interactable object has been hit.
		RaycastHit hit;
		bool hasRayHit = Physics.Raycast(this.transform.position, this.transform.forward,
			out hit, this.rayDistance, ~0, QueryTriggerInteraction.Ignore);
		bool hasInteractableLayer = (hasRayHit && (this.layermask == (layermask |
			(1 << hit.transform.gameObject.layer)))); 
		bool hasHit = (hasRayHit && hasInteractableLayer);

		this.interactPrompt.SetActive(hasHit);
				
		if(hasHit && Input.GetKey(KeyCode.F))
		{
			bool isDownOnce = Input.GetKeyDown(KeyCode.F);
			hit.transform.GetComponent<IInteractable>().OnInteract(this.gameObject, isDownOnce);
		}
	}
}
