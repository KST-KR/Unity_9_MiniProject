using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private TMP_Text _ammoText;
    [SerializeField] private Image _weaponIcon;
    [SerializeField] private TMP_Text _reloadText;
    #endregion

    #region 내부 변수
    private Weapon _currentWeapon;
    #endregion

    private void Start()
    {
        UpdateWeapon();

        _playerController.WeaponChanged += UpdateWeapon;
    }

    private void OnDestroy()
    {
        _playerController.WeaponChanged -= UpdateWeapon;

        if (_currentWeapon != null)
        {
            _currentWeapon.AmmoChanged -= UpdateAmmo;
            _currentWeapon.ReloadStarted -= ShowReload;
            _currentWeapon.ReloadCompleted -= HideReload;
        }
    }

    private void UpdateWeapon()
    {
        if (_currentWeapon != null)
        {
            _currentWeapon.AmmoChanged -= UpdateAmmo;
            _currentWeapon.ReloadStarted -= ShowReload;
            _currentWeapon.ReloadCompleted -= HideReload;
        }

        _currentWeapon = _playerController.CurrentWeapon;

        _currentWeapon.AmmoChanged += UpdateAmmo;
        _currentWeapon.ReloadStarted += ShowReload;
        _currentWeapon.ReloadCompleted += HideReload;

        UpdateAmmo(_currentWeapon.CurrentAmmo, _currentWeapon.MagazineSize);

        _weaponIcon.sprite = _currentWeapon.WeaponIcon;

        _reloadText.gameObject.SetActive(_currentWeapon.IsReloading);
    }

    private void UpdateAmmo(int currentAmmo, int magazineSize)
    {
        _ammoText.text = $"{currentAmmo} / {magazineSize}";
    }

    private void ShowReload()
    {
        _reloadText.gameObject.SetActive(true);
    }

    private void HideReload()
    {
        _reloadText.gameObject.SetActive(false);
    }
}