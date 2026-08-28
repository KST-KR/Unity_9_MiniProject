using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GrenadeUI : MonoBehaviour
{
    #region 인스펙터
    [Header("수류탄")]
    [SerializeField] private GrenadeController _grenadeController;

    [Header("UI")]
    [SerializeField] private Image _grenadeIcon;
    [SerializeField] private TMP_Text _cooldownText;

    [Header("아이콘 색상")]
    [SerializeField] private Color _readyColor = Color.white;
    [SerializeField] private Color _cooldownColor = Color.gray;
    #endregion

    private void Awake()
    {
        if (_cooldownText == null)
        {
            CPrint.Log("CooldownText 없음");
        }
        else
        {
            _cooldownText.gameObject.SetActive(false);
        }

        if (_grenadeIcon == null)
        {
            CPrint.Log("GrenadeIcon 없음");
        }
        else
        {
            _grenadeIcon.color = _readyColor;
        }

        if (_grenadeController == null)
        {
            CPrint.Log("GrenadeController 없음");
        }
    }

    void Update()
    {
        if (_grenadeController == null)
        {
            return;
        }

        float remainingCooldown = _grenadeController.RemainingCooldown;

        if (remainingCooldown > 0f)
        {
            if (_cooldownText != null)
            {
                _cooldownText.gameObject.SetActive(true);
                _cooldownText.text = Mathf.CeilToInt(remainingCooldown).ToString();
            }

            if (_grenadeIcon != null)
            {
                _grenadeIcon.color = _cooldownColor;
            }
        }
        else
        {
            if (_cooldownText != null)
            {
                _cooldownText.gameObject.SetActive(false);
            }

            if (_grenadeIcon != null)
            {
                _grenadeIcon.color = _readyColor;
            }
        }
    }
}
