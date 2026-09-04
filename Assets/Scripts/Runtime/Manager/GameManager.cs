using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region 싱글톤
    public static GameManager Instance { get; private set; }
    #endregion

    #region 내부 변수
    private int _killCount;
    private int _totalEarnedCurrency;
    private int _finalWave;

    private GameOverUI _gameOverUI;
    #endregion

    #region 프로퍼티
    public bool IsPaused { get; private set; }
    public bool IsGameOver { get; private set; }

    public int KillCount => _killCount;
    public int TotalEarnedCurrency => _totalEarnedCurrency;
    public int FinalWave => _finalWave;
    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void SetGameOverUI(GameOverUI gameOverUI)
    {
        _gameOverUI = gameOverUI;

        CPrint.Log("GameOverUI 연결 완료");
    }

    public void ClearGameOverUI(GameOverUI gameOverUI)
    {
        if (_gameOverUI == gameOverUI)
        {
            _gameOverUI = null;
        }
    }

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        CPrint.Log("게임 일시정지");
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        CPrint.Log("게임 재개");
    }

    public void GameOver()
    {
        if (IsGameOver)
        {
            return;
        }

        IsGameOver = true;

        if (WaveManager.Instance != null)
        {
            _finalWave = WaveManager.Instance.CurrentWave;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (_gameOverUI != null)
        {
            _gameOverUI.Show();
        }

        CPrint.Log("게임 오버");
        CPrint.Log($"최종 웨이브 : {_finalWave}");
        CPrint.Log($"최종 처치 수 : {_killCount}");
        CPrint.Log($"총 획득 재화 : {_totalEarnedCurrency}");
    }

    public void RecordKill(int reward)
    {
        _killCount++;
        _totalEarnedCurrency += reward;

        CPrint.Log($"처치 수 : {_killCount}, 총 획득 재화 : {_totalEarnedCurrency}");
    }

    public void RestartGame()
    {
        StartNewGame();

        SceneManager.LoadScene("GameScene");

        CPrint.Log("게임 재시작");
    }

    public void LoadResultScene()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("ResultScene");

        CPrint.Log("ResultScene으로 이동");
    }

    public void LoadMainScene()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainScene");

        CPrint.Log("MainScene으로 이동");
    }

    public void StartNewGame()
    {
        IsPaused = false;
        IsGameOver = false;

        _killCount = 0;
        _totalEarnedCurrency = 0;
        _finalWave = 0;

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        CPrint.Log("새 게임 시작");
    }
}