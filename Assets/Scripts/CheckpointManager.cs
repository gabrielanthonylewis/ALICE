using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ALICE.Utils.Animation;

namespace ALICE.Checkpoint
{
    // todo: Get rid of Find()
    // todo: as this is a singleton I need to clear it next level, add in the new checkpoints

    public class CheckpointManager : MonoBehaviour
    {
        private static CheckpointManager _instance;
        public static CheckpointManager instance { get { return _instance; } }
                
        private CheckpointData lastCheckPoint = new CheckpointData();

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
            // Attach CheckpointReached callback to every Checkpoint in the current level
            foreach (Checkpoint checkpoint in GameObject.FindObjectsOfType<Checkpoint>())
                checkpoint.AddListener(this.CheckpointReached);
        }

        public void CheckpointReached(int checkpointIndex)
        {
            // todo: maybe I should just delete the checkpoint once it is reached?
            // then I dont have to do this and also the scene reloads anyways
            if (checkpointIndex <= this.lastCheckPoint.index)
                return;

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