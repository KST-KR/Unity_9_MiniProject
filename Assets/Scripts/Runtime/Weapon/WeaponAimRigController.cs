using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponAimRigController : MonoBehaviour
{
    #region 인스펙터
    [Header("무기 Aim Rig")]
    [SerializeField] private Rig _rifleAimRig;
    [SerializeField] private Rig _pistolAimRig;

    [Header("참조")]
    [SerializeField] private ThirdPersonCamera _cameraController;

    [Header("전환 속도")]
    [SerializeField] private float _blendSpeed = 8f;

    [Header("왼손 IK")]
    [SerializeField] private TwoBoneIKConstraint _leftHandIK;
    #endregion

    #region 내부 변수
    private WeaponType _currentWeaponType;
    private bool _isReloading;
    private bool _isThrowingGrenade;
    #endregion

    private void Awake()
    {
        SetRigWeight(_rifleAimRig, 0f);
        SetRigWeight(_pistolAimRig, 0f);
    }

    private void Update()
    {
        UpdateAimRig();
    }

    public void SetWeaponType(WeaponType weaponType)
    {
        _currentWeaponType = weaponType;
    }

    public void UpdateAimRig()
    {
        if (_cameraController == null)
        {
            return;
        }

        if (_isReloading || _isThrowingGrenade)
        {
            _rifleAimRig.weight = Mathf.MoveTowards(_rifleAimRig.weight, 0f, _blendSpeed * Time.deltaTime);

            _pistolAimRig.weight = Mathf.MoveTowards(_pistolAimRig.weight, 0f, _blendSpeed * Time.deltaTime);

            _leftHandIK.weight = Mathf.MoveTowards(_leftHandIK.weight, 0f,_blendSpeed * Time.deltaTime);

            return;
        }

        float rifleTarget = 0f;
        float pistolTarget = 0f;

        if (_cameraController.IsAiming)
        {
            if (_currentWeaponType == WeaponType.Rifle)
            {
                rifleTarget = 1f;
            }
            else if (_currentWeaponType == WeaponType.Pistol)
            {
                pistolTarget = 1f;
            }
        }

        _rifleAimRig.weight = Mathf.MoveTowards(_rifleAimRig.weight, rifleTarget, _blendSpeed * Time.deltaTime);

        _pistolAimRig.weight = Mathf.MoveTowards(_pistolAimRig.weight, pistolTarget, _blendSpeed * Time.deltaTime);

        _leftHandIK.weight = Mathf.MoveTowards(_leftHandIK.weight, 1f, _blendSpeed * Time.deltaTime);
    }

    private void SetRigWeight(Rig rig, float weight)
    {
        if (rig != null)
        {
            rig.weight = weight;
        }
    }

    public void SetLeftHandTarget(Transform target)
    {
        _leftHandIK.data.target = target;
    }

    public void SetReloading(bool isReloading)
    {
        _isReloading = isReloading;
    }

    public void SetThrowingGrenade(bool isThrowingGrenade)
    {
        _isThrowingGrenade = isThrowingGrenade;
    }
}