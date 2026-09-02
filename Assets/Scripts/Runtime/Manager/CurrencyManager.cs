using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    #region 인스펙터
    [Header("재화")]
    [SerializeField] private int _startCurrency = 0;
    #endregion

    #region 내부 변수
    private int _currentCurrency;
    #endregion

    #region 프로퍼티
    public int CurrentCurrency => _currentCurrency;
    #endregion

    #region 이벤트
    public event System.Action<int> CurrencyChanged;
    #endregion

    private void Awake()
    {
        _currentCurrency = _startCurrency;
    }

    public void AddCurrency(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        _currentCurrency += amount;

        CPrint.Log($"재화 획득 : +{amount}, 현재 재화 : {_currentCurrency}");

        CurrencyChanged?.Invoke(_currentCurrency);
    }

    public bool TrySpendCurrency(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (_currentCurrency < amount)
        {
            CPrint.Log("재화 부족");
            return false;
        }

        _currentCurrency -= amount;

        CPrint.Log($"재화 사용 : -{amount}, 현재 재화 : {_currentCurrency}");

        CurrencyChanged?.Invoke(_currentCurrency);

        return true;
    }
}
