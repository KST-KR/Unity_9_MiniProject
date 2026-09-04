using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    #region 인스펙터
    [Header("버튼")]
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _exitButton;
    #endregion

    private void Awake()
    {
        _startButton.onClick.AddListener(StartGame);
        _exitButton.onClick.AddListener(ExitGame);
    }

    private void StartGame()
    {
        SceneManager.LoadScene("GameScene");

        CPrint.Log("게임 시작");
    }

    private void ExitGame()
    {
        Application.Quit();

        CPrint.Log("게임 종료");
    }
}
