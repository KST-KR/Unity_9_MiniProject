using UnityEngine;
using UnityEngine.Animations;

public class PlayerController : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private float _moveSpeed = 5f;
    #endregion

    #region 내부 변수
    private CharacterController _characterController;
    private Camera _camera;
    #endregion

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _camera = Camera.main;
    }

    private void Update()
    {
        Move();
        Aim();
    }

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDir = new Vector3(horizontal, 0f, vertical).normalized;

        _characterController.Move(moveDir * _moveSpeed * Time.deltaTime);
    }

    private void Aim()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 aimPos = ray.GetPoint(distance);

            Vector3 lookDir = aimPos - transform.position;
            lookDir.y = 0f;

            if (lookDir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDir);
            }
        }
    }
}