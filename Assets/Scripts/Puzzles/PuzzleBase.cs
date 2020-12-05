using UnityEngine;
using UnityEngine.Events;

public class PuzzleBase : MonoBehaviour
{
	[SerializeField] private UnityEvent onCompleteEvent;
	protected bool isComplete = false;
    
    protected virtual void OnComplete()
    {
        this.isComplete = true;
        this.onCompleteEvent?.Invoke();
    }
}
