using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action<int> OnLivesChanged;

    private int _lives = 20;

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
        OnLivesChanged?.Invoke(_lives);
    }

    private void HandleEnemyReachedEnd(EnemyData data)
    {
        _lives = Mathf.Max(0, _lives - data.damage);
        OnLivesChanged?.Invoke(_lives);
    }
}
