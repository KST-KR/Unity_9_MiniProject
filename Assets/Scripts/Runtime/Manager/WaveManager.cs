using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    #region 인스펙터
    [Header("웨이브")]
    [SerializeField] private EnemySpawner _enemySpawner;
    [SerializeField] private int _startEnemyCount = 5;
    [SerializeField] private int _enemyIncrease = 2;

    [Header("웨이브 간격")]
    [SerializeField] private float _waveInterval = 3f;
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

    private void Start()
    {
        StartNextWave();
    }

    private void StartNextWave()
    {
        _currentWave++;
        _currentEnemyCount = _startEnemyCount + ((_currentWave - 1) * _enemyIncrease);

        StartCoroutine(WaveRoutine());
    }

    private IEnumerator WaveRoutine()
    {
        _isWaveRunning = true;

        CPrint.Log($"===== Wave {_currentWave} 시작 =====");
        CPrint.Log($"적 생성 수 : {_currentEnemyCount}");

        _enemySpawner.StartSpawn(_currentEnemyCount);

        // 모든 적 생성이 끝날 때까지 대기
        yield return new WaitUntil(() => _enemySpawner.SpawnedEnemies.Count >= _currentEnemyCount);

        CPrint.Log($"Wave {_currentWave} 적 생성 완료");

        // 모든 적이 죽을 때까지 대기
        yield return new WaitUntil(() => _enemySpawner.SpawnedEnemies.Count == 0);

        _isWaveRunning = false;

        CPrint.Log($"===== Wave {_currentWave} 종료 =====");

        // 다음 웨이브 시작 전 대기
        yield return new WaitForSeconds(_waveInterval);

        StartNextWave();
    }
}