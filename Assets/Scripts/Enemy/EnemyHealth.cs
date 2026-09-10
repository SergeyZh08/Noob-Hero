using System;
using UnityEngine;

[System.Serializable]
public struct EnemyHealthData
{
    public float StartHealth;
    [HideInInspector] public float CurrentMaxHealth;
    [HideInInspector] public float CurrentHealth;
}

public class EnemyHealth : MonoBehaviour
{
    public Enemy Enemy {get; private set;}
    [SerializeField] private EnemyHealthData _enemyHealthData;
    [SerializeField] private bool _needBoost = false;
    public event Action<EnemyHealthData> OnEnemyHit;

    public void Init(Enemy enemy)
    {
        Enemy = enemy;
    }

    public void SetHealth(float healthMultiplier)
    {
        // ((multiplier - 1) * current wawe * start Hp) + start Hp
        _enemyHealthData.CurrentMaxHealth = _needBoost ? _enemyHealthData.StartHealth + (_enemyHealthData.StartHealth * healthMultiplier) : _enemyHealthData.StartHealth;
        _enemyHealthData.CurrentHealth = _enemyHealthData.CurrentMaxHealth;
    }

    public void TakeDamage(float value)
    {
        _enemyHealthData.CurrentHealth = Mathf.Max(0, _enemyHealthData.CurrentHealth - value);
        OnEnemyHit?.Invoke(_enemyHealthData);

        if (_enemyHealthData.CurrentHealth == 0)
        {
            Die();
        }
    }

    [ContextMenu("Die")]
    private void Die()
    {
        Enemy.Die();
    }

    private void OnDisable()
    {
        OnEnemyHit = null;
    }
}
