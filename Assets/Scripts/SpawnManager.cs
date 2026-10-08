using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static event Action<int> OnWaveChanged;

    [SerializeField] private ObjectPooler orcPool;
    [SerializeField] private ObjectPooler dragonPool;
    [SerializeField] private ObjectPooler kaijuPool;
    [SerializeField] private WaveData[] waves; 

    private float _spawnTimer;
    private Dictionary<EnemyType, ObjectPooler> _poolDictionary;
    private int _currentWaveIndex = 0;
    private int _waveCounter;
    private WaveData CurrentWave => waves[_currentWaveIndex];
    private float _spawnCounter;
    private int _enemiesRemoved;
    private float _timeBetweenWaves = 2f;
    private float _waveCooldown;
    private bool _isBetweenWaves = false;

    void Awake()
    {
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
    }

    void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
    }

    void Start()
    {
        OnWaveChanged?.Invoke(_waveCounter);
    }

    void Update()
    {
        if(_isBetweenWaves)
        {
            _waveCooldown -= Time.deltaTime;
            if(_waveCooldown <= 0f)
            {
                _currentWaveIndex = (_currentWaveIndex + 1) % waves.Length ;
                _waveCounter++;
                OnWaveChanged?.Invoke(_waveCounter);
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
                
                _isBetweenWaves = true;
                _waveCooldown = _timeBetweenWaves;
            }
        }

    }

    void SpawnEnemy()
    {
        if(_poolDictionary.TryGetValue(CurrentWave.enemyType, out var pool))
        { 
            GameObject spawnedObject = pool.GetPooledObject();
            spawnedObject.transform.position = transform.position;
            spawnedObject.SetActive(true);
        }
    }

    private void HandleEnemyReachedEnd(EnemyData data) 
    {
        _enemiesRemoved++;
    }
}
