using UnityEngine;
using System.Collections;

// The DestroyOnTouch script damages objects with health when they are inside the trigger.
public class DestroyOnTouch : MonoBehaviour 
{
    // Speed at which a destructable object will lose health
    [SerializeField] private float _killSpeedMultiplier = 25.0f;


	void OnTriggerStay(Collider other)
	{
        // Only affect objects that have health...
        if (!other.GetComponent<Destructable>())
            return;

        // If the object has an _Animation component and it's not playing return.
        // (Used in the case of the boss level where damage should only be dealt when the water rises)
        if (this.GetComponent<Animation>() && !this.GetComponent<Animation>().isPlaying)
			return;

		other.gameObject.GetComponent<Destructable> ().ManipulateHealth (_killSpeedMultiplier * Time.deltaTime);
	}
}
