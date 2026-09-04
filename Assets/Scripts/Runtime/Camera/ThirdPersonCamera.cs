using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class ThirdPersonCamera : MonoBehaviour
{
    #region 인스펙터
    [Header("플레이어")]
    [SerializeField] private Transform _target;

    [Header("일반 카메라")]
    [SerializeField] private Vector3 _normalOffset = new Vector3(0f, 5f, -7f);

    [Header("조준 카메라")]
    [SerializeField] private Vector3 _aimOffset = new Vector3(0.8f, 2f, -3f);

    [Header("카메라 회전")]
    [SerializeField] private float _rotateSpeed = 5f;

    [Header("에임 레이캐스트")]
    [SerializeField] private LayerMask _aimRaycastMask = ~0;
    [SerializeField] private float _aimRaycastDistance = 500f;

    [Header("카메라 반동")]
    [SerializeField] private float _recoilSpeed = 15f;
    [SerializeField] private float _recoilReturnSpeed = 10f;

    [Header("앉기 카메라")]
    [SerializeField] private float _crouchCameraOffset = 1f;
    [SerializeField] private float _crouchCameraSpeed = 8f;

    [Header("카메라 충돌")]
    [SerializeField] private LayerMask _cameraCollisionMask = ~0;
    [SerializeField] private float _cameraCollisionRadius = 0.2f;
    [SerializeField] private float _cameraCollisionOffset = 0.1f;
    [SerializeField] private float _minimumCameraDistance = 0.8f;

    [Header("사망 카메라")]
    [SerializeField] private float _deathCameraHeight = 4f;
    [SerializeField] private float _deathCameraMoveSpeed = 3f;
    #endregion

    #region 내부 변수
    private Camera _camera;

    private float _yaw;
    private float _pitch;
    private float _recoilPitch;
    private float _targetRecoilPitch;
    private float _currentCrouchOffset;

    private bool _isCrouching;
    private bool _isDeathCamera;

    private Vector3 _deathCameraStartPosition;
    #endregion

    #region 프로퍼티
    public bool IsAiming { get; private set; }
    #endregion

    private void Awake()
    {
        _camera = GetComponent<Camera>();

        if (_camera == null)
        {
            _camera = Camera.main;
        }

        Vector3 currentRot = transform.eulerAngles;

        _yaw = currentRot.y;
        _pitch = currentRot.x;
    }

    private void Update()
    {
        if (_isDeathCamera)
        {
            UpdateRecoil();
            return;
        }

        IsAiming = Input.GetMouseButton(1);

        Rotate();
        UpdateRecoil();
    }

    private void LateUpdate()
    {
        if (_target == null)
        {
            return;
        }

        if (_isDeathCamera)
        {
            UpdateDeathCamera();
            return;
        }

        UpdateCrouchCamera();
        UpdateCameraPosition();
    }

    private void Rotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        _yaw += mouseX * _rotateSpeed;
        _pitch -= mouseY * _rotateSpeed;

        _pitch = Mathf.Clamp(_pitch, -30f, 60f);
    }

    public void StartDeathCamera()
    {
        _isDeathCamera = true;

        _deathCameraStartPosition = _camera.transform.position;
    }

    private void UpdateDeathCamera()
    {
        Vector3 targetPos = _target.position;

        CharacterController characterController = _target.GetComponent<CharacterController>();

        if (characterController != null)
        {
            targetPos += characterController.center;
        }

        Vector3 deathCameraPos = _deathCameraStartPosition + Vector3.up * _deathCameraHeight;

        _camera.transform.position = Vector3.MoveTowards(_camera.transform.position, deathCameraPos, _deathCameraMoveSpeed * Time.deltaTime);

        Vector3 lookDir = targetPos - _camera.transform.position;

        if (lookDir != Vector3.zero)
        {
            _camera.transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }

    private void UpdateCrouchCamera()
    {
        float targetOffset = _isCrouching ? -_crouchCameraOffset : 0f;

        _currentCrouchOffset = Mathf.MoveTowards(_currentCrouchOffset, targetOffset, _crouchCameraSpeed * Time.deltaTime);
    }

    private void UpdateCameraPosition()
    {
        Vector3 finalPos = CalculateCameraPosition(out Quaternion rot);

        _camera.transform.position = finalPos;
        _camera.transform.rotation = rot;
    }

    private Vector3 CalculateCameraPosition(out Quaternion rot)
    {
        rot = Quaternion.Euler(_pitch + _recoilPitch, _yaw, 0f);

        Vector3 offset = IsAiming ? _aimOffset : _normalOffset;

        Vector3 targetPos = _target.position;

        CharacterController characterController = _target.GetComponent<CharacterController>();

        if (characterController != null)
        {
            targetPos += characterController.center;
        }

        targetPos.y += _currentCrouchOffset;

        Vector3 desiredPos = targetPos + rot * offset;

        Vector3 cameraDir = desiredPos - targetPos;
        float cameraDistance = cameraDir.magnitude;

        if (cameraDir != Vector3.zero)
        {
            cameraDir.Normalize();

            if (Physics.SphereCast(targetPos, _cameraCollisionRadius, cameraDir, out RaycastHit hit, cameraDistance, _cameraCollisionMask, QueryTriggerInteraction.Ignore))
            {
                cameraDistance = Mathf.Max(hit.distance - _cameraCollisionOffset, 0f);
            }
        }

        cameraDistance = Mathf.Max(cameraDistance, _minimumCameraDistance);

        return targetPos + cameraDir * cameraDistance;
    }

    public Vector3 GetForward()
    {
        Quaternion rot = Quaternion.Euler(0f, _yaw, 0f);
        Vector3 forward = rot * Vector3.forward;
        return forward.normalized;
    }

    public Vector3 GetRight()
    {
        Quaternion rot = Quaternion.Euler(0f, _yaw, 0f);
        Vector3 right = rot * Vector3.right;
        return right.normalized;
    }

    public Vector3 GetAimPosition()
    {
        Vector3 camPos = CalculateCameraPosition(out Quaternion rot); 
        Vector3 camForward = rot * Vector3.forward;

        Ray ray = new Ray(camPos, camForward);

        if (Physics.Raycast(ray, out RaycastHit hit, _aimRaycastDistance, _aimRaycastMask, QueryTriggerInteraction.Ignore))
        {
            return hit.point;
        }

        return ray.GetPoint(_aimRaycastDistance);
    }

    public void AddRecoil(float amount)
    {
        _targetRecoilPitch -= amount;
    }

    private void UpdateRecoil()
    {
        _recoilPitch = Mathf.MoveTowards(_recoilPitch, _targetRecoilPitch, _recoilSpeed * Time.deltaTime);

        _targetRecoilPitch = Mathf.MoveTowards(_targetRecoilPitch, 0f, _recoilReturnSpeed * Time.deltaTime);
    }

    public void SetCrouching(bool isCrouching)
    {
        _isCrouching = isCrouching;
    }
}
