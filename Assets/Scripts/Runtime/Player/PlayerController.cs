using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region 인스펙터
    [Header("이동 속도")]
    [SerializeField] private float _moveSpeed = 5f;

    [Header("3인칭 카메라")]
    [SerializeField] private ThirdPersonCamera _cameraController;

    [Header("무기")]
    [SerializeField] private List<Weapon> _weapons;

    [Header("애니메이터")]
    [SerializeField] private Animator _animator;
    #endregion

    #region 내부 변수
    private string _paramSpeed = "Speed";
    private string _paramAiming = "IsAiming";
    private string _paramShoot = "Shoot";
    private string _paramReload = "Reload";
    private string _paramWeaponType = "WeaponType";

    private float _animationDamp = 0.1f;

    private int _hashSpeed;
    private int _hashAiming;
    private int _hashShoot;
    private int _hashReload;
    private int _hashWeaponType;
    private int _currentWeaponIndex;

    private CharacterController _characterController;
    #endregion

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();

        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }

        _hashSpeed = Animator.StringToHash(_paramSpeed);
        _hashAiming = Animator.StringToHash(_paramAiming);
        _hashShoot = Animator.StringToHash(_paramShoot);
        _hashReload = Animator.StringToHash(_paramReload);
        _hashWeaponType = Animator.StringToHash(_paramWeaponType);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        for (int i = 0; i < _weapons.Count; i++)
        {
            _weapons[i].gameObject.SetActive(i == _currentWeaponIndex);
        }

        _animator.SetInteger(_hashWeaponType, (int)_weapons[_currentWeaponIndex].Type);
    }

    private void Update()
    {
        Move();
        Aim();
        SwitchWeapon();
        Shoot();
        Reload();
        UpdateAnimation();
    }

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDir = _cameraController.GetRight() * horizontal + _cameraController.GetForward() * vertical;

        moveDir.Normalize();

        _characterController.Move(moveDir * _moveSpeed * Time.deltaTime);
    }

    private void Aim()
    {
        Vector3 lookDir = _cameraController.GetForward();

        if (lookDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDir);
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
        if (index < 0 || index >= _weapons.Count)
        {
            return;
        }

        if (index == _currentWeaponIndex)
        {
            return;
        }

        for (int i = 0; i < _weapons.Count; i++)
        {
            _weapons[i].gameObject.SetActive(i == index);
        }

        _currentWeaponIndex = index;

        _animator.SetInteger(_hashWeaponType, (int)_weapons[_currentWeaponIndex].Type);
    }

    private void Shoot()
    {
        if (!Input.GetMouseButton(0))
        {
            return;
        }

        if (_weapons[_currentWeaponIndex].Fire())
        {
            _animator.SetTrigger(_hashShoot);
        }
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

        bool isAiming = Input.GetMouseButton(1);

        _animator.SetBool(_hashAiming, isAiming);
    }
}