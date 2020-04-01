using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int _currentCheckpoint = 0;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
            return;

        CheckpointManager.instance.CheckpointReached(_currentCheckpoint);

       // Destroy(this.gameObject);
    }
    
}
