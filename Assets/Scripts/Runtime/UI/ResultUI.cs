using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    #region 인스펙터
    [Header("결과 텍스트")]
    [SerializeField] private TMP_Text _finalWaveText;
    [SerializeField] private TMP_Text _killCountText;
    [SerializeField] private TMP_Text _earnedCurrencyText;

    [Header("버튼")]
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _mainMenuButton;
    #endregion

    private void Awake()
    {
        _restartButton.onClick.AddListener(RestartGame);
        _mainMenuButton.onClick.AddListener(LoadMainScene);
    }

    void Start()
    {
        if (GameManager.Instance == null)
        {
            CPrint.Warn("GameManager가 없음.");
            return;
        }

        ShowResult();
    }

    private void ShowResult()
    {
        _finalWaveText.text = $"최종 웨이브 : {GameManager.Instance.FinalWave}";
        _killCountText.text = $"처치한 적 : {GameManager.Instance.KillCount}";
        _earnedCurrencyText.text = $"획득 재화 : {GameManager.Instance.TotalEarnedCurrency}";

        CPrint.Log($"결과 화면 - 웨이브 : {GameManager.Instance.FinalWave}");
        CPrint.Log($"결과 화면 - 처치 수 : {GameManager.Instance.KillCount}");
        CPrint.Log($"결과 화면 - 획득 재화 : {GameManager.Instance.TotalEarnedCurrency}");
    }

    private void RestartGame()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        GameManager.Instance.RestartGame();
    }

    private void LoadMainScene()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        GameManager.Instance.LoadMainScene();
    }
}
