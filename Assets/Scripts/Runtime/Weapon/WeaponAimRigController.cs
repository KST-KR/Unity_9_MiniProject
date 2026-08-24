using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponAimRigController : MonoBehaviour
{
    [SerializeField] private Rig _weaponAimRig;
    [SerializeField] private ThirdPersonCamera _cameraController;
    [SerializeField] private float _blendSpeed = 8f;

    private void Update()
    {
        float target = _cameraController.IsAiming ? 1f : 0f;
        _weaponAimRig.weight = Mathf.MoveTowards(_weaponAimRig.weight, target, _blendSpeed * Time.deltaTime);
    }
}