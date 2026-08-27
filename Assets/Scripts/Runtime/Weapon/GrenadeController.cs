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
    #endregion

    #region 내부 변수
    private string _paramGrenade = "ThrowGrenade";

    private int _hashGrenade;

    private PlayerController _playerController;

    private float _throwEndTime;
    private float _animationEndTime;

    private bool _isThrowing;
    private bool _hasThrown;
    #endregion

    private void Awake()
    {
        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }

        _playerController = GetComponent<PlayerController>();

        _hashGrenade = Animator.StringToHash(_paramGrenade);
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

        _isThrowing = true;
        _hasThrown = false;

        _throwEndTime = Time.time + _throwTime;
        _animationEndTime = Time.time + _animationDuration;

        _playerController.CurrentWeapon.gameObject.SetActive(false);

        _playerController.SetAiming(false);

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

            _weaponAimRigController.SetThrowingGrenade(false);

            _isThrowing = false;
        }
    }

    private void CreateGrenade()
    {
        Grenade grenade = Instantiate(_grenadePrefab, _grenadeSpawnPoint.position, Quaternion.identity);

        Vector3 throwDir = transform.forward + Vector3.up * 0.3f;

        grenade.Initialize(throwDir);
    }

    private void EnableWeapon()
    {
        _playerController.CurrentWeapon.gameObject.SetActive(true);
    }
}