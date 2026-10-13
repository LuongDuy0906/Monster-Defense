using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static event Action<EnemyData> OnEnemyReachedEnd;
    public static event Action<Enemy> OnEnemyDestroyed;

    [SerializeField] private EnemyData data;
    [SerializeField] private Transform healthBar;

    private Path _currentPath;
    private Vector3 _targetPosition;
    private int _currentWaypoint;
    private float _lives;
    private Vector3 _healthBarOriginalScale;
    private float _maxLives;
    public EnemyData Data => data;
    private bool _hasBeenCounted = false;
    private float _speed;
    private Vector3 _offset;

    void Awake()
    {
        _currentPath = GameObject.Find("Path-1").GetComponent<Path>();
        _healthBarOriginalScale = healthBar.localScale;
    }

    void OnEnable()
    {
        _currentWaypoint = 0;
        _targetPosition = _currentPath.GetPosition(_currentWaypoint) + _offset;
    }

    void Update()
    {
        if(_hasBeenCounted)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _speed * Time.deltaTime);
        
        float relativeDistance = (transform.position - _targetPosition).magnitude;

        if(relativeDistance < 0.1f)
        {
            if(_currentWaypoint < _currentPath.Waypoints.Length - 1)
            {
                _currentWaypoint++;
                _targetPosition = _currentPath.GetPosition(_currentWaypoint) + _offset;
            }
            else
            {
                _hasBeenCounted = true;
                OnEnemyReachedEnd?.Invoke(data);
                gameObject.SetActive(false);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if(_hasBeenCounted) return;

        _lives -= damage;
        _lives = Mathf.Max(0, _lives);
        UpdateHealthBar();

        if(_lives == 0)
        {
            AudioManager.Instance.PlayEnemyDestroy();
            _hasBeenCounted = true;
            OnEnemyDestroyed?.Invoke(this);
            gameObject.SetActive(false);
        }
    }

    private void UpdateHealthBar()
    {
        float healthPercent = _lives / _maxLives;
        Vector3 scale = _healthBarOriginalScale;
        scale.x = _healthBarOriginalScale.x * healthPercent;
        healthBar.localScale = scale;
    }

    public void Initialize(float healthMultiplier) 
    {
        _hasBeenCounted = false;
        _maxLives = data.lives * healthMultiplier;
        _lives = _maxLives;
        UpdateHealthBar();
        _speed = UnityEngine.Random.Range(data.minSpeed, data.maxSpeed);

        float offsetX = UnityEngine.Random.Range(-0.5f, 0.5f);
        float offsetY = UnityEngine.Random.Range(-0.5f, 0.5f);

        _offset = new Vector3(offsetX, offsetY);
    }
}
