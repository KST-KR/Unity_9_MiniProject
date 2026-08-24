using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region 인스펙터
    [Header("이동 속도")]
    [SerializeField] private float _moveSpeed = 5f;

    [Header("3인칭 카메라")]
    [SerializeField] private ThirdPersonCamera _cameraController;

    [Header("무기")]
    [SerializeField] private Weapon _weapon;
    #endregion

    #region 내부 변수
    private CharacterController _characterController;
    #endregion

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Move();
        Aim();
        Shoot();
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
        if (!Input.GetMouseButton(1))
        {
            return;
        }

        Vector3 lookDir = _cameraController.GetForward();

        if (lookDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }

    private void Shoot()
    {
        if (!Input.GetMouseButton(0))
        {
            return;
        }

        _weapon.Fire();
    }

}