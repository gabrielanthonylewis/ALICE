using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour 
{
	[SerializeField] private GameObject playerPrefab = null;
	
    public void SpawnPlayer()
    {
        this.SpawnPlayer(this.transform.position, this.transform.rotation);
    }
    
    public void SpawnPlayer(Vector3 position, Quaternion rotation)
    {
        if (this.playerPrefab == null)
        {
            Debug.LogError("PlayerSpawnPoint - PlayerPrefab not assigned");
            return;
        }

        GameObject player = Instantiate(this.playerPrefab, position, rotation);
    }
}