using UnityEngine;

public class DestroyOnTouch : MonoBehaviour 
{
    [SerializeField] private float _killSpeedMultiplier = 25.0f;

	private void OnTriggerStay(Collider other)
	{
        if(other.isTrigger)
            return;

        if (!other.GetComponent<Destructable>())
            return;

        // TODO: Should probs just turn on this object in the boss mission when the boss is dead..
        // If the object has an _Animation component and it's not playing return.
        // (Used in the case of the boss level where damage should only be dealt when the water rises)
        if (this.GetComponent<Animation>() && !this.GetComponent<Animation>().isPlaying)
			return;

		other.gameObject.GetComponent<Destructable> ().ManipulateHealth (-(_killSpeedMultiplier * Time.deltaTime));
	}
}
