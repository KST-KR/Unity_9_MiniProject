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
    #endregion

    #region 내부 변수
    private List<GameObject> _spawnedEnemies = new List<GameObject>();
    private Dictionary<GameObject, System.Action> _deathHandlers = new Dictionary<GameObject, System.Action>();
    #endregion

    #region 프로퍼티
    public List<GameObject> SpawnedEnemies => _spawnedEnemies;
    #endregion

    public void StartSpawn(int count)
    {
        StartCoroutine(SpawnEnemies(count));
    }

    private IEnumerator SpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(_spawnInterval);
        }
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

        int enemyType = Random.Range(0, 2);
        int spawnIndex = Random.Range(0, _mapManager.SpawnPoints.Count);

        GameObject enemy = enemyType == 0 ? _enemyPool.GetMeleeEnemy() : _enemyPool.GetRangedEnemy();

        if (enemy == null)
        {
            CPrint.Warn("사용 가능한 적이 없음");
            return;
        }

        if (enemyType == 0)
        {
            EnemyController controller = enemy.GetComponent<EnemyController>();

            if (controller != null)
            {
                controller.SetTarget(_target);
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

        System.Action handler = () => OnEnemyDied(enemy);

        _deathHandlers.Add(enemy, handler);
        health.Died += handler;
    }

    private void OnEnemyDied(GameObject enemy)
    {
        if (!_spawnedEnemies.Contains(enemy))
        {
            return;
        }

        _spawnedEnemies.Remove(enemy);

        CPrint.Log($"적 사망 : {enemy.name}");
        CPrint.Log($"남은 적 : {_spawnedEnemies.Count}");

        UnregisterDeathEvent(enemy);
    }

    private void UnregisterDeathEvent(GameObject enemy)
    {
        if (!_deathHandlers.TryGetValue(enemy, out System.Action handler))
        {
            return;
        }

        Health health = enemy.GetComponent<Health>();

        if (health != null)
        {
            health.Died -= handler;
        }

        _deathHandlers.Remove(enemy);
    }
}