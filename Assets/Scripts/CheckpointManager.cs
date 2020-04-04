using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ALICE.Utils.Animation;

namespace ALICE.Checkpoint
{
    // todo: Get rid of Find()

    public class CheckpointManager : MonoBehaviour
    {
        private static CheckpointManager _instance;
        public static CheckpointManager instance { get { return _instance; } }
                
        private CheckpointData lastCheckPoint = new CheckpointData();

        [SerializeField]
        private Animator checkpointReachedAnimator = null;

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

            this.gameObject.name += "NOWWOT";

            /* Start will only get called once as this is a singleton.
             * I need to find all the remaining checkpoints every time a level is loaded. */
            SceneManager.sceneLoaded += this.OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // New level
            if (scene.buildIndex != this.currentSceneIndex)
            {
                this.currentSceneIndex = scene.buildIndex;
                
                // Reset data
                this.ClearLastCheckpoint();
                this.Initialise();
            }
        }

        private void Initialise()
        {
            // Attach OnCheckpointReached callback to every Checkpoint in the current level
            foreach (Checkpoint checkpoint in GameObject.FindObjectsOfType<Checkpoint>())
                checkpoint.AddListener(this.OnCheckpointReached);
        }

        public void OnCheckpointReached(int checkpointIndex)
        {
            this.SaveLastCheckpoint(checkpointIndex);
            AnimationUtils.SetTrigger(this.checkpointReachedAnimator, "ShowCheckpoint");
        }

        private void SaveLastCheckpoint(int currentCheckpoint)
        {
            Transform player = GameObject.FindGameObjectWithTag("Player").transform;

            this.lastCheckPoint = new CheckpointData
            {
                index = currentCheckpoint,
                enemyPositions = this.GetEnemyPositions(),
                playerPosition = player.position,
                playerRotation = player.rotation
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

        public void LoadLastCheckpointOnReload()
        {
            SceneManager.sceneLoaded += this.LoadLastCheckpoint;
        }

        private void LoadLastCheckpoint(Scene scene, LoadSceneMode mode)
        {
            if (this.lastCheckPoint.index < 0)
                return;

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

            // Assign player position
            GameObject.FindGameObjectWithTag("Player").transform.position = this.lastCheckPoint.playerPosition;
            GameObject.FindGameObjectWithTag("Player").transform.rotation = this.lastCheckPoint.playerRotation;

            SceneManager.sceneLoaded -= this.LoadLastCheckpoint;
        }

        public void ClearLastCheckpoint()
        {
            // Note: Not clearing other data as will use allocate memory.
            this.lastCheckPoint = new CheckpointData
            {
                index = -1
            };
        }
    }
}