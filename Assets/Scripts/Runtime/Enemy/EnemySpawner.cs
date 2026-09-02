using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    #region 인스펙터
    [Header("적 풀")]
    [SerializeField] private EnemyPool _enemyPool;

    [Header("맵")]
    [SerializeField] private MapManager _mapManager;

    [Header("스폰")]
    [SerializeField] private float _spawnInterval = 1f;

    [Header("플레이어")]
    [SerializeField] private Transform _target;

    [Header("원거리 적 총알 풀")]
    [SerializeField] private BulletPool _rangedEnemyBulletPool;

    [Header("적 비율")]
    [SerializeField] private float _rangedEnemyRate = 0.2f;

    [Header("적 체력")]
    [SerializeField] private float _healthMultiplier = 1f;

    [Header("적 공격력")]
    [SerializeField] private float _damageMultiplier = 1f;

    [Header("재화")]
    [SerializeField] private CurrencyManager _currencyManager;
    [SerializeField] private int _enemyReward = 10;
    #endregion

    #region 내부 변수
    private List<GameObject> _spawnedEnemies = new List<GameObject>();
    private Dictionary<GameObject, System.Action> _deathHandlers = new Dictionary<GameObject, System.Action>();
    private Dictionary<GameObject, System.Action> _deathAnimationHandlers =
    new Dictionary<GameObject, System.Action>();
    private bool _isSpawning;
    #endregion

    #region 프로퍼티
    public List<GameObject> SpawnedEnemies => _spawnedEnemies;
    public bool IsSpawning => _isSpawning;
    #endregion

    public void SetRangedEnemyRate(float rate)
    {
        _rangedEnemyRate = Mathf.Clamp01(rate);
    }

    public void SetHealthMultiplier(float multiplier)
    {
        _healthMultiplier = multiplier;
    }

    public void SetDamageMultiplier(float multiplier)
    {
        _damageMultiplier = multiplier;
    }

    public void StartSpawn(int count)
    {
        StartCoroutine(SpawnEnemies(count));
    }

    private IEnumerator SpawnEnemies(int count)
    {
        _isSpawning = true;

        for (int i = 0; i < count; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(_spawnInterval);
        }

        _isSpawning = false;
    }

    private void SpawnEnemy()
    {
        if (_enemyPool == null)
        {
            CPrint.Warn("EnemyPool 없음");
            return;
        }

        if (_mapManager == null)
        {
            CPrint.Warn("MapManager 없음");
            return;
        }

        if (_mapManager.SpawnPoints == null || _mapManager.SpawnPoints.Count == 0)
        {
            CPrint.Warn("SpawnPoint 없음");
            return;
        }

        if (_target == null)
        {
            CPrint.Warn("플레이어 Target 없음");
            return;
        }

        bool isRangedEnemy = Random.value < _rangedEnemyRate;

        CPrint.Log($"적 타입 결정 : {(isRangedEnemy ? "원거리" : "근거리")} " + $"(현재 원거리 비율 : {_rangedEnemyRate * 100f}%)");

        int spawnIndex = Random.Range(0, _mapManager.SpawnPoints.Count);

        GameObject enemy = isRangedEnemy ? _enemyPool.GetRangedEnemy() : _enemyPool.GetMeleeEnemy();

        if (enemy == null)
        {
            CPrint.Warn("사용 가능한 적이 없음");
            return;
        }

        if (!isRangedEnemy)
        {
            EnemyController controller = enemy.GetComponent<EnemyController>();

            if (controller != null)
            {
                controller.SetTarget(_target);
                controller.SetDamageMultiplier(_damageMultiplier);
            }
        }
        else
        {
            RangedEnemyController controller = enemy.GetComponent<RangedEnemyController>();

            if (controller != null)
            {
                controller.SetTarget(_target);
            }

            // 프리팹 안에서 비어있던 BulletPool 참조를 스폰 시점에 채워줌.
            EnemyPistol pistol = enemy.GetComponentInChildren<EnemyPistol>();

            if (pistol != null)
            {
                if (_rangedEnemyBulletPool != null)
                {
                    pistol.SetBulletPool(_rangedEnemyBulletPool);
                    pistol.SetDamageMultiplier(_damageMultiplier);
                }
                else
                {
                    CPrint.Warn("RangedEnemyBulletPool이 비어있음");
                }
            }
        }

        Transform spawnPoint = _mapManager.SpawnPoints[spawnIndex];

        enemy.transform.position = spawnPoint.position;
        enemy.transform.rotation = spawnPoint.rotation;

        Health health = enemy.GetComponent<Health>();

        if (health != null)
        {
            health.SetHealthMultiplier(_healthMultiplier);
            CPrint.Log($"적 체력 적용 : {health.MaxHealth}");
        }
        else
        {
            CPrint.Warn($"Health 없음 : {enemy.name}");
        }

        _spawnedEnemies.Add(enemy);

        RegisterDeathEvent(enemy);

        CPrint.Log($"적 생성 : {enemy.name}");
    }

    private void RegisterDeathEvent(GameObject enemy)
    {
        if (_deathHandlers.ContainsKey(enemy))
        {
            return;
        }

        Health health = enemy.GetComponent<Health>();

        if (health == null)
        {
            CPrint.Warn($"Health 없음 : {enemy.name}");
            return;
        }

        System.Action deathHandler = () => OnEnemyDied(enemy);

        _deathHandlers.Add(enemy, deathHandler);
        health.Died += deathHandler;

        if (_deathAnimationHandlers.ContainsKey(enemy))
        {
            return;
        }

        System.Action deathAnimationHandler = () => ReturnEnemyToPool(enemy);

        _deathAnimationHandlers.Add(enemy, deathAnimationHandler);

        EnemyController meleeController = enemy.GetComponent<EnemyController>();

        if (meleeController != null)
        {
            meleeController.DeathAnimationCompleted += deathAnimationHandler;
            return;
        }

        RangedEnemyController rangedController = enemy.GetComponent<RangedEnemyController>();

        if (rangedController != null)
        {
            rangedController.DeathAnimationCompleted += deathAnimationHandler;
            return;
        }

        CPrint.Warn($"EnemyController 없음 : {enemy.name}");
    }

    private void ReturnEnemyToPool(GameObject enemy)
    {
        if (enemy == null)
        {
            return;
        }

        CPrint.Log($"죽음 애니메이션 완료 : {enemy.name}");

        UnregisterDeathEvent(enemy);

        _enemyPool.Return(enemy);

        CPrint.Log($"EnemyPool 반환 : {enemy.name}");
    }

    private void OnEnemyDied(GameObject enemy)
    {
        if (!_spawnedEnemies.Contains(enemy))
        {
            return;
        }

        _spawnedEnemies.Remove(enemy);

        if (_currencyManager != null)
        {
            _currencyManager.AddCurrency(_enemyReward);
        }

        CPrint.Log($"적 사망 : {enemy.name}");
        CPrint.Log($"남은 적 : {_spawnedEnemies.Count}");
    }

    private void UnregisterDeathEvent(GameObject enemy)
    {
        if (_deathHandlers.TryGetValue(enemy, out System.Action deathHandler))
        {
            Health health = enemy.GetComponent<Health>();

            if (health != null)
            {
                health.Died -= deathHandler;
            }

            _deathHandlers.Remove(enemy);
        }

        if (_deathAnimationHandlers.TryGetValue(enemy, out System.Action deathAnimationHandler))
        {
            EnemyController meleeController = enemy.GetComponent<EnemyController>();

            if (meleeController != null)
            {
                meleeController.DeathAnimationCompleted -= deathAnimationHandler;
            }

            RangedEnemyController rangedController = enemy.GetComponent<RangedEnemyController>();

            if (rangedController != null)
            {
                rangedController.DeathAnimationCompleted -= deathAnimationHandler;
            }

            _deathAnimationHandlers.Remove(enemy);
        }
    }
}