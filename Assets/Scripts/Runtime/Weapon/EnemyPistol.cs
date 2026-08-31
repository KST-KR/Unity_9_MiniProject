using UnityEngine;

public class EnemyPistol : MonoBehaviour
{
    #region 인스펙터
    [Header("공격")]
    [SerializeField] private float _damage = 5f;

    [Header("총알")]
    [SerializeField] private BulletPool _bulletPool;

    [Header("발사 위치")]
    [SerializeField] private Transform _firePoint;

    [Header("조준 보정")]
    [SerializeField] private float _targetHeightOffset = 1f;
    
    [Header("총알")]
    [SerializeField] private float _bulletSpeed = 10f;
    #endregion

    #region 내부 변수
    private float _baseDamage;
    private float _currentDamage;
    #endregion

    private void Awake()
    {
        _baseDamage = _damage;
        _currentDamage = _damage;
    }

    public void SetDamageMultiplier(float multiplier)
    {
        _currentDamage = _baseDamage * multiplier;

        CPrint.Log($"원거리 적 공격력 설정 : 기본 {_baseDamage} → 현재 {_currentDamage}");
    }

    public void SetBulletPool(BulletPool bulletPool)
    {
        _bulletPool = bulletPool;
    }

    public void Fire(Transform target)
    {
        CPrint.Log("Fire 호출");

        if (_bulletPool == null)
        {
            CPrint.Log("BulletPool 없음");
            return;
        }

        if (_firePoint == null)
        {
            CPrint.Log("FirePoint 없음");
            return;
        }

        if (target == null)
        {
            CPrint.Log("Target 없음");
            return;
        }

        Vector3 targetPos = target.position + Vector3.up * _targetHeightOffset;
        Vector3 fireDir = targetPos - _firePoint.position;

        if (fireDir == Vector3.zero)
        {
            CPrint.Log("FireDir가 Vector3.zero");
            return;
        }

        fireDir.Normalize();

        Bullet bullet = _bulletPool.Get();

        if (bullet == null)
        {
            CPrint.Log("BulletPool에서 총알을 가져오지 못함");
            return;
        }

        bullet.transform.position = _firePoint.position;
        bullet.transform.rotation = Quaternion.LookRotation(fireDir);

        bullet.Initialize(fireDir, _currentDamage, _bulletSpeed, _bulletPool, Bullet.BulletOwner.Enemy);

        CPrint.Log($"원거리 적 공격 Damage : {_currentDamage}");
    }
}