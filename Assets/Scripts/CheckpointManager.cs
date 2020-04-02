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
                
        private CheckpointData lastCheckPoint;

        [SerializeField]
        private Animator checkpointReachedAnimator = null;
        
        /* Singleton pattern */
        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(this.gameObject);
            else
                _instance = this;

            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            if (this.lastCheckPoint.enemyPositions == null || this.lastCheckPoint.enemyPositions.Length == 0)
                ClearLastCheckpoint();
        }

        public void CheckpointReached(int currentCheckpoint)
        {
            if (currentCheckpoint <= this.lastCheckPoint.index)
                return;

            this.SaveLastCheckpoint(currentCheckpoint);

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

        public void LoadWhenSceneLoaded()
        {
            Debug.Log("!!SUBSCRIBED!!");
            SceneManager.sceneLoaded += this.LoadLastCheckpoint;
        }

        private void LoadLastCheckpoint(Scene scene, LoadSceneMode mode)
        {
            // Debug.Log("ENTERED");
            if (this.lastCheckPoint.index < 0)
                return;
            // Debug.Log("LOAD CHECKPOINT");

            // Ensure only desired amount of enemies are alive
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            for (int i = 0; i < (enemies.Length - this.lastCheckPoint.enemyPositions.Length); i++)
            {
                // Debug.Log("DESTROYED");
                Destroy(enemies[i]);
                enemies[i] = null;
            }
            // Remove null objects
            List<GameObject> unassignedEnemies = new List<GameObject>();
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] == null)
                    continue;
                // Debug.Log("ADDED");
                unassignedEnemies.Add(enemies[i]);
            }

            // Assign enemy positions
            for (int i = 0; i < unassignedEnemies.Count; i++)
            {
                //  Debug.Log("Loaded: " + _lastCheckPoint.enemyPositions[i]);
                unassignedEnemies[i].transform.position = this.lastCheckPoint.enemyPositions[i];
            }

            // Assign player position
            GameObject.FindGameObjectWithTag("Player").transform.position = this.lastCheckPoint.playerPosition;
            GameObject.FindGameObjectWithTag("Player").transform.rotation = this.lastCheckPoint.playerRotation;

            SceneManager.sceneLoaded -= LoadLastCheckpoint;
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