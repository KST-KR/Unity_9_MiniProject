using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ThirdPersonCamera : MonoBehaviour
{
    #region 인스펙터
    [Header("플레이어")]
    [SerializeField] private Transform _target;

    [Header("일반 카메라")]
    [SerializeField] private Vector3 _normaloffset = new Vector3(0f, 5f, -7f);

    [Header("조준 카메라")]
    [SerializeField] private Vector3 _aimOffset = new Vector3(0.8f, 2f, -3f);

    [Header("카메라 회전")]
    [SerializeField] private float _rotateSpeed = 5f;
    #endregion

    #region 내부 변수
    private Camera _camera;

    private float _yaw;
    private float _pitch;
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

    private void LateUpdate()
    {
        if (_target == null)
        {
            return;
        }

        Rotate();

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

        Vector3 offset = Input.GetMouseButton(1) ? _aimOffset : _normaloffset;

        _camera.transform.position = _target.position + rot * offset;
        _camera.transform.rotation = rot;
    }

    public Vector3 GetForward()
    {
        Vector3 forward = _camera.transform.forward;
        forward.y = 0f;

        return forward.normalized;
    }

    public Vector3 GetRight()
    {
        Vector3 right = _camera.transform.right;
        right.y = 0f;

        return right.normalized;
    }

}
