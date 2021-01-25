using UnityEngine;
using UnityEngine.Events;

// The CinematicCameraEvent script is to be soley used by an Animation event. 
public class CinematicCameraEvent : MonoBehaviour
{
	public UnityEvent onCinematicFinished = new UnityEvent();

	private void Update()
	{
		if(Input.GetKeyUp(KeyCode.Escape))
			this.InvokeFunction();
	}

	public void InvokeFunction()
	{
		this.onCinematicFinished.Invoke();
		
        GameObject.Destroy(this.gameObject);
	}
}