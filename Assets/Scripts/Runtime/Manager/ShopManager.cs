using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    #region 인스펙터
    [Header("재화")]
    [SerializeField] private CurrencyManager _currencyManager;

    [Header("주사위")]
    [SerializeField] private DiceManager _diceManager;

    [Header("웨이브")]
    [SerializeField] private WaveManager _waveManager;

    [Header("UI")]
    [SerializeField] private TMP_Text _currencyText;

    [Header("상품")]
    [SerializeField] private ShopItem[] _shopItems;

    [SerializeField] private Image[] _itemIcons;
    [SerializeField] private TMP_Text[] _itemNameTexts;
    [SerializeField] private TMP_Text[] _itemDescriptionTexts;
    [SerializeField] private TMP_Text[] _itemPriceTexts;
    [SerializeField] private Button[] _buyButtons;
    [SerializeField] private TMP_Text[] _buyButtonTexts;
    #endregion

    #region 내부 변수
    private bool[] _isPurchased;
    private int[] _purchaseCounts;
    #endregion

    private void Awake()
    {
        _isPurchased = new bool[_shopItems.Length];
        _purchaseCounts = new int[_shopItems.Length];

        for (int i = 0; i < _buyButtons.Length; i++)
        {
            int index = i;

            _buyButtons[i].onClick.AddListener(() =>
            {
                BuyItem(index);
            });
        }
    }

    private void Start()
    {
        _diceManager = DiceManager.Instance;
        _currencyManager = CurrencyManager.Instance;
        _waveManager = WaveManager.Instance;

        if (_waveManager != null)
        {
            _waveManager.WaveChanged += ResetShop;
        }

        if (_currencyManager != null)
        {
            _currencyManager.CurrencyChanged += UpdateCurrencyText;
            UpdateCurrencyText(_currencyManager.CurrentCurrency);
        }

        SetupShop();

        if (_waveManager != null)
        {
            ResetShop(_waveManager.CurrentWave);
        }
    }

    private void OnDestroy()
    {
        if (_waveManager != null)
        {
            _waveManager.WaveChanged -= ResetShop;
        }

        if (_currencyManager != null)
        {
            _currencyManager.CurrencyChanged -= UpdateCurrencyText;
        }
    }

    #region 상점 설정
    private void SetupShop()
    {
        for (int i = 0; i < _buyButtons.Length && i < _shopItems.Length; i++)
        {
            if (i >= _itemIcons.Length ||
                i >= _itemNameTexts.Length ||
                i >= _itemDescriptionTexts.Length ||
                i >= _itemPriceTexts.Length ||
                i >= _buyButtons.Length)
            {
                continue;
            }

            ShopItem item = _shopItems[i];

            if (item == null)
            {
                continue;
            }

            _itemIcons[i].sprite = item.Icon;
            _itemNameTexts[i].text = item.ItemName;
            _itemDescriptionTexts[i].text = item.Description;

            UpdateItemPriceText(i);
        }
    }
    #endregion

    #region 구매
    private void BuyItem(int index)
    {
        if (index < 0 || index >= _shopItems.Length)
        {
            return;
        }

        if (_isPurchased[index])
        {
            CPrint.Log($"이번 웨이브에 이미 구매한 상품 : " + $"{_shopItems[index].ItemName}");

            return;
        }

        ShopItem item = _shopItems[index];

        if (item == null)
        {
            return;
        }

        if (_currencyManager == null || _diceManager == null)
        {
            return;
        }

        if (!CanPurchase(item))
        {
            CPrint.Log($"구매할 수 없는 상품 : {item.ItemName}");
            return;
        }

        int currentPrice = GetCurrentPrice(index);

        bool spent = _currencyManager.TrySpendCurrency(currentPrice);

        if (!spent)
        {
            CPrint.Log($"상품 구매 실패 : {item.ItemName}");
            return;
        }

        ApplyItem(item);

        _isPurchased[index] = true;
        _purchaseCounts[index]++;

        // 구매 후 다음 가격 표시
        UpdateItemPriceText(index);

        UpdateAllBuyButtonStates();

        CPrint.Log($"상품 구매 완료 : {item.ItemName}, " + $"구매 가격 : {currentPrice}, " + $"다음 가격 : {GetCurrentPrice(index)}");
    }
    #endregion

    #region 가격
    private int GetCurrentPrice(int index)
    {
        ShopItem item = _shopItems[index];

        return item.Price + (item.PriceIncrease * _purchaseCounts[index]);
    }

    private void UpdateItemPriceText(int index)
    {
        if (index < 0 || index >= _shopItems.Length || index >= _itemPriceTexts.Length)
        {
            return;
        }

        ShopItem item = _shopItems[index];

        if (item == null)
        {
            return;
        }

        int price = GetCurrentPrice(index);

        _itemPriceTexts[index].text = $"{price}";
    }
    #endregion

    #region 구매 가능 여부
    private bool CanPurchase(ShopItem item)
    {
        switch (item.Type)
        {
            case ShopItemType.DiceCount:
                return !IsDiceCountMax();

            case ShopItemType.DiceType:
                return !IsDiceTypeMax();

            case ShopItemType.DiceMinValue:
                return !IsDiceMinValueMax();

            case ShopItemType.RerollCount:
                return true;

            default:
                return false;
        }
    }
    #endregion

    #region 상품 적용
    private void ApplyItem(ShopItem item)
    {
        switch (item.Type)
        {
            case ShopItemType.DiceCount:
                for (int i = 0; i < item.Value; i++)
                {
                    _diceManager.AddDice();
                }
                break;

            case ShopItemType.DiceType:
                _diceManager.AddDiceTypeUpgradeCount(item.Value);
                break;

            case ShopItemType.DiceMinValue:
                _diceManager.AddMinValueUpgradeCount(item.Value);
                break;

            case ShopItemType.RerollCount:
                _diceManager.AddRerollCount(item.Value);
                break;
        }
    }
    #endregion

    #region 웨이브
    private void ResetShop(int wave)
    {
        for (int i = 0; i < _isPurchased.Length; i++)
        {
            _isPurchased[i] = false;
        }

        for (int i = 0; i < _shopItems.Length; i++)
        {
            UpdateItemPriceText(i);
        }

        UpdateAllBuyButtonStates();

        CPrint.Log($"Wave {wave} 시작 → 상점 구매 상태 초기화");
    }
    #endregion

    #region 재화
    private void UpdateCurrencyText(int currency)
    {
        if (_currencyText == null)
        {
            return;
        }

        _currencyText.text = $"재화 : {currency}";
    }
    #endregion

    #region 구매 상태
    private bool IsDiceCountMax()
    {
        return _diceManager.DiceCount >= DiceManager.MaxDiceCount;
    }

    private bool IsDiceTypeMax()
    {
        if (_diceManager.DiceCount < DiceManager.MaxDiceCount)
        {
            return false;
        }

        for (int i = 0; i < _diceManager.Dices.Count; i++)
        {
            if (_diceManager.Dices[i].CanUpgradeDiceType())
            {
                return false;
            }
        }

        return true;
    }

    private bool IsDiceMinValueMax()
    {
        if (_diceManager.DiceCount == 0)
        {
            return true;
        }

        for (int i = 0; i < _diceManager.Dices.Count; i++)
        {
            if (_diceManager.Dices[i].CanIncreaseMinValue())
            {
                return false;
            }
        }

        return true;
    }

    private void UpdateBuyButtonState(int index)
    {
        if (index < 0 || 
            index >= _shopItems.Length || 
            index >= _buyButtons.Length || 
            index >= _buyButtonTexts.Length)
        {
            return;
        }

        ShopItem item = _shopItems[index];

        if (item == null)
        {
            return;
        }

        bool isMax = false;

        switch (item.Type)
        {
            case ShopItemType.DiceCount:
                isMax = IsDiceCountMax();
                break;

            case ShopItemType.DiceType:
                isMax = IsDiceTypeMax();
                break;

            case ShopItemType.DiceMinValue:
                isMax = IsDiceMinValueMax();
                break;
        }

        if (isMax)
        {
            _buyButtons[index].interactable = false;
            _buyButtonTexts[index].text = "최대 개수";
            return;
        }

        if (_isPurchased[index])
        {
            _buyButtons[index].interactable = false;
            _buyButtonTexts[index].text = "구매 완료";
            return;
        }

        _buyButtons[index].interactable = true;
        _buyButtonTexts[index].text = "구매";
    }

    private void UpdateAllBuyButtonStates()
    {
        for (int i = 0; i < _buyButtons.Length && i < _shopItems.Length; i++)
        {
            UpdateBuyButtonState(i);
        }
    }
    #endregion
}