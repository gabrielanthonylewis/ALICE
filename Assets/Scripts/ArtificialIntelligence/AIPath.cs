using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIPath : MonoBehaviour
{
	[SerializeField] private Transform pointA = null;
	[SerializeField] private Transform pointB = null;

    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        this.navMeshAgent = this.GetComponent<NavMeshAgent>();
        this.navMeshAgent.destination = this.pointA.position;
    }

    private void Update()
    {
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
