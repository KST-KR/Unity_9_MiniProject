using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    #region 인스펙터
    [Header("플레이어")]
    [SerializeField] private Health _health;

    [Header("체력 UI")]
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private TMP_Text _healthText;
    #endregion

    private void Start()
    {
        if (_health == null)
        {
            return;
        }

        _health.Hit += UpdateHealthUI;
        _health.Died += UpdateHealthUI;

        UpdateHealthUI();
    }

    private void OnDestroy()
    {
        if (_health == null)
        {
            return;
        }

        _health.Hit -= UpdateHealthUI;
        _health.Died -= UpdateHealthUI;
    }

    private void UpdateHealthUI()
    {
        float currentHealth = _health.CurrentHealth;
        float maxHealth = _health.MaxHealth;

        _healthSlider.value = currentHealth / maxHealth;
        _healthText.text = $"{currentHealth:0} / {maxHealth:0}";
    }
}