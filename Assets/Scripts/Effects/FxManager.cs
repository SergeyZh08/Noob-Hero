using System.Collections.Generic;
using UnityEngine;

public class FxManager : MonoBehaviour
{
    private Dictionary<EffectSettings, Pool<VisualEffect>> _pools = new Dictionary<EffectSettings, Pool<VisualEffect>>();

    public void Play(EffectSettings settings, Vector3 position)
    {
        if (settings == null || settings.Prefab == null)
        {
            return;
        }

        if (!_pools.TryGetValue(settings, out Pool<VisualEffect> pool))
        {
            pool = new Pool<VisualEffect>(settings.Prefab, 5, 5, transform);
            _pools.Add(settings, pool);
        }

        VisualEffect effect = pool.Get();
        effect.Play(settings, position, pool.Release);
    }
}