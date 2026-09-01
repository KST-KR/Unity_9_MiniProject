using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    #region 인스펙터
    [Header("다이스")]
    [SerializeField] private DiceManager _diceManager;

    [Header("어빌리티")]
    [SerializeField] private AbilityManager _abilityManager;

    [SerializeField] private List<Ability> _abilities;

    [Header("UI")]
    [SerializeField] private DiceUI _diceUI;
    #endregion

    #region 내부 변수
    private int _diceResult;
    private List<Ability> _currentChoices = new List<Ability>();
    #endregion

    #region 프로퍼티
    public int DiceResult => _diceResult;
    public IReadOnlyList<Ability> CurrentChoices => _currentChoices;
    #endregion

    #region 이벤트
    public event System.Action UpgradeCompleted;
    #endregion

    public void StartUpgrade()
    {
        _diceUI.Show();

        CPrint.Log("업그레이드 시작");
    }

    public void RollDice()
    {
        _diceResult = _diceManager.RollD4();

        CreateAbilityChoices();

        _diceUI.ShowDiceResult(_diceResult);
        _diceUI.ShowAbilities(_currentChoices);

        CPrint.Log($"업그레이드 시작 - 주사위 결과 : {_diceResult}");
    }

    private void CreateAbilityChoices()
    {
        _currentChoices.Clear();

        List<Ability> candidates = new List<Ability>(_abilities);

        while (_currentChoices.Count < 3 && candidates.Count > 0)
        {
            int randomIndex = Random.Range(0, candidates.Count);

            Ability ability = candidates[randomIndex];

            _currentChoices.Add(ability);
            candidates.RemoveAt(randomIndex);
        }

        CPrint.Log($"어빌리티 선택지 생성 : {_currentChoices.Count}개");
    }

    public void SelectAbility(int index)
    {
        if (index < 0 || index >= _currentChoices.Count)
        {
            return;
        }

        Ability ability = _currentChoices[index];

        _abilityManager.ApplyAbility(ability, _diceResult);

        CPrint.Log($"어빌리티 선택 : {ability.AbilityName}," + $"증가량 : {_diceResult}");

        _diceUI.Hide();
        UpgradeCompleted?.Invoke();
    }

}
