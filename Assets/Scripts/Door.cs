using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour, IInteractable
{
	[SerializeField] private int levelIndex;

	public void OnInteract(GameObject interactor, bool isDownOnce)
	{
		SceneManager.LoadScene(this.levelIndex);
	}
}
