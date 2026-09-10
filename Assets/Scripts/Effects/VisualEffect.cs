using System;
using System.Collections;
using UnityEngine;

public class VisualEffect : MonoBehaviour, IPoolable
{
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private AudioSource _audioSource;

    private Action<VisualEffect> _release;
    private Coroutine _currentCoroutine;

    public void Play(EffectSettings settings, Vector3 position, Action<VisualEffect> release)
    {
        _release = release;

        transform.position = position;

        _particleSystem.Play();

        if (settings.Sound != null && _audioSource != null)
        {
            _audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
            _audioSource.PlayOneShot(settings.Sound);
        }

        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }

        _currentCoroutine = StartCoroutine(DelayRoutine());
    }

    private IEnumerator DelayRoutine()
    {
        yield return new WaitUntil(() => !_particleSystem.IsAlive(true));

        _currentCoroutine = null;
        _release?.Invoke(this);
    }

    public void OnGetFromPool()
    {
        
    }

    public void OnReleaseToPool()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
        }

        _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _release = null;
    }
}