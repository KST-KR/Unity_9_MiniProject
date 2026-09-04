using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region 인스펙터
    [Header("이동 속도")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _runMultiplier = 1.8f;

    [Header("점프")]
    [SerializeField] private float _jumpHeight = 2f;

    [Header("중력")]
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _groundStick = -2f;

    [Header("3인칭 카메라")]
    [SerializeField] private ThirdPersonCamera _cameraController;

    [Header("무기")]
    [SerializeField] private List<Weapon> _weapons;

    [Header("애니메이터")]
    [SerializeField] private Animator _animator;

    [Header("조준 리깅")]
    [SerializeField] private WeaponAimRigController _weaponAimRigController;
    #endregion

    #region 내부 변수
    private string _paramSpeed = "Speed";
    private string _paramRunning = "IsRunning";
    private string _paramJump = "Jump";
    private string _paramCrouching = "IsCrouching";
    private string _paramAiming = "IsAiming";
    private string _paramShoot = "Shoot";
    private string _paramReload = "Reload";
    private string _paramWeaponType = "WeaponType";
    private string _paramGrounded = "IsGrounded";
    private string _paramHit = "Hit";
    private string _paramDeath = "Death";

    private float _animationDamp = 0.1f;
    private float _verticalVelocity;
    private float _baseMoveSpeed;
    private float _moveSpeedMultiplier = 1f;

    private int _hashSpeed;
    private int _hashRunning;
    private int _hashJump;
    private int _hashCrouching;
    private int _hashAiming;
    private int _hashShoot;
    private int _hashReload;
    private int _hashWeaponType;
    private int _hashGrounded;
    private int _hashHit;
    private int _hashDeath;
    private int _currentWeaponIndex = 0;

    private bool _isCrouching;
    private bool _isDead;
    private bool _canAim = true;
    private bool _canShoot = true;

    private CharacterController _characterController;
    private Health _health;

    private Vector3 _moveVelocity;
    #endregion

    #region 프로퍼티
    public Weapon CurrentWeapon => _weapons[_currentWeaponIndex];
    public IReadOnlyList<Weapon> Weapons => _weapons;
    #endregion

    #region 이벤트
    public event System.Action WeaponChanged;
    #endregion

    private void Awake()
    {
        _baseMoveSpeed = _moveSpeed;

        _characterController = GetComponent<CharacterController>();
        _health = GetComponent<Health>();

        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }

        _health.Hit += OnHit;
        _health.Died += OnDied;

        _hashSpeed = Animator.StringToHash(_paramSpeed);
        _hashAiming = Animator.StringToHash(_paramAiming);
        _hashJump = Animator.StringToHash(_paramJump);
        _hashCrouching = Animator.StringToHash(_paramCrouching);
        _hashShoot = Animator.StringToHash(_paramShoot);
        _hashReload = Animator.StringToHash(_paramReload);
        _hashWeaponType = Animator.StringToHash(_paramWeaponType);
        _hashRunning = Animator.StringToHash(_paramRunning);
        _hashGrounded = Animator.StringToHash(_paramGrounded);
        _hashHit = Animator.StringToHash(_paramHit);
        _hashDeath = Animator.StringToHash(_paramDeath);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (_weapons.Count == 0)
        {
            return;
        }

        for (int i = 0; i < _weapons.Count; i++)
        {
            _weapons[i].gameObject.SetActive(i == _currentWeaponIndex);
        }

        Weapon currentWeapon = CurrentWeapon;

        _animator.SetInteger(_hashWeaponType, (int)currentWeapon.Type);

        _weaponAimRigController.SetWeaponType(currentWeapon.Type);
        _weaponAimRigController.SetLeftHandTarget(currentWeapon.LeftHandTarget);

        currentWeapon.SetCrouching(_isCrouching);

        currentWeapon.ReloadStarted += OnReloadStarted;
        currentWeapon.ReloadCompleted += OnReloadCompleted;
    }

    private void Update()
    {
        if (_isDead)
        {
            return;
        }
        
        Crouch();
        Move();
        JumpAndGravity();
        ApplyMovement();
        UpdateCameraCrouch();

        Aim();
        SwitchWeapon();
        Shoot();
        Reload();
        UpdateAnimation();
    }

    private void OnReloadStarted()
    {
        _weaponAimRigController.SetReloading(true);
    }

    private void OnReloadCompleted()
    {
        _weaponAimRigController.SetReloading(false);
    }

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDir = _cameraController.GetRight() * horizontal + _cameraController.GetForward() * vertical;

        moveDir = Vector3.ClampMagnitude(moveDir, 1f);

        bool isMoving = moveDir != Vector3.zero;

        float currentSpeed = _moveSpeed;

        if (_isCrouching)
        {
            currentSpeed *= 0.5f;
        }
        else if (isMoving && Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed *= _runMultiplier;
        }

        _moveVelocity = moveDir * currentSpeed;
    }

    private void JumpAndGravity()
    {
        if (_characterController.isGrounded)
        {
            if (_verticalVelocity < 0f)
            {
                _verticalVelocity = _groundStick;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);

                _animator.SetTrigger(_hashJump);
            }
        }
        else
        {
            _verticalVelocity += _gravity * Time.deltaTime;
        }
    }

    private void ApplyMovement()
    {
        Vector3 velocity = _moveVelocity;
        velocity.y = _verticalVelocity;

        _characterController.Move(velocity * Time.deltaTime);
    }

    private void Crouch()
    {
        if (!Input.GetKeyDown(KeyCode.LeftControl))
        {
            return;
        }

        _isCrouching = !_isCrouching;

        CurrentWeapon.SetCrouching(_isCrouching);
    }

    private void UpdateCameraCrouch()
    {
        _cameraController.SetCrouching(_isCrouching);
    }

    private void Aim()
    {
        if (!_canAim)
        {
            return;
        }

        Vector3 lookDir = _cameraController.GetForward();

        if (lookDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }

    public void SetAiming(bool canAim)
    {
        _canAim = canAim;

        if (!canAim)
        {
            _animator.SetBool(_hashAiming, false);
        }
    }

    private void SwitchWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeWeapon(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeWeapon(1);
        }
    }

    private void ChangeWeapon(int index)
    {
        if (_weapons[_currentWeaponIndex].IsReloading)
        {
            return;
        }

        if (index < 0 || index >= _weapons.Count)
        {
            return;
        }

        if (index == _currentWeaponIndex)
        {
            return;
        }

        Weapon previousWeapon = CurrentWeapon;

        previousWeapon.ReloadStarted -= OnReloadStarted;
        previousWeapon.ReloadCompleted -= OnReloadCompleted;

        for (int i = 0; i < _weapons.Count; i++)
        {
            _weapons[i].gameObject.SetActive(i == index);
        }

        _currentWeaponIndex = index;

        Weapon currentWeapon = CurrentWeapon;

        currentWeapon.ReloadStarted += OnReloadStarted;
        currentWeapon.ReloadCompleted += OnReloadCompleted;

        _animator.SetInteger(_hashWeaponType, (int)currentWeapon.Type);

        _weaponAimRigController.SetWeaponType(currentWeapon.Type);
        _weaponAimRigController.SetLeftHandTarget(currentWeapon.LeftHandTarget);

        currentWeapon.SetCrouching(_isCrouching);

        WeaponChanged?.Invoke();
    }

    private void Shoot()
    {
        if (!_canShoot)
        {
            return;
        }

        if (!Input.GetMouseButton(0))
        {
            return;
        }

        if (_weapons[_currentWeaponIndex].Fire())
        {
            _animator.SetTrigger(_hashShoot);
        }
    }

    public void SetShooting(bool canShoot)
    {
        _canShoot = canShoot;
    }

    private void Reload()
    {
        if (!Input.GetKeyDown(KeyCode.R))
        {
            return;
        }

        if (_weapons[_currentWeaponIndex].Reload())
        {
            _animator.SetTrigger(_hashReload);
        }
    }

    private void UpdateAnimation()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(horizontal, 0f, vertical);
        input = Vector3.ClampMagnitude(input, 1f);

        float speed = input.magnitude;

        _animator.SetFloat(_hashSpeed, speed, _animationDamp, Time.deltaTime);

        bool isReloading = _weapons[_currentWeaponIndex].IsReloading;
        bool isAiming = Input.GetMouseButton(1) && !isReloading && _canAim;
        bool isRunning = input != Vector3.zero && Input.GetKey(KeyCode.LeftShift) && !_isCrouching;
        bool isGrounded = _characterController.isGrounded;

        _animator.SetBool(_hashAiming, isAiming);
        _animator.SetBool(_hashRunning, isRunning);
        _animator.SetBool(_hashGrounded, isGrounded);
        _animator.SetBool(_hashCrouching, _isCrouching);
    }

    private void OnHit()
    {
        _animator.SetTrigger(_hashHit);
    }

    private void OnDied()
    {
        _isDead = true;

        _animator.SetFloat(_hashSpeed, 0f);
        _animator.SetBool(_hashAiming, false);
        _animator.SetBool(_hashRunning, false);
        _animator.SetTrigger(_hashDeath);

        _cameraController.StartDeathCamera();

        if(GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

    public void SetMoveSpeedMultiplier(float multiplier)
    {
        _moveSpeedMultiplier = multiplier;
        _moveSpeed = _baseMoveSpeed * _moveSpeedMultiplier;

        CPrint.Log($"이동 속도 설정 : 기본 {_baseMoveSpeed} → 현재 {_moveSpeed}");
    }
}