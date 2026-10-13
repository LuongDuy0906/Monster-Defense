using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    public static event Action<int> OnWaveChanged;
    public static event Action OnMissionCompleted;

    [SerializeField] private ObjectPooler orcPool;
    [SerializeField] private ObjectPooler dragonPool;
    [SerializeField] private ObjectPooler kaijuPool;
    private WaveData[] _waves => LevelManager.Instance.CurrentLevel.waves; 

    private float _spawnTimer;
    private Dictionary<EnemyType, ObjectPooler> _poolDictionary;
    private int _currentWaveIndex = 0;
    private int _waveCounter;
    private WaveData CurrentWave => _waves[_currentWaveIndex];
    private float _spawnCounter;
    private int _enemiesRemoved;
    private float _timeBetweenWaves = 1f;
    private float _waveCooldown;
    private bool _isBetweenWaves = false;
    private bool _isEndlessMode = false;
    private Path _currentPath;
    private bool _isGamePlayScene = false;

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        } else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        _poolDictionary = new Dictionary<EnemyType, ObjectPooler>()
        {
            { EnemyType.Orc, orcPool },
            { EnemyType.Dragon, dragonPool },
            { EnemyType.Kaiju, kaijuPool },
        };
    }

    void OnEnable()
    {
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
        Enemy.OnEnemyDestroyed -= HandleEnemyDestroyed;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        OnWaveChanged?.Invoke(_waveCounter);
    }

    void Update()
    {
        if(!_isGamePlayScene) return;

        if(_isBetweenWaves)
        {
            _waveCooldown -= Time.deltaTime;
            if(_waveCooldown <= 0f)
            {
                _currentWaveIndex = (_currentWaveIndex + 1) % _waves.Length ;
                _waveCounter++;
                OnWaveChanged?.Invoke(_waveCounter);
                AudioManager.Instance.PlaySound(CurrentWave.waveSpawnClip);
                _spawnCounter = 0;
                _enemiesRemoved = 0;
                _spawnTimer = 0f;
                _isBetweenWaves = false;
            }
        } else
        {
            _spawnTimer -= Time.deltaTime;

            if(_spawnTimer < 0 && _spawnCounter < CurrentWave.enemiesPerWave)
            {
                _spawnTimer = CurrentWave.spawnInterval;
                SpawnEnemy();
                _spawnCounter++;
            } else if(_spawnCounter >= CurrentWave.enemiesPerWave && _enemiesRemoved >= CurrentWave.enemiesPerWave)
            {
                if(_waveCounter + 1 >= LevelManager.Instance.CurrentLevel.wavesToWin && !_isEndlessMode && GameManager.Instance.Lives > 0)
                {
                    OnMissionCompleted?.Invoke();
                } else
                {
                    _isBetweenWaves = true;
                    _waveCooldown = _timeBetweenWaves;
                }
            }
        }

    }

    void SpawnEnemy()
    {
        if(_poolDictionary.TryGetValue(CurrentWave.enemyType, out var pool))
        { 
            GameObject spawnedObject = pool.GetPooledObject();
            spawnedObject.transform.position = transform.position;

            float healthMultiplier = 1f + (_waveCounter * 0.1f);

            Enemy enemy = spawnedObject.GetComponent<Enemy>();
            enemy.Initialize(_currentPath, healthMultiplier); 

            spawnedObject.SetActive(true);
        }
    }

    private void HandleEnemyReachedEnd(EnemyData data) 
    {
        _enemiesRemoved++;
    }

    private void HandleEnemyDestroyed(Enemy enemy)
    {
        _enemiesRemoved++;
    }

    public void EnableEndlessMode()
    {
        _isEndlessMode = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _isGamePlayScene = scene.name != "MainMenuScene";
        ResetWaveState();

        if(!_isGamePlayScene) return;
        _currentPath = GameObject.Find("Path-1").GetComponent<Path>();
        AudioManager.Instance.PlaySound(CurrentWave.waveSpawnClip);

        if(LevelManager.Instance.CurrentLevel != null)
        {
            transform.position = LevelManager.Instance.CurrentLevel.initialSpawnPosition;
        }
    }

    private void ResetWaveState()
    {
        _currentWaveIndex = 0;
        _waveCounter = 0;
        OnWaveChanged?.Invoke(_waveCounter);
        _spawnCounter = 0;
        _enemiesRemoved = 0;
        _spawnTimer = 0f;
        _isBetweenWaves = false;
        _isEndlessMode = false;

        foreach(var pool in _poolDictionary.Values)
        {
            if(pool != null)
            {
                pool.ResetPool();
            }
        }
    }
}
