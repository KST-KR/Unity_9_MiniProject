using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceSlotUI : MonoBehaviour
{
    #region 인스펙터
    [Header("주사위")]
    [SerializeField] private Image _diceImage;
    [SerializeField] private TMP_Text _diceNameText;

    [Header("D6 업그레이드")]
    [SerializeField] private TMP_Text _diceTypeUpgradeNameText;
    [SerializeField] private TMP_Text _diceTypeUpgradeCountText;
    [SerializeField] private Button _diceTypeUpgradeButton;

    [Header("범위 상향")]
    [SerializeField] private TMP_Text _rangeUpgradeNameText;
    [SerializeField] private TMP_Text _rangeUpgradeCountText;
    [SerializeField] private Button _rangeUpgradeButton;
    #endregion

    #region 내부 변수
    private int _diceIndex;
    private DiceUpgradeUI _diceUpgradeUI;
    #endregion

    private void Awake()
    {
        _diceTypeUpgradeButton.onClick.AddListener(OnDiceTypeUpgradeClicked);

        _rangeUpgradeButton.onClick.AddListener(OnRangeUpgradeClicked);
    }

    private void OnDestroy()
    {
        _diceTypeUpgradeButton.onClick.RemoveListener(OnDiceTypeUpgradeClicked);

        _rangeUpgradeButton.onClick.RemoveListener(OnRangeUpgradeClicked);
    }

    public void Initialize(int index, DiceUpgradeUI diceUpgradeUI)
    {
        _diceIndex = index;
        _diceUpgradeUI = diceUpgradeUI;
    }

    public void Refresh(Dice dice, DiceManager diceManager, Sprite[] d4Sprites, Sprite[] d6Sprites)
    {
        if (dice == null || diceManager == null)
        {
            return;
        }

        _diceNameText.text = $"D{dice.DiceSides}";

        UpdateDiceImage(dice, d4Sprites, d6Sprites);

        // D6 업그레이드
        _diceTypeUpgradeNameText.text = "D6 업그레이드";

        _diceTypeUpgradeCountText.text = $"가능 횟수 : {diceManager.DiceTypeUpgradeCount}";

        _diceTypeUpgradeButton.interactable = dice.CanUpgradeDiceType() && diceManager.DiceTypeUpgradeCount > 0;

        // 범위 향상
        _rangeUpgradeNameText.text = "범위 향상";

        _rangeUpgradeCountText.text = $"가능 횟수 : {diceManager.MinValueUpgradeCount}";

        _rangeUpgradeButton.interactable = dice.CanIncreaseMinValue() && diceManager.MinValueUpgradeCount > 0;
    }

    private void UpdateDiceImage(Dice dice, Sprite[] d4Sprites, Sprite[] d6Sprites)
    {
        Sprite[] sprites;

        if (dice.DiceSides == 4)
        {
            sprites = d4Sprites;
        }
        else
        {
            sprites = d6Sprites;
        }

        if (sprites == null)
        {
            return;
        }

        // 아직 굴리지 않은 경우 기본적으로 첫 번째 이미지
        int result = dice.CurrentResult;

        if (result <= 0)
        {
            result = dice.MinValue;
        }

        if (result > sprites.Length)
        {
            return;
        }

        _diceImage.sprite = sprites[result - 1];
    }

    public void ShowResult(Dice dice, Sprite[] d4Sprites, Sprite[] d6Sprites)
    {
        if (dice == null)
        {
            return;
        }

        UpdateDiceImage(dice, d4Sprites, d6Sprites);
    }

    private void OnDiceTypeUpgradeClicked()
    {
        _diceUpgradeUI.UpgradeDiceType(_diceIndex);
    }

    private void OnRangeUpgradeClicked()
    {
        _diceUpgradeUI.IncreaseMinValue(_diceIndex);
    }
}