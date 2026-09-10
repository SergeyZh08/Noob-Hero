using System.Collections;
using TMPro;
using UnityEngine;

public class EnemyHit : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    private MaterialPropertyBlock _block;
    [SerializeField] private Enemy _enemy;
    private static readonly WaitForSeconds _delay = new WaitForSeconds(1f);
    private static int _currentHealthID = Shader.PropertyToID("_CurrentHealth");

    private Coroutine _currentCoroutine;

    private void Start()
    {
        _block = new MaterialPropertyBlock();
    }

    private void OnEnable()
    {
        if (_enemy)
        {
            _enemy.Health.OnEnemyHit += Hit;
        }

        _renderer.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (_enemy)
        {
            _enemy.Health.OnEnemyHit -= Hit;
        }
        
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
        }
    }

    private void Hit(EnemyHealthData enemyHealthData)
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }

        _renderer.GetPropertyBlock(_block);
        _block.SetFloat(_currentHealthID, enemyHealthData.CurrentHealth / enemyHealthData.CurrentMaxHealth);
        _renderer.SetPropertyBlock(_block);

        _renderer.gameObject.SetActive(true);

        _currentCoroutine = StartCoroutine(DelayRoutine());
    }

    private IEnumerator DelayRoutine()
    {
        yield return _delay;

        _renderer.gameObject.SetActive(false);
        _currentCoroutine = null;
    }
}
