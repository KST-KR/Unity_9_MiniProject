using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveEndUI : MonoBehaviour
{
    #region 인스펙터
    [Header("게임")]
    [SerializeField] private GameManager _gameManager;

    [Header("웨이브")]
    [SerializeField] private WaveManager _waveManager;
    [SerializeField] private TMP_Text _waveText;

    [Header("주사위")]
    [SerializeField] private UpgradeManager _upgradeManager;

    [Header("웨이브 종료 패널")]
    [SerializeField] private GameObject _waveEndPanel;
    [SerializeField] private Button _diceButton;
    #endregion

    #region 내부 변수
    private bool _isUpgradeCompleted;
    #endregion

    private void Start()
    {
        _waveManager.WaveEnded += Show;
        _upgradeManager.UpgradeCompleted += OnUpgradeCompleted;
    }

    private void OnDestroy()
    {
        _waveManager.WaveEnded -= Show;
        _upgradeManager.UpgradeCompleted -= OnUpgradeCompleted;
    }

    private void Show()
    {
        _waveEndPanel.SetActive(true);

        _waveText.text = $"Wave {_waveManager.CurrentWave} 종료";

        _diceButton.interactable = !_isUpgradeCompleted;

        _gameManager.PauseGame();

        CPrint.Log("웨이브 종료 UI 표시");
    }

    private void OnUpgradeCompleted()
    {
        _isUpgradeCompleted = true;

        _waveEndPanel.SetActive(true);

        _diceButton.interactable = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        CPrint.Log("업그레이드 완료 → 웨이브 종료 메뉴 표시");
    }

    public void OpenDice()
    {
        _waveEndPanel.SetActive(false);

        _upgradeManager.StartUpgrade();
    }

    public void NextWave()
    {
        _isUpgradeCompleted = false;

        _waveEndPanel.SetActive(false);

        _gameManager.ResumeGame();

        _waveManager.StartNextWave();

        CPrint.Log("다음 웨이브 시작");
    }
}