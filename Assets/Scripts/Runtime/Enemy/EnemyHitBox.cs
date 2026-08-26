using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    #region 인스펙터
    [Header("대미지 배율")]
    [SerializeField] private float _damageMultiplier = 1f;
    #endregion

    #region 파라미터
    public float DamageMultiplier => _damageMultiplier;
    #endregion
}