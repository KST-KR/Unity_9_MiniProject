using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUpgradeUI : MonoBehaviour
{
    #region 인스펙터
    [Header("참조")]
    [SerializeField] private UpgradeManager _upgradeManager;

    [Header("주사위")]
    [SerializeField] private DiceManager _diceManager;
    [SerializeField] private DiceResultSlotUI[] _diceSlots;

    [Header("주사위 이미지")]
    [SerializeField] private Sprite[] _d4Sprites;
    [SerializeField] private Sprite[] _d6Sprites;

    [Header("결과")]
    [SerializeField] private TMP_Text _diceResultText;
    [SerializeField] private Button _rollButton;

    [Header("업그레이드")]
    [SerializeField] private GameObject _upgradePanel;
    [SerializeField] private Button[] _abilityButtons;
    [SerializeField] private TMP_Text[] _abilityNameTexts;
    [SerializeField] private Image[] _abilityIcons;

    [Header("뒤로가기")]
    [SerializeField] private Button _backButton;

    [Header("패널")]
    [SerializeField] private GameObject _waveEndPanel;
    #endregion

    #region 내부 변수
    private bool _isRolling;
    #endregion

    private void Awake()
    {
        if (_diceManager == null)
        {
            _diceManager = FindFirstObjectByType<DiceManager>();
        }

        for (int i = 0; i < _abilityButtons.Length; i++)
        {
            int index = i;

            _abilityButtons[i].onClick.AddListener(() =>
            {
                _upgradeManager.SelectAbility(index);
            });
        }

        _rollButton.onClick.AddListener(OnRollButtonClicked);

        if (_backButton != null)
        {
            _backButton.onClick.AddListener(Close);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        _isRolling = false;
    }

    #region 패널
    public void Show()
    {
        gameObject.SetActive(true);

        _upgradePanel.SetActive(false);

        _rollButton.gameObject.SetActive(true);
        _rollButton.interactable = true;

        _diceResultText.text = "주사위 : ?";

        ShowDiceSlots();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ShowDiceSlots()
    {
        if (_diceManager == null)
        {
            return;
        }

        IReadOnlyList<Dice> dices = _diceManager.Dices;

        for (int i = 0; i < _diceSlots.Length; i++)
        {
            if (_diceSlots[i] == null)
            {
                continue;
            }

            if (i >= dices.Count)
            {
                _diceSlots[i].Hide();
                continue;
            }

            _diceSlots[i].Show(dices[i], _d4Sprites, _d6Sprites);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    #endregion

    #region 주사위 굴리기
    private void OnRollButtonClicked()
    {
        if (_isRolling)
        {
            return;
        }

        StartCoroutine(RollDiceRoutine());
    }

    private IEnumerator RollDiceRoutine()
    {
        _isRolling = true;

        _rollButton.interactable = false;
        _upgradePanel.SetActive(false);

        // 주사위 굴리는 연출
        for (int i = 0; i < 8; i++)
        {
            ShowRandomDiceResults();

            yield return new WaitForSecondsRealtime(0.1f);
        }

        // 실제 주사위 결과 생성
        _upgradeManager.RollDice();

        // 실제 결과 표시
        ShowDiceSlots();

        _isRolling = false;
    }

    private void ShowRandomDiceResults()
    {
        if (_diceManager == null)
        {
            return;
        }

        IReadOnlyList<Dice> dices = _diceManager.Dices;

        for (int i = 0; i < _diceSlots.Length; i++)
        {
            if (_diceSlots[i] == null)
            {
                continue;
            }

            if (i >= dices.Count)
            {
                _diceSlots[i].Hide();
                continue;
            }

            Dice dice = dices[i];

            int randomResult = Random.Range(dice.MinValue, dice.DiceSides + 1);

            _diceSlots[i].ShowResult(randomResult, dice, _d4Sprites, _d6Sprites);
        }
    }

    public void ShowDiceResult(int result)
    {
        _diceResultText.text = $"총 결과 : {result}";
    }
    #endregion

    #region 어빌리티
    public void ShowAbilities(IReadOnlyList<Ability> abilities)
    {
        _upgradePanel.SetActive(true);

        for (int i = 0; i < _abilityButtons.Length; i++)
        {
            if (i >= abilities.Count)
            {
                _abilityButtons[i].gameObject.SetActive(false);
                continue;
            }

            _abilityButtons[i].gameObject.SetActive(true);

            _abilityNameTexts[i].text = abilities[i].AbilityName;
            _abilityIcons[i].sprite = abilities[i].Icon;
        }
    }
    #endregion

    private void OnDestroy()
    {
        if (_backButton != null)
        {
            _backButton.onClick.RemoveListener(Close);
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);

        if (_waveEndPanel != null)
        {
            _waveEndPanel.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        CPrint.Log("능력 강화 패널 닫기 → 웨이브 종료 메뉴");
    }
}