using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour 
{
	[SerializeField] private GameObject playerPrefab = null;
	
    public GameObject SpawnPlayer()
    {
        return this.SpawnPlayer(this.transform.position, this.transform.rotation);
    }
    
    public GameObject SpawnPlayer(Vector3 position, Quaternion rotation)
    {
        if (this.playerPrefab == null)
        {
            Debug.LogError("PlayerSpawnPoint - PlayerPrefab not assigned");
            return null;
        }

        return Instantiate(this.playerPrefab, position, rotation);
    }
}