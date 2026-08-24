using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    #region 인스펙터
    [Header("사격")]
    [SerializeField] private GameObject _bulletPrefabs;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _fireRate = 0.2f;
    #endregion

    #region 내부 변수
    private float _lastFireTime;
    #endregion

    public void Fire()
    {
        if (Time.time < _lastFireTime + _fireRate)
        {
            return;
        }

        _lastFireTime = Time.time;

        Instantiate(_bulletPrefabs, _firePoint.position, _firePoint.rotation);
    }
}
