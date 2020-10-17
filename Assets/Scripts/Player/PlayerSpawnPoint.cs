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

        GameObject player = Instantiate(this.playerPrefab, position, rotation);
        Inventory.instance.Initialise();
        
        return player;
    }
}