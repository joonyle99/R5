using System;
using UnityEngine;
using JoonyleGameDevKit;

public abstract class EnemyBehaviour : CombatEntity
{
    public void Initialize(Action<int> onDamaged, Action onDead)
    {
        InitCombatEntity(onDamaged, onDead);

        OnInitialize();
    }

    // ============ ... ============

    public virtual void Tick(float deltaTime) { }

    protected virtual void OnInitialize() { }

    // ============ ... ============

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<PlayerBehaviour>(out var player))
        {
            player.TakeDamage(attack);
        }
    }
}
