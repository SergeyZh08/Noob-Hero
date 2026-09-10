using System;
using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour, IPoolable
{
    [SerializeField] private EffectSettings _effectSettings;
    [SerializeField] private LayerMask _layerMask;
    private float _radius;
    private float _damage;
    private float _lifeTime;
    private Collider[] _enemies = new Collider[50];
    private bool _isActivated = false;
    private float _timer = 0;
    private Action<Bomb> _release;
    private FxManager _fxManager;

    public void Init(float radius, float damage, float lifeTime, FxManager fxManager, Action<Bomb> action)
    {
        _radius = radius;
        _damage = damage;
        _lifeTime = lifeTime;
        _fxManager = fxManager;
        _release = action;
    }

    private void Update()
    {
        if (_isActivated)
        {
            return;
        }

        _timer += Time.deltaTime;

        if (_timer >= _lifeTime)
        {
            Die();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isActivated)
        {
            return;
        }

        if (other.TryGetComponent(out Enemy _))
        {
            _isActivated = true;

            int len = Physics.OverlapSphereNonAlloc(transform.position, _radius, _enemies, _layerMask, QueryTriggerInteraction.Ignore);

            for (int i = 0; i < len; i++)
            {
                if (_enemies[i].TryGetComponent(out Enemy enemy))
                {
                    enemy.TakeDamage(_damage);
                }
            }

            _fxManager.Play(_effectSettings, transform.position);
            Die();
        }
    }

    private void Die()
    {
        _release?.Invoke(this);
    }

    public void OnGetFromPool()
    {
        _timer = 0;
        _isActivated = false;
    }

    public void OnReleaseToPool()
    {
        _release = null;
        _radius = 0;
        _damage = 0;
        _fxManager = null;
        _lifeTime = 0;
    }
}
