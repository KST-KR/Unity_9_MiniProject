using UnityEngine;

public enum ShopItemType
{
    DiceCount,
    DiceType,
    DiceMinValue,
    RerollCount
}

[CreateAssetMenu(fileName = "ShopItem_", menuName = "DiceGunDefense/ShopItem")]
public class ShopItem : ScriptableObject
{
    #region 인스펙터
    [Header("상품")]
    [SerializeField] private string _itemName;
    [SerializeField] private string _description;
    [SerializeField] private Sprite _icon;
    [SerializeField] private int _price;
    [SerializeField] private int _priceIncrease;

    [Header("효과")]
    [SerializeField] private ShopItemType _type;
    [SerializeField] private int _value;
    #endregion

    #region 프로퍼티
    public string ItemName => _itemName;
    public string Description => _description;
    public Sprite Icon => _icon;
    public int Price => _price;
    public int PriceIncrease => _priceIncrease;
    public ShopItemType Type => _type;
    public int Value => _value;
    #endregion
}