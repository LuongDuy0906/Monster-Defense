using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private ObjectPooler orcPool;
    [SerializeField] private ObjectPooler dragonPool;
    [SerializeField] private ObjectPooler kaijuPool;
    [SerializeField] private WaveData[] waves; 

    private float _spawnTimer;
    private Dictionary<EnemyType, ObjectPooler> _poolDictionary;
    private int _currentWaveIndex = 0;
    private WaveData CurrentWave => waves[_currentWaveIndex];
    private float _spawnCounter;
    private int _enemiesRemoved;

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

    void Update()
    {
        _spawnTimer -= Time.deltaTime;

        if(_spawnTimer < 0 && _spawnCounter < CurrentWave.enemiesPerWave)
        {
            _spawnTimer = CurrentWave.spawnInterval;
            SpawnEnemy();
            _spawnCounter++;
        } else if(_spawnCounter >= CurrentWave.enemiesPerWave && _enemiesRemoved >= CurrentWave.enemiesPerWave)
        {
            _currentWaveIndex = (_currentWaveIndex + 1) % waves.Length ;
            _spawnCounter = 0;
            _enemiesRemoved = 0;
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
