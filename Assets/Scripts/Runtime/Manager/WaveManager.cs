using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    #region 싱글톤
    public static WaveManager Instance { get; private set; }
    #endregion

    #region 인스펙터
    [Header("웨이브")]
    [SerializeField] private EnemySpawner _enemySpawner;
    [SerializeField] private int _startEnemyCount = 5;
    [SerializeField] private int _enemyIncrease = 2;

    [Header("적 비율")]
    [SerializeField] private float _startRangedEnemyRate = 0.2f;
    [SerializeField] private float _rangedEnemyRateIncrease = 0.05f;
    [SerializeField] private float _maxRangedEnemyRate = 0.5f;

    [Header("적 체력")]
    [SerializeField] private float _startHealthMultiplier = 1f;
    [SerializeField] private float _healthMultiplierIncrease = 0.1f;

    [Header("적 공격력")]
    [SerializeField] private float _startDamageMultiplier = 1f;
    [SerializeField] private float _damageMultiplierIncrease = 0.1f;
    #endregion

    #region 내부 변수
    private int _currentWave;
    private int _currentEnemyCount;

    private bool _isWaveRunning;
    #endregion

    #region 프로퍼티
    public int CurrentWave => _currentWave;
    public bool IsWaveRunning => _isWaveRunning;
    #endregion

    #region 이벤트
    public event System.Action<int> WaveChanged;
    public event System.Action WaveEnded;
    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        CPrint.Log("WaveManager Awake 실행");
    }

    private void Start()
    {
        CPrint.Log("WaveManager Start 실행");

        StartNextWave();
    }

    public void StartNextWave()
    {
        _currentWave++;

        _currentEnemyCount = _startEnemyCount + ((_currentWave - 1) * _enemyIncrease);

        float rangedEnemyRate = GetRangedEnemyRate();
        float healthMultiplier = GetHealthMultiplier();
        float damageMultiplier = GetDamageMultiplier();

        _enemySpawner.SetRangedEnemyRate(rangedEnemyRate);
        _enemySpawner.SetHealthMultiplier(healthMultiplier);
        _enemySpawner.SetDamageMultiplier(damageMultiplier);

        CPrint.Log($"===== Wave {_currentWave} =====");
        CPrint.Log($"적 생성 수 : {_currentEnemyCount}");
        CPrint.Log($"원거리 적 비율 : {rangedEnemyRate * 100f}%");
        CPrint.Log($"적 체력 배율 : {healthMultiplier * 100f}%");
        CPrint.Log($"적 공격력 배율 : {damageMultiplier * 100f}%");

        WaveChanged?.Invoke(_currentWave);

        StartCoroutine(WaveRoutine());
    }

    private float GetRangedEnemyRate()
    {
        float rate = _startRangedEnemyRate + ((_currentWave - 1) * _rangedEnemyRateIncrease);

        return Mathf.Min(rate, _maxRangedEnemyRate);
    }

    private float GetDamageMultiplier()
    {
        return _startDamageMultiplier + ((_currentWave - 1) * _damageMultiplierIncrease);
    }

    private IEnumerator WaveRoutine()
    {
        _isWaveRunning = true;

        CPrint.Log($"===== Wave {_currentWave} 시작 =====");
        CPrint.Log($"적 생성 수 : {_currentEnemyCount}");

        _enemySpawner.StartSpawn(_currentEnemyCount);

        // 모든 적 생성이 끝날 때까지 대기
        yield return new WaitUntil(() => !_enemySpawner.IsSpawning);

        CPrint.Log($"Wave {_currentWave} 적 생성 완료");

        // 모든 적이 죽을 때까지 대기
        yield return new WaitUntil(() => _enemySpawner.SpawnedEnemies.Count == 0);

        _isWaveRunning = false;

        CPrint.Log($"===== Wave {_currentWave} 종료 =====");

        WaveEnded?.Invoke();
    }

    private float GetHealthMultiplier()
    {
        return _startHealthMultiplier + ((_currentWave - 1) * _healthMultiplierIncrease);
    }
}