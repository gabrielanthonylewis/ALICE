using UnityEngine;

public class AIPath : AIMovementBase
{
	[SerializeField] private Transform pointA = null;
	[SerializeField] private Transform pointB = null;


    private void Start()
    {
        this.navMeshAgent.destination = this.pointA.position;
    }

    private void Update()
    {
        if(this.target != null)
            return;
            
        // When a point has been reached go to the next.
        if(!this.navMeshAgent.pathPending && this.navMeshAgent.remainingDistance < 0.5f)
            this.GoToNextPoint();
    }


    private void GoToNextPoint()
    {
        this.navMeshAgent.destination = (this.navMeshAgent.destination == this.pointA.position) ? 
            this.pointB.position : this.pointA.position; 
    }
}
