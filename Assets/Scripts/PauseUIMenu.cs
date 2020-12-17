using UnityEngine;

public class PauseUIMenu : MonoBehaviour 
{
	[SerializeField] private GameObject pauseUIHolder = null;

	private void Update() 
	{
		if(Input.GetKeyDown(KeyCode.Escape))
			this.SetPauseState(!this.pauseUIHolder.activeSelf);
	}
	
	public void SetPauseState(bool show)
	{
		this.pauseUIHolder.SetActive(show);

		// Freeze time if showing, resume time if not showing.
		Time.timeScale = System.Convert.ToInt32(!show);

		Cursor.visible = show;
		Cursor.lockState = (show) ? CursorLockMode.Confined : CursorLockMode.Locked;
	}
}
