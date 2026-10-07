using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private ObjectPooler pool;

    private float _spawnTimer;
    private float _spawnInterval = 1f;

    void Update()
    {
        _spawnTimer -= Time.deltaTime;

        if(_spawnTimer < 0)
        {
            _spawnTimer = _spawnInterval;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        GameObject spawnedObject = pool.GetPooledObject();
        spawnedObject.transform.position = transform.position;
        spawnedObject.SetActive(true);
    }
}
