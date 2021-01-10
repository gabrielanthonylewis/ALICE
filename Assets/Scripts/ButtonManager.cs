using UnityEngine;
using UnityEngine.SceneManagement;
using ALICE.Checkpoint;

// Provides many different functions that will be used upon a UI button press.
public class ButtonManager : MonoBehaviour 
{
	[SerializeField] private GameObject checkpointButton = null;

	private void OnEnable()
	{
		if(this.checkpointButton != null)
			this.checkpointButton.SetActive(CheckpointManager.instance.HasCheckpoint());
	}

    public void LoadLevel(int levelIndex)
	{
		Time.timeScale = 1.0f;
        SceneManager.LoadScene(levelIndex);
	}
	
    public void LoadLastCheckpoint()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

	public void ReloadLevel()
	{
        CheckpointManager.instance.OnRestartLevel();
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

	public void Show(GameObject obj)
	{
		obj.SetActive(true);
    }

	public void Hide(GameObject obj)
	{
		obj.SetActive(false);
    }
	
	public void QuitApplication()
	{
		Application.Quit();
	}
}
