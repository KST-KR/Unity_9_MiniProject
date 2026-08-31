using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    #region 인스펙터
    [Header("근거리 적")]
    [SerializeField] private GameObject _meleeEnemyPrefab;
    [SerializeField] private int _meleePoolSize = 10;

    [Header("원거리 적")]
    [SerializeField] private GameObject _rangedEnemyPrefab;
    [SerializeField] private int _rangedPoolSize = 10;
    #endregion

    #region 내부 변수
    private List<GameObject> _meleeEnemies = new List<GameObject>();
    private List<GameObject> _rangedEnemies = new List<GameObject>();
    #endregion

    private void Awake()
    {
        CreatePool(_meleeEnemyPrefab, _meleePoolSize, _meleeEnemies);
        CreatePool(_rangedEnemyPrefab, _rangedPoolSize, _rangedEnemies);
    }

    private void CreatePool(GameObject prefab, int poolSize, List<GameObject> pool)
    {
        if (prefab == null)
        {
            CPrint.Warn("프리펩 없음");
            return;
        }

        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(prefab, transform);

            enemy.SetActive(false);

            pool.Add(enemy);
        }
    }

    public GameObject GetMeleeEnemy()
    {
        return GetEnemy(_meleeEnemies);
    }

    public GameObject GetRangedEnemy()
    {
        return GetEnemy(_rangedEnemies);
    }

    private GameObject GetEnemy(List<GameObject> pool)
    {
        foreach (GameObject enemy in pool)
        {
            if (enemy == null)
            {
                continue;
            }

            if (!enemy.activeSelf)
            {
                enemy.SetActive(true);
                return enemy;
            }
        }

        return null;
    }

    public void Return(GameObject enemy)
    {
        if (enemy == null)
        {
            CPrint.Warn("적이 비어있음.");
            return;
        }

        enemy.SetActive(false);
    }
}
