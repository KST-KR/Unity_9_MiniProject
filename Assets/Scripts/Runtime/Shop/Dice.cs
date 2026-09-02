using System;
using UnityEngine;

[Serializable]
public class Dice
{
    #region 내부 변수
    private int _diceSides;
    private int _minValue;

    private int _currentResult;
    #endregion

    #region 프로퍼티
    public int DiceSides => _diceSides;
    public int MinValue => _minValue;
    public int CurrentResult => _currentResult;
    #endregion

    public Dice()
    {
        _diceSides = 4;
        _minValue = 1;
        _currentResult = 0;
    }

    public int Roll()
    {
        _currentResult = UnityEngine.Random.Range(_minValue, _diceSides + 1);

        return _currentResult;
    }

    public bool CanUpgradeDiceType()
    {
        return _diceSides == 4;
    }

    public bool CanIncreaseMinValue()
    {
        return _minValue < _diceSides;
    }

    public void UpgradeDiceType()
    {
        if (!CanUpgradeDiceType())
        {
            return;
        }

        _diceSides = 6;
    }

    public void IncreaseMinValue()
    {
        if (!CanIncreaseMinValue())
        {
            return;
        }

        _minValue++;
    }
}