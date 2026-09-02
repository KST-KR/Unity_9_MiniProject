using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    #region 상수
    public const int MaxDiceCount = 5;
    #endregion

    #region 내부 변수
    private List<Dice> _dices = new List<Dice>();

    private int _diceTypeUpgradeCount;
    private int _minValueUpgradeCount;
    private int _rerollCount;
    #endregion

    #region 프로퍼티
    public IReadOnlyList<Dice> Dices => _dices;

    public int DiceCount => _dices.Count;

    public int DiceTypeUpgradeCount => _diceTypeUpgradeCount;

    public int MinValueUpgradeCount => _minValueUpgradeCount;

    public int RerollCount => _rerollCount;
    #endregion

    private void Awake()
    {
        // 게임 시작 시 D4 하나 보유
        AddDice();
    }

    #region 주사위 소지
    public bool AddDice()
    {
        if (_dices.Count >= MaxDiceCount)
        {
            CPrint.Log("주사위 최대 보유 개수에 도달했습니다.");
            return false;
        }

        Dice dice = new Dice();

        _dices.Add(dice);

        CPrint.Log($"주사위 추가 : D{dice.DiceSides}, " + $"현재 보유 개수 : {_dices.Count}");

        return true;
    }
    #endregion

    #region 주사위 굴리기
    public int RollDice(int index)
    {
        if (!IsValidIndex(index))
        {
            return 0;
        }

        Dice dice = _dices[index];

        int result = dice.Roll();

        CPrint.Log($"주사위 {index + 1} 결과 : {result} " + $"(D{dice.DiceSides}, {dice.MinValue}~{dice.DiceSides})");

        return result;
    }

    public int RollAllDice()
    {
        int totalResult = 0;

        for (int i = 0; i < _dices.Count; i++)
        {
            totalResult += RollDice(i);
        }

        CPrint.Log($"전체 주사위 합계 : {totalResult}");

        return totalResult;
    }
    #endregion

    #region 주사위 업그레이드 횟수
    public void AddDiceTypeUpgradeCount(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        _diceTypeUpgradeCount += amount;

        CPrint.Log($"D6 업그레이드 횟수 증가 : +{amount}, " + $"현재 {_diceTypeUpgradeCount}");
    }

    public void AddMinValueUpgradeCount(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        _minValueUpgradeCount += amount;

        CPrint.Log($"범위 향상 횟수 증가 : +{amount}, " + $"현재 {_minValueUpgradeCount}");
    }
    #endregion

    #region 주사위 업그레이드
    public bool UpgradeDiceType(int index)
    {
        if (!IsValidIndex(index))
        {
            return false;
        }

        if (_diceTypeUpgradeCount <= 0)
        {
            CPrint.Log("D6 업그레이드 횟수가 부족합니다.");
            return false;
        }

        Dice dice = _dices[index];

        if (!dice.CanUpgradeDiceType())
        {
            CPrint.Log($"주사위 {index + 1}은 D6 업그레이드가 불가능합니다.");

            return false;
        }

        dice.UpgradeDiceType();

        _diceTypeUpgradeCount--;

        CPrint.Log($"주사위 {index + 1} 종류 변경 : " + $"D{dice.DiceSides}, " + $"남은 D6 업그레이드 횟수 : {_diceTypeUpgradeCount}");

        return true;
    }

    public bool IncreaseMinValue(int index)
    {
        if (!IsValidIndex(index))
        {
            return false;
        }

        if (_minValueUpgradeCount <= 0)
        {
            CPrint.Log("범위 향상 횟수가 부족합니다.");
            return false;
        }

        Dice dice = _dices[index];

        if (!dice.CanIncreaseMinValue())
        {
            CPrint.Log($"주사위 {index + 1}은 범위 향상이 불가능합니다.");

            return false;
        }

        dice.IncreaseMinValue();

        _minValueUpgradeCount--;

        CPrint.Log($"주사위 {index + 1} 범위 향상 : " + $"{dice.MinValue}~{dice.DiceSides}, " + $"남은 범위 향상 횟수 : {_minValueUpgradeCount}");

        return true;
    }
    #endregion

    #region 리롤
    public void AddRerollCount(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        _rerollCount += amount;

        CPrint.Log($"리롤 횟수 증가 : +{amount}, " + $"현재 {_rerollCount}");
    }
    #endregion

    #region 유틸리티
    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < _dices.Count;
    }
    #endregion
}