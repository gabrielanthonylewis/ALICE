using UnityEngine;

public class MouseSettings : MonoBehaviour 
{
	[SerializeField] private bool visibility = true;
	[SerializeField] private CursorLockMode lockState = CursorLockMode.Locked;
	
	private void Awake() 
	{
		Cursor.visible = this.visibility;
		Cursor.lockState = this.lockState;
	}
}
