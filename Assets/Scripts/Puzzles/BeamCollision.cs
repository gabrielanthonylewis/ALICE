using UnityEngine;

public class BeamCollision : MonoBehaviour 
{
	[SerializeField] private float rayDistance = 6.0f;

	private BeamTowerController hitTower = null;

	private void Update()
	{
		RaycastHit hit;

		bool hasHit = Physics.Raycast(this.transform.position, this.transform.forward,
			out hit, this.rayDistance);
		
		this.SetHitTower(hasHit ? hit.transform.GetComponent<BeamTowerController>() : null);
	}

	private void SetHitTower(BeamTowerController tower)
	{
		if(this.hitTower == tower)
			return;

		// If the tower is already powered ignore it.
		if(tower != null && tower.IsPowered())
			return;

		// When switching targets turn off the old one.
		if(this.hitTower != null && this.hitTower != tower)
			this.hitTower.SetPoweredState(false);

		this.hitTower = tower;

		if(this.hitTower != null)
			this.hitTower.SetPoweredState(true);
	}
}
