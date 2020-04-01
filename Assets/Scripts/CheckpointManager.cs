using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    private static CheckpointManager _instance;
    public static CheckpointManager instance { get { return _instance; } }

    [System.Serializable]
    private struct CheckpointData
    {
        public int index;
        public Vector3[] enemyPositions;
        public Vector3 playerPosition;
        public Quaternion playerRotation;
        //todo ammo
        // todo health
        // todo grenades
        // todo guns?
    };

    [SerializeField]
    private CheckpointData _lastCheckPoint;

    private Text _checkpointReachedText = null;


    void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;

        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        if (_lastCheckPoint.enemyPositions == null || _lastCheckPoint.enemyPositions.Length == 0)
            Reset();
    }

    void OnEnable()
    {
        _checkpointReachedText = GameObject.FindGameObjectWithTag("CheckpointLabel").GetComponent<Text>();
        _checkpointReachedText.gameObject.SetActive(false);
    }


    public void CheckpointReached(int currentCheckpoint)
    {
        if (currentCheckpoint <= _lastCheckPoint.index)
            return;

        _lastCheckPoint = new CheckpointData();

        _lastCheckPoint.index = currentCheckpoint;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        _lastCheckPoint.enemyPositions = new Vector3[enemies.Length];
        for (int i = 0; i < enemies.Length; i++)
        {
            //Debug.Log("Saved: " + enemies[i].transform.position);
            _lastCheckPoint.enemyPositions[i] = enemies[i].transform.position;
        }
        _lastCheckPoint.playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        _lastCheckPoint.playerRotation = GameObject.FindGameObjectWithTag("Player").transform.rotation;

        StartCoroutine(CheckpointReachedText());
    }

    private IEnumerator CheckpointReachedText()
    {
       // Debug.Log("Start");
        if(_checkpointReachedText == null)
            _checkpointReachedText = GameObject.FindGameObjectWithTag("CheckpointLabel").GetComponent<Text>();

        _checkpointReachedText.gameObject.SetActive(true);
        _checkpointReachedText.canvasRenderer.SetAlpha(0.0f);
        _checkpointReachedText.CrossFadeAlpha(255.0f, 0.5f, false);
        yield return new WaitForSeconds(3.0f);
        //_checkpointReachedText.canvasRenderer.SetAlpha(1.0f);
        _checkpointReachedText.CrossFadeAlpha(1.0f, 0.5f, false);
        yield return new WaitForSeconds(0.5f);
        _checkpointReachedText.gameObject.SetActive(false);
       // Debug.Log("end");
    }

    public void LoadWhenSceneLoaded()
    {
       // Debug.Log("Subscribed");
        SceneManager.sceneLoaded += LoadLastCheckpoint;
    }

    public void Reset()
    {
        //Debug.Log("RESET");
        _lastCheckPoint = new CheckpointData();
        _lastCheckPoint.index = -1;
    }

    private void LoadLastCheckpoint(Scene scene, LoadSceneMode mode)
    {
       // Debug.Log("ENTERED");
        if (_lastCheckPoint.index < 0)
            return;
       // Debug.Log("LOAD CHECKPOINT");
  
        // Ensure only desired amount of enemies are alive
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < (enemies.Length - _lastCheckPoint.enemyPositions.Length); i++)
        {
           // Debug.Log("DESTROYED");
            Destroy(enemies[i]);
            enemies[i] = null;
        }
        // Remove null objects
        List<GameObject> unassignedEnemies = new List<GameObject>();
        for(int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == null)
                continue;
           // Debug.Log("ADDED");
            unassignedEnemies.Add(enemies[i]);
        }

        // Assign enemy positions
        for(int i = 0; i < unassignedEnemies.Count; i++)
        {
          //  Debug.Log("Loaded: " + _lastCheckPoint.enemyPositions[i]);
            unassignedEnemies[i].transform.position = _lastCheckPoint.enemyPositions[i];
        }

        // Assign player position
        GameObject.FindGameObjectWithTag("Player").transform.position = _lastCheckPoint.playerPosition;
        GameObject.FindGameObjectWithTag("Player").transform.rotation = _lastCheckPoint.playerRotation;

        SceneManager.sceneLoaded -= LoadLastCheckpoint;
    }
   
}
