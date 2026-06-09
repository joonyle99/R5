using System;
using UnityEngine;

public class EnemyManager
{
    private EnemyBehaviour[] _enemies;
    private int _aliveCount;

    public EnemyManager()
    {
        
    }

    public void Initialize()
    {
        _enemies = UnityEngine.Object.FindObjectsByType<EnemyBehaviour>(FindObjectsSortMode.None);
        _aliveCount = _enemies.Length;

        foreach (var enemy in _enemies)
            enemy.Initialize(null, OnEnemyDead);
    }

    public void Tick(float deltaTime)
    {
        foreach (var enemy in _enemies)
        {
            if (!enemy.IsDead)
                enemy.Tick(deltaTime);
        }
    }

    private void OnEnemyDead()
    {
        _aliveCount--;
    }
}
