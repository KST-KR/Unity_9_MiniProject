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
    #endregion

    #region 내부 변수
    private bool[] _isPurchased;
    #endregion

    private void Awake()
    {
        if (_currencyManager == null)
        {
            _currencyManager = FindFirstObjectByType<CurrencyManager>();
        }

        if (_diceManager == null)
        {
            _diceManager = FindFirstObjectByType<DiceManager>();
        }

        if (_waveManager == null)
        {
            _waveManager = FindFirstObjectByType<WaveManager>();
        }

        _isPurchased = new bool[_shopItems.Length];

        for (int i = 0; i < _buyButtons.Length; i++)
        {
            int index = i;

            _buyButtons[i].onClick.AddListener(() =>
            {
                BuyItem(index);
            });
        }

        if (_waveManager != null)
        {
            _waveManager.WaveChanged += ResetShop;
        }
    }

    private void OnEnable()
    {
        if (_currencyManager != null)
        {
            _currencyManager.CurrencyChanged += UpdateCurrencyText;
            UpdateCurrencyText(_currencyManager.CurrentCurrency);
        }
    }

    private void OnDisable()
    {
        if (_currencyManager != null)
        {
            _currencyManager.CurrencyChanged -= UpdateCurrencyText;
        }
    }

    private void Start()
    {
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
    }

    private void SetupShop()
    {
        for (int i = 0; i < _shopItems.Length; i++)
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
            _itemPriceTexts[i].text = $"{item.Price}";

            _buyButtons[i].interactable = true;
        }
    }

    private void BuyItem(int index)
    {
        if (index < 0 || index >= _shopItems.Length)
        {
            return;
        }

        if (_isPurchased[index])
        {
            CPrint.Log(
                $"이번 웨이브에 이미 구매한 상품 : " +
                $"{_shopItems[index].ItemName}");

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

        bool spent = _currencyManager.TrySpendCurrency(item.Price);

        if (!spent)
        {
            CPrint.Log($"상품 구매 실패 : {item.ItemName}");
            return;
        }

        ApplyItem(item);

        _isPurchased[index] = true;
        _buyButtons[index].interactable = false;

        CPrint.Log($"상품 구매 완료 : {item.ItemName}");
    }

    private bool CanPurchase(ShopItem item)
    {
        switch (item.Type)
        {
            case ShopItemType.DiceCount:
                return _diceManager.DiceCount + item.Value
                    <= DiceManager.MaxDiceCount;

            case ShopItemType.DiceType:
                return true;

            case ShopItemType.DiceMinValue:
                return true;

            case ShopItemType.RerollCount:
                return true;

            default:
                return false;
        }
    }

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

    private void ResetShop(int wave)
    {
        for (int i = 0; i < _isPurchased.Length; i++)
        {
            _isPurchased[i] = false;
        }

        for (int i = 0; i < _buyButtons.Length; i++)
        {
            _buyButtons[i].interactable = true;
        }

        CPrint.Log($"Wave {wave} 시작 → 상점 구매 상태 초기화");
    }

    private void UpdateCurrencyText(int currency)
    {
        if (_currencyText == null)
        {
            return;
        }

        _currencyText.text = $"재화 : {currency}";
    }
}