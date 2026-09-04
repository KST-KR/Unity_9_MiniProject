using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    #region 인스펙터
    [Header("게임 오버 패널")]
    [SerializeField] private GameObject _gameOverPanel;

    [Header("버튼")]
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _resultButton;
    #endregion

    private void Awake()
    {
        _restartButton.onClick.AddListener(RestartGame);
        _resultButton.onClick.AddListener(LoadResultScene);

        Hide();
    }

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            CPrint.Warn("GameManager가 없습니다.");
            return;
        }

        GameManager.Instance.SetGameOverUI(this);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ClearGameOverUI(this);
        }
    }

    public void Show()
    {
        _gameOverPanel.SetActive(true);
    }

    public void Hide()
    {
        _gameOverPanel.SetActive(false);
    }

    private void RestartGame()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        GameManager.Instance.RestartGame();
    }

    private void LoadResultScene()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        GameManager.Instance.LoadResultScene();
    }
}