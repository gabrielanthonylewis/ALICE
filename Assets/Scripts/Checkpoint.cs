using UnityEngine;
using UnityEngine.Events;

namespace ALICE.Checkpoint
{
    public class CheckpointReachedEvent : UnityEvent<int> { }
    
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField]
        private int currentCheckpoint = 0;

        private CheckpointReachedEvent checkpointReachedEvent = new CheckpointReachedEvent();

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag != "Player")
                return;

            this.CheckpointReached();
        }

        private void CheckpointReached()
        {
            Debug.Log("CheckpointReached");

            this.checkpointReachedEvent.Invoke(this.currentCheckpoint);

            Destroy(this.gameObject);
        }

        public void AddListener(UnityAction<int> callback)
        {
            this.checkpointReachedEvent.AddListener(callback);
        }
    }
}
