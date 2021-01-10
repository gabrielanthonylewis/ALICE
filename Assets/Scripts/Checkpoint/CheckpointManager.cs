using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace ALICE.Checkpoint
{
    public class CheckpointManager : MonoBehaviour
    {
        private static CheckpointManager _instance;
        public static CheckpointManager instance { get { return _instance;  } }

        [SerializeField] private Animator checkpointReachedAnimator = null;

        private CheckpointData lastCheckPoint = null;

        private readonly string enemyTag = "Enemy";
        private readonly string playerTag = "Player";
        private readonly string checkpointReachedAnimationParam = "ShowCheckpoint";

        private Transform player = null;
        private Inventory playerInventory = null;
        private int currentSceneIndex = -1;
        private bool waitForCameraEvent = false;
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

            SceneManager.sceneLoaded += this.OnSceneLoaded;
        }

        // Like Start except called every time a scene is loaded.
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            this.AddCheckpointListeners();

            bool isNewLevel = (scene.buildIndex != this.currentSceneIndex);
            this.skipNextCheckpoint = !isNewLevel;
            this.currentSceneIndex = scene.buildIndex;

            this.waitForCameraEvent = (GameObject.FindObjectOfType<CinematicCameraEvent>() != null);
            if(!this.waitForCameraEvent)
            {
                if(this.lastCheckPoint != null)
                    this.LoadLastCheckpoint();
                else
                    this.SpawnPlayerOnSpawnPoint();
            }
        }

        public void SpawnPlayerOnSpawnPoint()
        {
            this.player = GameObject.FindObjectOfType<PlayerSpawnPoint>().SpawnPlayer().transform;
            this.playerInventory = this.player.GetComponentInChildren<Inventory>();

            // Load inventory but reset the rest as new spawn.
            if(this.lastCheckPoint != null)
            {
                this.lastCheckPoint.enemies = new ActorData[0];
                this.lastCheckPoint.playerPosition = this.player.position;
                this.lastCheckPoint.playerRotation = this.player.rotation;

                this.player.GetComponent<Destructable>().SetHealth(this.lastCheckPoint.health);
                this.player.GetComponent<SlowmoController>().SetRemainingTime(this.lastCheckPoint.slowmo);
                this.LoadInventory();
            }
        }

        public void OnRestartLevel()
        {
            this.ClearLastCheckpoint();
            this.currentSceneIndex = -1;
        }

        public bool HasCheckpoint()
        {
            return (this.lastCheckPoint != null);
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
        }

        public void SaveLastCheckpoint(bool showNotification = true)
        {
            this.lastCheckPoint = new CheckpointData
            {
                enemies = this.GetEnemiesActorData(),
                playerPosition = this.player.position,
                playerRotation = this.player.rotation,
                health = this.player.GetComponent<Destructable>().GetHealth(),
                slowmo = this.player.GetComponent<SlowmoController>().remainingTime,
                inventory = this.playerInventory.GetInventoryData()
            };

            if(showNotification)
            {
                AnimationUtils.SetTrigger(this.checkpointReachedAnimator,
                    this.checkpointReachedAnimationParam);
            }
        }

        private ActorData[] GetEnemiesActorData()
        {
             GameObject[] enemies = GameObject.FindGameObjectsWithTag(this.enemyTag);

             ActorData[] actorData = new ActorData[enemies.Length];
             for(int i = 0; i < actorData.Length; i++)
             {
                 actorData[i] = new ActorData
                 {
                     objectID = GlobalObjectId.GetGlobalObjectIdSlow(enemies[i]).targetObjectId,
                     position = enemies[i].transform.position,
                     rotation = enemies[i].transform.rotation
                 };
             }

             return actorData;
        }

        private void LoadLastCheckpoint()
        {
            if (this.lastCheckPoint == null)
                return;

            this.LoadEnemies();

            this.player = GameObject.FindObjectOfType<PlayerSpawnPoint>().SpawnPlayer(
                this.lastCheckPoint.playerPosition, this.lastCheckPoint.playerRotation).transform;

            this.playerInventory = this.player.GetComponentInChildren<Inventory>();
            this.player.GetComponent<Destructable>().SetHealth(this.lastCheckPoint.health);
            this.player.GetComponent<SlowmoController>().SetRemainingTime(this.lastCheckPoint.slowmo);

            this.LoadInventory();
        }

        private void LoadInventory()
        {
            this.playerInventory.LoadInventory(this.lastCheckPoint.inventory);
        }

        private void LoadEnemies()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(this.enemyTag);
            for(int i = 0; i < enemies.Length; i++)
            {
                bool shouldDestroy = true;
                for(int j = 0; j < this.lastCheckPoint.enemies.Length; j++)
                {
                    if(GlobalObjectId.GetGlobalObjectIdSlow(enemies[i]).targetObjectId ==
                        this.lastCheckPoint.enemies[j].objectID)
                    {
                        shouldDestroy = false;
                    
                        enemies[i].transform.position = this.lastCheckPoint.enemies[j].position;
                        enemies[i].transform.rotation = this.lastCheckPoint.enemies[j].rotation;
                    
                        break;
                    }
                }

                if(shouldDestroy)
                    GameObject.Destroy(enemies[i]);
            }
        }

        private void ClearLastCheckpoint()
        {
            this.lastCheckPoint = null;
        }
    }
}