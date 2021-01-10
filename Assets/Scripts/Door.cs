using UnityEngine;
using UnityEngine.SceneManagement;
using ALICE.Checkpoint;

public class Door : MonoBehaviour, IInteractable
{
	[SerializeField] private int levelIndex;

	public void OnInteract(GameObject interactor, bool isDownOnce)
	{
		CheckpointManager.instance.SaveLastCheckpoint(false);
		SceneManager.LoadScene(this.levelIndex);
	}
}
