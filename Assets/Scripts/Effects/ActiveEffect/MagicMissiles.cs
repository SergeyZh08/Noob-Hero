using System;
using UnityEngine;

public class MagicMissiles : MonoBehaviour, IPoolable
{
    [SerializeField] private EffectSettings _effectSettings;
    [SerializeField] private LayerMask _layerMask;
    private Collider[] _colliders = new Collider[25];
    private float _speed;
    private float _damage;
    private Enemy _enemy;
    private Action<MagicMissiles> _release;
    private bool _isDead = false;
    private float _radius = 1f;
    private FxManager _fxManager;

    public void Init(float speed, float damage, Enemy enemy, FxManager fxManager, Action<MagicMissiles> action)
    {
        _speed = speed;
        _damage = damage;
        _enemy = enemy;
        _fxManager = fxManager;
        _release = action;
    }

    public void Update()
    {
        if (_enemy.IsAlive)
        {
            transform.position = Vector3.MoveTowards(transform.position, _enemy.transform.position, Time.deltaTime * _speed);

            if (transform.position == _enemy.transform.position)
            {
                TakeDamage();
                Die();
            }
        }
        else
        {
            Die();
        }
    }

    private void TakeDamage()
    {
        int len = Physics.OverlapSphereNonAlloc(transform.position, _radius, _colliders, _layerMask, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < len; i++)
        {
            if (_colliders[i].TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(_damage);
            }
        }

        _fxManager.Play(_effectSettings, transform.position);
    }

    private void Die()
    {
        if (_isDead)
        {
            return;
        }

        _isDead = true;
        _release?.Invoke(this);
    }

    public void OnGetFromPool()
    {
        _isDead = false;
    }

    public void OnReleaseToPool()
    {
        _release = null;
        _enemy = null;
        _speed = 0;
        _damage = 0;
    }
}
