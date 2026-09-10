using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private FxManager _fxManager;
    [SerializeField] private float _radiusForSpawnInside = 20f;
    [SerializeField] private float _radiusForSpawnOutside = 30f;
    [Header("Enemy Pool")]
    [SerializeField] private int _startSize;
    [SerializeField] private int _step;
    [SerializeField] private Transform _parent;
    private ChapterManager _waweManager;
    private ChapterSettings _currentChapter;
    private LootManager _lootManager;
    private Player _player;

    private Dictionary<Enemy, Pool<Enemy>> _pools = new Dictionary<Enemy, Pool<Enemy>>();
    private Dictionary<Enemy, Pool<Enemy>> _enemyToPool = new Dictionary<Enemy, Pool<Enemy>>();
    private List<Enemy> _spawnedEnemies = new List<Enemy>();
    public event Action AllEnemiesDie;
    private bool _lastWave;
    private int _currentWave;
    private int _maxEnemies;
    private float _enemyHealthMultiplier;

    public void Init(LootManager lootManager, Player player, ChapterManager waweManager, FxManager fxManager)
    {
        _waweManager = waweManager;
        _lootManager = lootManager;
        _fxManager = fxManager;
        _player = player;
        _currentWave = -1;

        _currentChapter = _waweManager.GetChapterSettings;
        _enemyHealthMultiplier = _currentChapter.EnemyHealthMultiplier;
        _maxEnemies = _currentChapter.MaxEnemies;

        StartCoroutine(PreLoadEnemies());
    }

    public void NextWave()
    {
        StopAllCoroutines();

        _currentWave++;

        if (_currentWave >= _currentChapter.EnemyWaves[0].NumberPerSecund.Length)
        {
            _lastWave = true;
            return;
        }

        for (int i = 0; i < _currentChapter.EnemyWaves.Length; i++)
        {
            if (_currentChapter.EnemyWaves[i].NumberPerSecund[_currentWave] > 0)
            {
                StartCoroutine(SpawnRoutuine(_currentChapter.EnemyWaves[i].Enemy, _currentChapter.EnemyWaves[i].NumberPerSecund[_currentWave]));
            }
        }
    }

    private IEnumerator SpawnRoutuine(Enemy enemy, float enemyPerSecunds)
    {
        var delay = new WaitForSeconds(1 / enemyPerSecunds);

        while (true)
        {
            yield return delay;

            SpawnAndInit(enemy);
        }
    }

    private void SpawnAndInit(Enemy enemyPrefab)
    {
        if (_spawnedEnemies.Count > _maxEnemies)
        {
            return;
        }

        Vector3 direction = Quaternion.Euler(0f, UnityEngine.Random.Range(-30f, 30f), 0f) * _player.Forward;

        float _radiusForSpawn = UnityEngine.Random.Range(_radiusForSpawnInside, _radiusForSpawnOutside);
        Vector3 position = _player.transform.position + direction * _radiusForSpawn;

        Enemy newEnemy = _pools[enemyPrefab].Get(e => e.transform.position = position);
        _enemyToPool[newEnemy] = _pools[enemyPrefab];

        newEnemy.OnEnemyDie += _lootManager.CreateLoot;
        newEnemy.OnEnemyDie += RemoveEnemy;

        newEnemy.Movement.SetTarget(_player.transform);
        // ((multiplier - 1) * current wawe * start Hp) + start Hp
        newEnemy.Health.SetHealth((_enemyHealthMultiplier - 1) * _currentWave);
        _spawnedEnemies.Add(newEnemy);
    }

    private void RemoveEnemy(Enemy enemy)
    {
        enemy.OnEnemyDie -= _lootManager.CreateLoot;
        enemy.OnEnemyDie -= RemoveEnemy;

        _fxManager.Play(enemy.EffectSettings, enemy.transform.position);

        _spawnedEnemies.Remove(enemy);

        _enemyToPool[enemy].Release(enemy);
        _enemyToPool.Remove(enemy);

        if (_lastWave && _spawnedEnemies.Count == 0)
        {
            AllEnemiesDie?.Invoke();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Player player = FindFirstObjectByType<Player>();
        Handles.color = Color.coral;
        Handles.DrawWireDisc(player.transform.position, Vector3.up, _radiusForSpawnInside);
        Handles.DrawWireDisc(player.transform.position, Vector3.up, _radiusForSpawnOutside);
    }
#endif

    public Enemy[] GetClosest(Vector3 point, int count)
    {
        if (_spawnedEnemies != null)
        {
            _spawnedEnemies.Sort((a, b) =>
            {
                float ad = (a.transform.position - point).sqrMagnitude;
                float bd = (b.transform.position - point).sqrMagnitude;
                return ad.CompareTo(bd);
            });

            int len = Mathf.Min(count, _spawnedEnemies.Count);

            Enemy[] enemies = new Enemy[len];

            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i] = _spawnedEnemies[i];
            }

            return enemies;
        }

        return new Enemy[0];
    }

    public IEnumerator PreLoadEnemies()
    {
        for (int i = 0; i < _currentChapter.EnemyWaves.Length; i++)
        {
            Enemy enemyPrefab = _currentChapter.EnemyWaves[i].Enemy;

            if (enemyPrefab == null || _pools.ContainsKey(enemyPrefab))
            {
                continue;
            }
            Pool<Enemy> pool = new Pool<Enemy>(enemyPrefab, _startSize, _step, _parent);
            _pools.Add(enemyPrefab, pool);

            yield return null;
        }
    }
}
