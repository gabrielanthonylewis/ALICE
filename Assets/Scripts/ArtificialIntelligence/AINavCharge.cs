using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AINavCharge : MonoBehaviour
{
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private float range = 100.0f;

    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        this.navMeshAgent = this.GetComponent<NavMeshAgent>();
    }

}
