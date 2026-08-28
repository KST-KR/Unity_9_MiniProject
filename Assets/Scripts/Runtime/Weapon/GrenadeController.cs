using UnityEngine;

public class GrenadeController : MonoBehaviour
{
    #region 인스펙터
    [Header("수류탄")]
    [SerializeField] private Grenade _grenadePrefab;
    [SerializeField] private Transform _grenadeSpawnPoint;

    [Header("애니메이터")]
    [SerializeField] private Animator _animator;

    [Header("조준 리깅")]
    [SerializeField] private WeaponAimRigController _weaponAimRigController;

    [Header("투척 시간")]
    [SerializeField] private float _throwTime = 0.5f;
    [SerializeField] private float _animationDuration = 1.2f;

    [Header("쿨타임")]
    [SerializeField] private float _cooldown = 5f;
    #endregion

    #region 내부 변수
    private string _paramGrenade = "ThrowGrenade";

    private int _hashGrenade;

    private PlayerController _playerController;

    private float _throwEndTime;
    private float _animationEndTime;
    private float _nextUseTime;

    private bool _isThrowing;
    private bool _hasThrown;
    #endregion

    #region 프로퍼티
    public float Cooldown => _cooldown;
    public float RemainingCooldown => Mathf.Max(_nextUseTime - Time.time, 0f);
    #endregion

    private void Awake()
    {
        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }

        _playerController = GetComponent<PlayerController>();

        _hashGrenade = Animator.StringToHash(_paramGrenade);

        if (_playerController == null)
        {
            CPrint.Warn("PlayerController 없음");
        }

        if (_animator == null)
        {
            CPrint.Warn("Animator 없음");
        }

        if (_grenadePrefab == null)
        {
            CPrint.Warn("Grenade Prefab 없음");
        }

        if (_grenadeSpawnPoint == null)
        {
            CPrint.Warn("Grenade Spawn Point 없음");
        }

        if (_weaponAimRigController == null)
        {
            CPrint.Warn("WeaponAimRigController 없음");
        }
    }

    private void Update()
    {
        if (_isThrowing)
        {
            UpdateThrow();
            return;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            ThrowGrenade();
        }
    }

    private void ThrowGrenade()
    {
        if (_isThrowing)
        {
            return;
        }

        if (Time.time < _nextUseTime)
        {
            return;
        }

        if (_playerController == null)
        {
            return;
        }

        if (_playerController.CurrentWeapon == null)
        {
            CPrint.Warn("CurrentWeapon 없음");
            return;
        }

        if (_grenadePrefab == null)
        {
            return;
        }

        if (_grenadeSpawnPoint == null)
        {
            return;
        }

        if (_weaponAimRigController == null)
        {
            return;
        }

        _nextUseTime = Time.time + _cooldown;

        _isThrowing = true;
        _hasThrown = false;

        _throwEndTime = Time.time + _throwTime;
        _animationEndTime = Time.time + _animationDuration;

        _playerController.CurrentWeapon.gameObject.SetActive(false);

        _playerController.SetAiming(false);
        _playerController.SetShooting(false);

        _weaponAimRigController.SetThrowingGrenade(true);

        _animator.SetTrigger(_hashGrenade);
    }

    private void UpdateThrow()
    {
        if (!_hasThrown && Time.time >= _throwEndTime)
        {
            _hasThrown = true;
            CreateGrenade();
        }

        if (Time.time >= _animationEndTime)
        {
            EnableWeapon();

            _playerController.SetAiming(true);
            _playerController.SetShooting(true);

            _weaponAimRigController.SetThrowingGrenade(false);

            _isThrowing = false;
        }
    }

    private void CreateGrenade()
    {
        if (_grenadePrefab == null)
        {
            return;
        }

        if (_grenadeSpawnPoint == null)
        {
            return;
        }

        Grenade grenade = Instantiate(_grenadePrefab, _grenadeSpawnPoint.position, Quaternion.identity);

        Vector3 throwDir = transform.forward + Vector3.up * 0.3f;

        grenade.Initialize(throwDir);
    }

    private void EnableWeapon()
    {
        if (_playerController == null)
        {
            return;
        }

        if (_playerController.CurrentWeapon == null)
        {
            return;
        }

        _playerController.CurrentWeapon.gameObject.SetActive(true);
    }
}