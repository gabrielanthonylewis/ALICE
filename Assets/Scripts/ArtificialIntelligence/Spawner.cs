using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour 
{
	[SerializeField] private GameObject entity = null;
	[SerializeField] private int quantity = 1;
	[SerializeField] private float spawnDelay = 0.0f;

	private void Start() 
	{
		this.StartCoroutine(this.SpawnRoutine());
	}
	
	private IEnumerator SpawnRoutine()
	{
		for(int i = 0; i < this.quantity; i++)
		{
			yield return new WaitForSeconds(this.spawnDelay);
		
			GameObject.Instantiate(this.entity, this.transform.position, this.transform.rotation);
		}
	}
}
