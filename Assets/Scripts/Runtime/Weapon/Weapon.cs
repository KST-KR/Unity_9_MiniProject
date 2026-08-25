using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    #region 인스펙터
    [Header("무기")]
    [SerializeField] private WeaponType _weaponType;

    [Header("사격")]
    [SerializeField] private GameObject _bulletPrefabs;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _fireRate = 0.2f;

    [Header("탄창")]
    [SerializeField] private int _magazineSize = 12;
    [SerializeField] private float _reloadTime = 1.5f;

    [Header("3인칭 카메라")]
    [SerializeField] private ThirdPersonCamera _cameraController;

    [Header("총알")]
    [SerializeField] private BulletPool _bulletPool;

    [Header("반동")]
    [SerializeField] private Transform _weaponModel;
    [SerializeField] private float _recoilAngle = 5f;
    [SerializeField] private float _recoilSpeed = 15f;
    [SerializeField] private float _returnSpeed = 20f;
    [SerializeField] private float _cameraRecoilAmount = 1f;

    [Header("UI")]
    [SerializeField] private Sprite _weaponIcon;

    [Header("총구 화염")]
    [SerializeField] private ParticleSystem _muzzleFlash;

    [Header("왼손 IK")]
    [SerializeField] private Transform _leftHandTarget;
    #endregion

    #region 내부 변수
    private int _currentAmmo;
    private float _lastFireTime;
    private float _reloadEndTime;
    private bool _isReloading;

    private Quaternion _initialLocalRotation;
    private Quaternion _targetLocalRotation;
    #endregion

    #region 프로퍼티
    public WeaponType Type => _weaponType;
    public int CurrentAmmo => _currentAmmo;
    public int MagazineSize => _magazineSize;
    public Sprite WeaponIcon => _weaponIcon;
    public bool IsReloading => _isReloading;
    public Transform LeftHandTarget => _leftHandTarget;
    #endregion

    #region 이벤트
    public event System.Action<int, int> AmmoChanged;
    public event System.Action ReloadStarted;
    public event System.Action ReloadCompleted;
    #endregion

    private void Awake()
    {
        _currentAmmo = _magazineSize;

        _initialLocalRotation = _weaponModel.localRotation;
        _targetLocalRotation = _initialLocalRotation;
    }

    private void Update()
    {
        UpdateReload();
        UpdateRecoil();
    }

    public bool Fire()
    {
        if (_isReloading)
        {
            return false;
        }

        if (_currentAmmo <= 0)
        {
            return false;
        }

        if (Time.time < _lastFireTime + _fireRate)
        {
            return false;
        }

        Vector3 aimPos = _cameraController.GetAimPosition();

        Vector3 fireDir = aimPos - _firePoint.position;

        if (fireDir == Vector3.zero)
        {
            return false;
        }

        fireDir.Normalize();

        Bullet bullet = _bulletPool.Get();

        if (bullet == null)
        {
            return false;
        }

        _lastFireTime = Time.time;
        _currentAmmo--;

        AmmoChanged?.Invoke(_currentAmmo, _magazineSize);

        bullet.transform.position = _firePoint.position;
        bullet.transform.rotation = Quaternion.LookRotation(fireDir);

        bullet.Initialize(fireDir, _bulletPool);

        _muzzleFlash.Play();

        _targetLocalRotation = _initialLocalRotation * Quaternion.Euler(-_recoilAngle, 0f, 0f);

        _cameraController.AddRecoil(_cameraRecoilAmount);

        return true;
    }

    public bool Reload()
    {
        if (_isReloading)
        {
            return false;
        }

        if (_currentAmmo == _magazineSize)
        {
            return false;
        }

        _isReloading = true;
        _reloadEndTime = Time.time + _reloadTime;

        ReloadStarted?.Invoke();

        return true;
    }

    private void UpdateReload()
    {
        if (!_isReloading)
        {
            return;
        }

        if (Time.time < _reloadEndTime)
        {
            return;
        }

        _currentAmmo = _magazineSize;
        _isReloading = false;

        AmmoChanged?.Invoke(_currentAmmo, _magazineSize);
        ReloadCompleted?.Invoke();
    }

    private void UpdateRecoil()
    {
        _weaponModel.localRotation = Quaternion.Slerp(_weaponModel.localRotation, _targetLocalRotation, _recoilSpeed * Time.deltaTime);

        _targetLocalRotation = Quaternion.Slerp(_targetLocalRotation, _initialLocalRotation, _returnSpeed * Time.deltaTime);
    }
}