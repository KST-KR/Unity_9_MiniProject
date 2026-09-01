using UnityEngine;

public class DiceManager : MonoBehaviour
{
	#region 내부 변수
	private int _currentResult;
	#endregion

	#region 프로퍼티
	public int CurrentResult => _currentResult;
	#endregion

	public int RollD4()
	{
		_currentResult = Random.Range(1, 5);

		CPrint.Log($"D4 주사위 결과 : {_currentResult}");

		return _currentResult;
	}
}
