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

        private Transform player = null;
        private int currentSceneIndex = -1;

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
            this.player = GameObject.FindGameObjectWithTag("Player").transform;
            if (this.player == null)
                Debug.LogError("Player not found");

            // New level
            if (scene.buildIndex != this.currentSceneIndex)
            {
                this.currentSceneIndex = scene.buildIndex;
                
                // Reset data
                this.ClearLastCheckpoint();
                this.Initialise();
            }
            // Same level so load last checkpoint
            else
                this.LoadLastCheckpoint();
        }

        public void OnRestartLevel()
        {
            this.ClearLastCheckpoint();
            this.currentSceneIndex = -1;
        }

        private void Initialise()
        {
            // todo: Second checkpoint deletes but the callback isnt called.
            // Attach OnCheckpointReached callback to every Checkpoint in the current level
            Checkpoint[] checkpoints = GameObject.FindObjectsOfType<Checkpoint>();
            foreach (Checkpoint checkpoint in checkpoints)
                checkpoint.checkpointReachedEvent.AddListener(this.OnCheckpointReached);
        }

        private void OnCheckpointReached()
        {
            this.SaveLastCheckpoint();
            AnimationUtils.SetTrigger(this.checkpointReachedAnimator, "ShowCheckpoint");
        }

        private void SaveLastCheckpoint()
        {
            this.lastCheckPoint = new CheckpointData
            {
                enemyPositions = this.GetEnemyPositions(),
                playerPosition = this.player.position,
                playerRotation = this.player.rotation
            };
        }

        private Vector3[] GetEnemyPositions()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
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
            this.LoadPlayer();
        }

        private void LoadPlayer()
        {
            // Assign player position
            this.player.position = this.lastCheckPoint.playerPosition;
            this.player.rotation = this.lastCheckPoint.playerRotation;
            // todo: call sceneSetup.SpawnPlayer(position, rotation) ?? this means I need to call this if not got a checkpoint though.
            // unless if not got last checkpoint then do SpawnPlayer() and it will use spawnpoint
        }

        private void LoadEnemies()
        {
            // Ensure only desired amount of enemies are alive
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
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