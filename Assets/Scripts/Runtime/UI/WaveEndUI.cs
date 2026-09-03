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

    [Header("재화")]
    [SerializeField] private CurrencyManager _currencyManager;
    [SerializeField] private TMP_Text _currencyText;

    [Header("주사위")]
    [SerializeField] private UpgradeManager _upgradeManager;
    [SerializeField] private DiceUpgradeUI _diceUpgradeUI;

    [Header("능력 강화")]
    [SerializeField] private AbilityUpgradeUI _abilityUpgradeUI;

    [Header("상점")]
    [SerializeField] private GameObject _shopPanel;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _shopBackButton;

    [Header("웨이브 종료 패널")]
    [SerializeField] private GameObject _waveEndPanel;
    [SerializeField] private Button _diceButton;
    [SerializeField] private Button _abilityButton;
    #endregion

    #region 내부 변수
    private bool _isUpgradeCompleted;
    #endregion

    private void Start()
    {
        _currencyManager = CurrencyManager.Instance;

        _waveManager.WaveEnded += Show;
        _upgradeManager.UpgradeCompleted += OnUpgradeCompleted;

        _shopButton.onClick.AddListener(OpenShop);
        _shopBackButton.onClick.AddListener(CloseShop);

        if (_diceButton != null)
        {
            _diceButton.onClick.AddListener(OpenDiceUpgrade);
        }

        if (_abilityButton != null)
        {
            _abilityButton.onClick.AddListener(OpenAbilityUpgrade);
        }

        if (_currencyManager != null)
        {
            _currencyManager.CurrencyChanged += UpdateCurrencyText;
            UpdateCurrencyText(_currencyManager.CurrentCurrency);
        }
    }

    private void OnDestroy()
    {
        _waveManager.WaveEnded -= Show;
        _upgradeManager.UpgradeCompleted -= OnUpgradeCompleted;

        _shopButton.onClick.RemoveListener(OpenShop);
        _shopBackButton.onClick.RemoveListener(CloseShop);

        if (_diceButton != null)
        {
            _diceButton.onClick.RemoveListener(OpenDiceUpgrade);
        }

        if (_abilityButton != null)
        {
            _abilityButton.onClick.RemoveListener(OpenAbilityUpgrade);
        }

        if (_currencyManager != null)
        {
            _currencyManager.CurrencyChanged -= UpdateCurrencyText;
        }
    }

    private void Show()
    {
        _waveEndPanel.SetActive(true);

        _waveText.text = $"Wave {_waveManager.CurrentWave} 종료";

        // 이번 웨이브에 어빌리티 강화를 했으면 버튼 비활성화
        if (_abilityButton != null)
        {
            _abilityButton.interactable = !_isUpgradeCompleted;
        }

        if (_currencyManager != null)
        {
            UpdateCurrencyText(_currencyManager.CurrentCurrency);
        }

        _gameManager.PauseGame();

        CPrint.Log("웨이브 종료 UI 표시");
    }

    private void OnUpgradeCompleted()
    {
        _isUpgradeCompleted = true;

        _waveEndPanel.SetActive(true);

        if (_abilityButton != null)
        {
            _abilityButton.interactable = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        CPrint.Log("어빌리티 강화 완료 → 웨이브 종료 메뉴 표시");
    }

    private void UpdateCurrencyText(int currency)
    {
        if (_currencyText == null)
        {
            return;
        }

        _currencyText.text = $"재화 : {currency}";
    }

    #region 강화 패널
    public void OpenDiceUpgrade()
    {
        _waveEndPanel.SetActive(false);

        if (_diceUpgradeUI != null)
        {
            _diceUpgradeUI.Show();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        CPrint.Log("주사위 강화 패널 열기");
    }

    public void OpenAbilityUpgrade()
    {
        CPrint.Log("능력 강화 버튼 클릭");

        if (_abilityUpgradeUI == null)
        {
            CPrint.Log("ERROR : AbilityUpgradeUI가 연결되지 않았습니다.");
            return;
        }

        CPrint.Log("AbilityUpgradeUI.Show() 실행");

        _waveEndPanel.SetActive(false);

        _abilityUpgradeUI.Show();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    #endregion

    #region 상점
    public void OpenShop()
    {
        _waveEndPanel.SetActive(false);
        _shopPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        CPrint.Log("상점 패널 열기");
    }

    public void CloseShop()
    {
        _shopPanel.SetActive(false);
        _waveEndPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        CPrint.Log("상점 닫기 → 웨이브 종료 메뉴");
    }
    #endregion

    public void NextWave()
    {
        _isUpgradeCompleted = false;

        _waveEndPanel.SetActive(false);

        _gameManager.ResumeGame();

        _waveManager.StartNextWave();

        CPrint.Log("다음 웨이브 시작");
    }
}