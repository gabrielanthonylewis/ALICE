using UnityEngine;
using UnityEngine.Events;

// The CinematicCameraEvent script is to be soley used by an Animation event. 
public class CinematicCameraEvent : MonoBehaviour
{
	[SerializeField] private UnityEvent events = null;

	public void InvokeFunction()
	{
		this.events.Invoke ();
		
        GameObject.Destroy(this.gameObject);
	}
}