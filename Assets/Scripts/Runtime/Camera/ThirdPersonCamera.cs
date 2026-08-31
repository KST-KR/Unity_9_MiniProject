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
    #endregion

    #region 내부 변수
    private Camera _camera;

    private float _yaw;
    private float _pitch;
    private float _recoilPitch;
    private float _targetRecoilPitch;
    private float _currentCrouchOffset;

    private bool _isCrouching;
    #endregion
    public bool IsAiming { get; private set; }


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

    private void UpdateCrouchCamera()
    {
        float targetOffset = _isCrouching ? -_crouchCameraOffset : 0f;

        _currentCrouchOffset = Mathf.MoveTowards(_currentCrouchOffset, targetOffset, _crouchCameraSpeed * Time.deltaTime);
    }

    private void UpdateCameraPosition()
    {
        Quaternion rot = Quaternion.Euler(_pitch + _recoilPitch, _yaw, 0f);
        Vector3 offset = IsAiming ? _aimOffset : _normalOffset;

        Vector3 targetPosition = _target.position;
        targetPosition.y += _currentCrouchOffset;

        _camera.transform.position = targetPosition + rot * offset;
        _camera.transform.rotation = rot;
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
        Quaternion rot = Quaternion.Euler(_pitch + _recoilPitch, _yaw, 0f);

        Vector3 targetPos = _target.position;
        targetPos.y += _currentCrouchOffset;

        Vector3 offset = IsAiming ? _aimOffset : _normalOffset;

        Vector3 camPos = targetPos + rot * offset;
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
