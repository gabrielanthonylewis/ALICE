using UnityEngine;

public class AINavCharge : AIMovementBase
{
    [SerializeField] private float speedMultiplier = 1.0f;

    private void Update() 
    {
        if(this.target != null)
        {
            this.transform.position = Vector3.MoveTowards(transform.position,
                target.transform.position, this.speedMultiplier * Time.deltaTime);
        }
    }
}
