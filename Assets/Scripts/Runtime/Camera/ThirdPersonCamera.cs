using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
    #endregion

    #region 내부 변수
    private Camera _camera;

    private float _yaw;
    private float _pitch;

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
        IsAiming = Input.GetMouseButton(1);
        Rotate();
    }

    private void LateUpdate()
    {
        if (_target == null)
        {
            return;
        }

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

    private void UpdateCameraPosition()
    {
        Quaternion rot = Quaternion.Euler(_pitch, _yaw, 0f);
        Vector3 offset = IsAiming ? _aimOffset : _normalOffset;

        _camera.transform.position = _target.position + rot * offset;
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
        Quaternion rot = Quaternion.Euler(_pitch, _yaw, 0f);
        Vector3 camPos = _target.position + rot * (IsAiming ? _aimOffset : _normalOffset);
        Vector3 camForward = rot * Vector3.forward;

        Ray ray = new Ray(camPos, camForward);

        if (Physics.Raycast(ray, out RaycastHit hit, _aimRaycastDistance, _aimRaycastMask, QueryTriggerInteraction.Ignore))
        {
            return hit.point;
        }

        return ray.GetPoint(_aimRaycastDistance);
    }

}
