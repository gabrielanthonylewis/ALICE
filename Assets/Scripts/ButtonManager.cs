using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using ALICE.Checkpoint;

// The ButtonManager script provides many different functions that will be used upon button press.
// Examples including reloading the level, loading a level and quitting the game.
public class ButtonManager : MonoBehaviour 
{
    public void LoadLevel(int levelNo)
	{
		// The Inventory is static throught levels/rooms so only reset when on the main menu.
		if (levelNo == 0)
		{
			// Reset Inventory to inital state.
			Inventory.instance.Initialise ();
		}

		Time.timeScale = 1f;

        // Load level "levelNo", e.g. level 0 = the Main Menu.
        SceneManager.LoadScene(levelNo);
	}
	
    public void LoadLastCheckpoint()
    {
        CheckpointManager.instance.LoadWhenSceneLoaded();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1.0f;
    }

	public void ReloadLevel()
	{
        CheckpointManager.instance.ClearLastCheckpoint();

        // Reload the current level.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // Reset Inventory to inital state.
        Inventory.instance.Initialise();

        Time.timeScale = 1.0f;
    }

	public void Show(GameObject obj)
	{
		obj.SetActive (true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

	public void Hide(GameObject obj)
	{
		obj.SetActive (false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
	

	public void QuitApplication()
	{
		// Exit the game/application.
		Application.Quit();
	}
}
