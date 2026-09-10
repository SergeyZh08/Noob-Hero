using UnityEngine;

[CreateAssetMenu(fileName = "EffectSettings", menuName = "Effects/Effect Settings")]
public class EffectSettings : ScriptableObject
{
    public VisualEffect Prefab;
    public AudioClip Sound;
}