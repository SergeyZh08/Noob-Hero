using System.Collections;
using UnityEngine;

public class Blink : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Enemy _enemy;
    [SerializeField] private float _blinkTime = 0.5f;
    private Coroutine _currentCoroutine;
    private MaterialPropertyBlock _block;
    private static int _baseColorID = Shader.PropertyToID("_Basecolor");

    private void Awake()
    {
        _block = new MaterialPropertyBlock();
    }

    private void OnEnable()
    {
        if (_enemy)
        {
            _enemy.Health.OnEnemyHit += StartBlink;
        }
    }

    private void OnDisable()
    {
        if (_enemy)
        {
            _enemy.Health.OnEnemyHit -= StartBlink;
        }

        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
        }

        SetColor(Color.clear);
    }

    private void StartBlink(EnemyHealthData _)
    {
        if (_currentCoroutine != null)
        {
            return;
        }

        _currentCoroutine = StartCoroutine(BlinkProcess());
    }

    private IEnumerator BlinkProcess()
    {
        for (float t = 0; t < _blinkTime; t += Time.deltaTime)
        {
            //красный цвет
            SetColor(new Color(Mathf.Sin(t * 30) * 0.5f + 0.5f, 0, 0));

            yield return null;
        }

        SetColor(Color.clear);
        _currentCoroutine = null;
    }

    private void SetColor(Color color)
    {
        _renderer.GetPropertyBlock(_block);

        _block.SetColor(_baseColorID, color);

        _renderer.SetPropertyBlock(_block);
    }
}
