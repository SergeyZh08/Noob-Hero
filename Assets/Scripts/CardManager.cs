using System;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] private GameObject _cardParent;
    [SerializeField] private Card[] _cards;
    private EffectManager _effectManager;
    private GameStateManager _gameStateManager;
    private EnemySpawner _enemySpawner;
    private Action _onCardsResolved;

    public void Init(EffectManager effectManager, GameStateManager gameStateManager, EnemySpawner enemySpawner)
    {
        _effectManager = effectManager;
        _gameStateManager = gameStateManager;
        _enemySpawner = enemySpawner;

        for (int i = 0; i < _cards.Length; i++)
        {
            _cards[i].Init(SelectCard);
        }
    }

    public void ShowCards(List<Effect> effects, Action onCardsResolved = null)
    {
        _onCardsResolved = onCardsResolved;

        _cardParent.SetActive(true);

        for (int i = 0; i < _cards.Length; i++)
        {
            _cards[i].Show(effects[i]);
        }

        _gameStateManager.SetCardState();
    }

    private void SelectCard(Effect effect)
    {
        _effectManager.AddEffect(effect);
        _enemySpawner.NextWave();
        _gameStateManager.SetAction();
        
        HideCards();

        _onCardsResolved?.Invoke();
        _onCardsResolved = null;
    }

    public void HideCards()
    {
        _cardParent.SetActive(false);
    }
}
