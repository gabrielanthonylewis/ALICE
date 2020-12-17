using UnityEngine;

public class Elevator : Platform
{
	[SerializeField] private GameObject door = null;

    protected override void OnReachedTarget()
    {
        base.OnReachedTarget();

        if(this.door != null)
            this.door.SetActive(false);
    }
}
