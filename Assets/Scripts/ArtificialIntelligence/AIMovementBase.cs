using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIMovementBase : MonoBehaviour
{
    protected NavMeshAgent navMeshAgent;
    protected Transform target = null;


    private void Start()
    {
        this.navMeshAgent = this.GetComponent<NavMeshAgent>();
    }


    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
