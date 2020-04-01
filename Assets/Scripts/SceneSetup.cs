using UnityEngine;
using System.Collections;

// The SceneSetup script instantiates all of the assigned objects.
public class SceneSetup : MonoBehaviour 
{
	// Player prefab (needs position)
	[SerializeField] private GameObject _playerPrefab;

	// Spawn position of player
	[SerializeField] private Transform _playerSpawn;

	// Other objects to be instantiated (do not need position)
	[SerializeField] private GameObject[] _otherToSpawn;

	// If true the GameObject is destroyed after the Setup is complete.
	[SerializeField] private bool _destroyOnSetup = true;


	void Start ()
	{
		Setup ();
	}

	public void Setup()
	{
		// Instantiated all of the objects withinthe _otherToSpawn array.
		for (int i = 0; i < _otherToSpawn.Length; i++)
		{
			if(_otherToSpawn[i] == null)
				continue;

			GameObject obj = Instantiate(_otherToSpawn[i], this.transform.position, this.transform.rotation) as GameObject;
			obj.name = _otherToSpawn[i].name; // To avoid "(clone)" in the name.
		}	

		if (_destroyOnSetup)
			Destroy (this.gameObject);
	}

}
