using UnityEngine;

public class LevelupManager : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private EffectManager _effectManager;
    [SerializeField] private ExperienceVisual _experienceVisual;

    private int _pendingLevelUps = 0;
    private bool _isProcessing = false;

    public void Init()
    {
        _player.Experience.OnLevelUp += LevelUp;

        _experienceVisual.Init(_player.transform);
    }

    private void OnDisable()
    {
        _player.Experience.OnLevelUp -= LevelUp;
    }

    public void StartLeveling()
    {
        ShowEffect();
    }

    private void LevelUp(int level)
    {
        _pendingLevelUps++;
        ProcessNext();
    }

    private void ProcessNext()
    {
        if (_isProcessing || _pendingLevelUps <= 0)
        {
            return;
        }

        _isProcessing = true;
        _pendingLevelUps--;

        _experienceVisual.Play(ShowEffect);
    }

    private void ShowEffect()
    {
        _effectManager.ShowEffect(OnCardsResolved);
    }

    private void OnCardsResolved()
    {
        _isProcessing = false;
        ProcessNext();
    }
}