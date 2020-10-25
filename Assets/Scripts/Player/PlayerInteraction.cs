using UnityEngine;

public class PlayerInteraction : MonoBehaviour 
{
	[SerializeField] private LayerMask layermask;
	[SerializeField] private float rayDistance = 4.0f;

	private void Update() 
	{
		RaycastHit hit;
		if (Physics.Raycast (this.transform.position, this.transform.forward, out hit, this.rayDistance, this.layermask))
		{
			if(Input.GetKey(KeyCode.F))
				hit.transform.GetComponent<IInteractable>()?.OnInteract(this.gameObject);
		}
	}
}
