using UnityEngine;

public interface IInteractable
{
    void OnInteract(GameObject interactor, bool isDownOnce = false);
}
