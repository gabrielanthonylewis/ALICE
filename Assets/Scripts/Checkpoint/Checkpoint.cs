using UnityEngine;
using UnityEngine.Events;

namespace ALICE.Checkpoint
{   
    [RequireComponent(typeof(BoxCollider))]
    public class Checkpoint : MonoBehaviour
    {
        [HideInInspector]
        public UnityEvent checkpointReachedEvent = new UnityEvent();

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag != "Player")
                return;

            this.CheckpointReached();
        }

        private void CheckpointReached()
        {
            if (this.checkpointReachedEvent != null)
                this.checkpointReachedEvent.Invoke();

            Destroy(this.gameObject);
        }
    }
}
