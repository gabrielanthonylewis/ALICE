using UnityEngine;

public class PlayerInteraction : MonoBehaviour 
{
	[SerializeField] private LayerMask layermask;
	[SerializeField] private float rayDistance = 4.0f;
	[SerializeField] private GameObject interactPrompt;

	private void Update() 
	{
		RaycastHit hit;
		bool hasHit = Physics.Raycast(this.transform.position, this.transform.forward,
			out hit, this.rayDistance, this.layermask); 
		
		this.interactPrompt.SetActive(hasHit);
				
		if(hasHit && Input.GetKey(KeyCode.F))
		{
			bool isDownOnce = Input.GetKeyDown(KeyCode.F);
			hit.transform.GetComponent<IInteractable>()?.OnInteract(this.gameObject, isDownOnce);
		}
	}
}
