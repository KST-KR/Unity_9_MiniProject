using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceUI : MonoBehaviour
{
    #region 인스펙터
    [Header("참조")]
    [SerializeField] private UpgradeManager _upgradeManager;

    [Header("주사위")]
    [SerializeField] private Image _diceImage;
    [SerializeField] private Sprite[] _diceSprites;
    [SerializeField] private TMP_Text _diceResultText;
    [SerializeField] private Button _rollButton;

    [Header("업그레이드")]
    [SerializeField] private GameObject _upgradePanel;
    [SerializeField] private Button[] _abilityButtons;
    [SerializeField] private TMP_Text[] _abilityNameTexts;
    [SerializeField] private Image[] _abilityIcons;
    #endregion

    #region 내부 변수
    private bool _isRolling;
    #endregion

    private void Awake()
    {
        for (int i = 0; i < _abilityButtons.Length; i++)
        {
            int index = i;

            _abilityButtons[i].onClick.AddListener(() =>
            {
                _upgradeManager.SelectAbility(index);
            });
        }

        _rollButton.onClick.AddListener(OnRollButtonClicked);
    }

    public void Show()
    {
        gameObject.SetActive(true);

        _upgradePanel.SetActive(false);

        _rollButton.gameObject.SetActive(true);
        _rollButton.interactable = true;

        _diceImage.gameObject.SetActive(true);
        _diceResultText.text = "D4 : ?";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Hide()
    {
        gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

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
            int randomResult = Random.Range(1, 5);

            ShowDiceResult(randomResult);

            yield return new WaitForSecondsRealtime(0.1f);
        }

        // 실제 D4 결과 생성
        _upgradeManager.RollDice();

        _isRolling = false;
    }

    public void ShowDiceResult(int result)
    {
        _diceResultText.text = $"D4 : {result}";

        if (_diceSprites == null || _diceSprites.Length < 4)
        {
            return;
        }

        _diceImage.sprite = _diceSprites[result - 1];
    }

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
}