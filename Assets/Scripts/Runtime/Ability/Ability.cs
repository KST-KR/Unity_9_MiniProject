using UnityEngine;

[CreateAssetMenu(fileName = "Ability_", menuName = "DiceGunDefense/Ability")]
public class Ability : ScriptableObject
{
	#region 인스펙터
	[Header("어빌리티")]
	[SerializeField] private string _abilityName;
	[SerializeField] private string _description;
	[SerializeField] private Sprite _icon;

	[Header("효과")]
	[SerializeField] private AbilityType _type;
	#endregion

	#region 프로퍼티
	public string AbilityName => _abilityName;
	public string Description => _description;
	public Sprite Icon => _icon;
	public AbilityType Type => _type;
	#endregion
}
