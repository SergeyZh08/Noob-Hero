using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField] private List<ActiveEffect> _activeEffects = new List<ActiveEffect>();
    [SerializeField] private List<ActiveEffect> _collectedActiveEffects = new List<ActiveEffect>();

    [SerializeField] private List<PassiveEffect> _passiveEffects = new List<PassiveEffect>();
    [SerializeField] private List<PassiveEffect> _collectedPassiveEffects = new List<PassiveEffect>();

    private CardManager _cardManager;
    private TopIconManager _topIconManager;
    private Player _player;
    private bool _isFirstShow = true;

    public void Update()
    {
        for (int i = 0; i < _collectedActiveEffects.Count; i++)
        {
            _collectedActiveEffects[i].Tick(Time.deltaTime);
        }
    }

    public void Init(EnemySpawner enemySpawner, CardManager cardManager, TopIconManager topIconManager, Player player, FxManager fxManager)
    {
        _cardManager = cardManager;
        _topIconManager = topIconManager;
        _player = player;

        for (int i = 0; i < _activeEffects.Count; i++)
        {
            _activeEffects[i] = Instantiate(_activeEffects[i]);
            _activeEffects[i].Init(_player, enemySpawner, fxManager);
        }

        for (int i = 0; i < _passiveEffects.Count; i++)
        {
            _passiveEffects[i] = Instantiate(_passiveEffects[i]);
            _passiveEffects[i].Init(_player, enemySpawner, fxManager);
        }
    }

    public void ShowEffect(Action onCardsResolved = null)
    {
        List<Effect> effectsForShow = new List<Effect>();

        if (_isFirstShow)
        {
            foreach (var effect in _activeEffects)
            {
                if (effect is not ShieldEffect)
                {
                    effectsForShow.Add(effect);
                }
            }

            _isFirstShow = false;
        }
        else
        {
            for (int i = 0; i < _collectedActiveEffects.Count; i++)
            {
                if (_collectedActiveEffects[i].Level < 10)
                {
                    effectsForShow.Add(_collectedActiveEffects[i]);
                }
            }

            for (int i = 0; i < _collectedPassiveEffects.Count; i++)
            {
                if (_collectedPassiveEffects[i].Level < 10)
                {
                    effectsForShow.Add(_collectedPassiveEffects[i]);
                }
            }

            if (_collectedActiveEffects.Count < 4)
            {
                effectsForShow.AddRange(_activeEffects);
            }

            if (_collectedPassiveEffects.Count < 4)
            {
                effectsForShow.AddRange(_passiveEffects);
            }
        }

        int numberOfCardForShow = Mathf.Min(effectsForShow.Count, 3);

        int[] indexes = RandomSort(effectsForShow.Count, numberOfCardForShow);

        List<Effect> effectsForCards = new List<Effect>();

        for (int i = 0; i < indexes.Length; i++)
        {
            effectsForCards.Add(effectsForShow[indexes[i]]);
        }

        _cardManager.ShowCards(effectsForCards, onCardsResolved);
    }

    public void AddEffect(Effect effect)
    {
        if (effect is ActiveEffect c_effect)
        {
            if (!_collectedActiveEffects.Contains(c_effect))
            {
                _collectedActiveEffects.Add(c_effect);
                _activeEffects.Remove(c_effect);
                _topIconManager.Add(c_effect);
            }
        }

        if (effect is PassiveEffect o_effect)
        {
            if (!_collectedPassiveEffects.Contains(o_effect))
            {
                _collectedPassiveEffects.Add(o_effect);
                _passiveEffects.Remove(o_effect);
                _topIconManager.Add(o_effect);
            }
        }

        effect.Activate();
    }

    private int[] RandomSort(int lenght, int number)
    {
        int[] arr = new int[lenght];

        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = i;
        }

        for (int i = 0; i < arr.Length; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, arr.Length);

            int temp = arr[i];
            arr[i] = arr[randomIndex];
            arr[randomIndex] = temp;
        }

        int[] result = new int[number];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = arr[i];
        }

        return result;
    }
}
