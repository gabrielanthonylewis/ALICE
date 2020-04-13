using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ALICE.Utils.Animation;

namespace ALICE.Checkpoint
{
    public class CheckpointManager : MonoBehaviour
    {
        private static CheckpointManager _instance;
        public static CheckpointManager instance { get { return _instance;  } }
                
        private CheckpointData lastCheckPoint = new CheckpointData();

        [SerializeField]
        private Animator checkpointReachedAnimator = null;

        private readonly string enemyTag = "Enemy";
        private readonly string playerTag = "Player";
        private readonly string checkpointReachedAnimationParam = "ShowCheckpoint";

        private Transform player = null;
        private int currentSceneIndex = -1;
        private bool skipNextCheckpoint = false;

        /* Singleton pattern */
        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else if (_instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            else
                return;
                        
            DontDestroyOnLoad(this.gameObject);

            /* Start will only get called once as this is a singleton.
             * I need to find all the remaining checkpoints every time a level is loaded. */
            SceneManager.sceneLoaded += this.OnSceneLoaded;
        }

        /* Like Start except called every time 
         * a scene is loaded (as this is a singleton) */
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            this.AddCheckpointListeners();

            bool hasReloaded = (scene.buildIndex == this.currentSceneIndex);
            this.skipNextCheckpoint = hasReloaded;

            if (hasReloaded)
                this.LoadLastCheckpoint();
            else
            {
                // New level
                this.currentSceneIndex = scene.buildIndex;
                this.ClearLastCheckpoint();

                this.player = GameObject.FindObjectOfType<PlayerSpawnPoint>().SpawnPlayer().transform;
            }                
        }

        public void OnRestartLevel()
        {
            this.ClearLastCheckpoint();
            this.currentSceneIndex = -1;
        }

        private void AddCheckpointListeners()
        {
            // Attach OnCheckpointReached callback to every Checkpoint in the current level
            Checkpoint[] checkpoints = GameObject.FindObjectsOfType<Checkpoint>();
            foreach (Checkpoint checkpoint in checkpoints)
                checkpoint.checkpointReachedEvent.AddListener(this.OnCheckpointReached);
        }

        private void OnCheckpointReached()
        {
            // This is required to skip saving the current checkpoint when reloading.
            if(this.skipNextCheckpoint)
            {
                this.skipNextCheckpoint = false;
                return;
            }

            this.SaveLastCheckpoint();
            AnimationUtils.SetTrigger(this.checkpointReachedAnimator,
                this.checkpointReachedAnimationParam);
        }

        private void SaveLastCheckpoint()
        {
            this.lastCheckPoint = new CheckpointData
            {
                enemyPositions = this.GetEnemyPositions(),
                playerPosition = this.player.position,
                playerRotation = this.player.rotation,
                ammo = Inventory.instance.GetAmmo(),
                grenades = Inventory.instance.GetGrenades(),
                health = this.player.GetComponent<Destructable>().GetHealth(),
                slowmo = this.player.GetComponent<SlowmoController>().GetRemainingTime()
            };
        }

        private Vector3[] GetEnemyPositions()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(this.enemyTag);
            Vector3[] enemyPositions = new Vector3[enemies.Length];
            for (int i = 0; i < enemies.Length; i++)
                enemyPositions[i] = enemies[i].transform.position;

            return enemyPositions;
        }

        private void LoadLastCheckpoint()
        {
            if (this.lastCheckPoint == null)
                return;

            this.LoadEnemies();

            this.player = GameObject.FindObjectOfType<PlayerSpawnPoint>().SpawnPlayer(
                this.lastCheckPoint.playerPosition, this.lastCheckPoint.playerRotation).transform;

            this.player.GetComponent<Destructable>().SetHealth(this.lastCheckPoint.health);
            this.player.GetComponent<SlowmoController>().SetRemainingTime(this.lastCheckPoint.slowmo);

            this.LoadInventory();
        }

        private void LoadInventory()
        {
            Inventory.instance.SetAmmo(this.lastCheckPoint.ammo);
            Inventory.instance.SetGrenades(this.lastCheckPoint.grenades);
        }

        private void LoadEnemies()
        {
            // Ensure only desired amount of enemies are alive
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(this.enemyTag);
            for (int i = 0; i < (enemies.Length - this.lastCheckPoint.enemyPositions.Length); i++)
            {
                Destroy(enemies[i]);
                enemies[i] = null;
            }
            // Remove null objects
            List<GameObject> unassignedEnemies = new List<GameObject>();
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] == null)
                    continue;

                unassignedEnemies.Add(enemies[i]);
            }

            // Assign enemy positions
            for (int i = 0; i < unassignedEnemies.Count; i++)
                unassignedEnemies[i].transform.position = this.lastCheckPoint.enemyPositions[i];
        }

        private void ClearLastCheckpoint()
        {
            this.lastCheckPoint = null;
        }
    }
}