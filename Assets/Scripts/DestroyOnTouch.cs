using UnityEngine;
using System.Collections.Generic;

public class DestroyOnTouch : MonoBehaviour 
{
    [SerializeField] private float _killSpeedMultiplier = 25.0f;

    private List<Destructable> touched = new List<Destructable>();

    private void Update()
    {
        foreach(Destructable destructable in this.touched)
		    destructable.ManipulateHealth(-(_killSpeedMultiplier * Time.deltaTime));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.isTrigger)
            return;

        if (!other.GetComponent<Destructable>())
            return;

        this.touched.Add(other.GetComponent<Destructable>());
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.isTrigger)
            return;

        if (!other.GetComponent<Destructable>())
            return;

        this.touched.Remove(other.GetComponent<Destructable>());
    }
}
