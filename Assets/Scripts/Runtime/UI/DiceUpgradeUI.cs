using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceUpgradeUI : MonoBehaviour
{
    #region 인스펙터
    [Header("주사위")]
    [SerializeField] private DiceManager _diceManager;
    [SerializeField] private DiceSlotUI[] _diceSlots;

    [Header("주사위 이미지")]
    [SerializeField] private Sprite[] _d4Sprites;
    [SerializeField] private Sprite[] _d6Sprites;

    [Header("뒤로가기")]
    [SerializeField] private Button _backButton;

    [Header("패널")]
    [SerializeField] private GameObject _waveEndPanel;
    #endregion

    private void Awake()
    {
        if (_diceManager == null)
        {
            _diceManager = FindFirstObjectByType<DiceManager>();
        }

        for (int i = 0; i < _diceSlots.Length; i++)
        {
            if (_diceSlots[i] == null)
            {
                continue;
            }

            _diceSlots[i].Initialize(i, this);
        }

        if (_backButton != null)
        {
            _backButton.onClick.AddListener(Close);
        }
    }

    private void OnDestroy()
    {
        if (_backButton != null)
        {
            _backButton.onClick.RemoveListener(Close);
        }
    }

    #region 패널
    public void Show()
    {
        gameObject.SetActive(true);

        Refresh();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
    }
    #endregion

    #region 주사위
    public void Refresh()
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

            bool hasDice = i < dices.Count;

            _diceSlots[i].gameObject.SetActive(hasDice);

            if (!hasDice)
            {
                continue;
            }

            _diceSlots[i].Refresh(dices[i], _diceManager, _d4Sprites, _d6Sprites);
        }
    }

    public void UpgradeDiceType(int index)
    {
        if (_diceManager == null)
        {
            return;
        }

        bool upgraded = _diceManager.UpgradeDiceType(index);

        if (!upgraded)
        {
            return;
        }

        Refresh();
    }

    public void IncreaseMinValue(int index)
    {
        if (_diceManager == null)
        {
            return;
        }

        bool upgraded = _diceManager.IncreaseMinValue(index);

        if (!upgraded)
        {
            return;
        }

        Refresh();
    }
    #endregion
}