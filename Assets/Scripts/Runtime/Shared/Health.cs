using UnityEngine;

public class Health : MonoBehaviour
{
    #region 인스펙터
    [Header("체력")]
    [SerializeField] private float _maxHealth = 100f;
    #endregion

    #region 내부 변수
    private float _baseMaxHealth;
    private float _currentMaxHealth;
    private float _currentHealth;

    private float _healthMultiplier = 1f;
    #endregion

    #region 프로퍼티
    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _currentMaxHealth;
    #endregion

    #region 이벤트
    public event System.Action Hit;
    public event System.Action Died;
    public event System.Action HealthChanged;
    #endregion

    protected virtual void Awake()
    {
        _baseMaxHealth = _maxHealth;
        _currentMaxHealth = _baseMaxHealth;
        _currentHealth = _currentMaxHealth;
    }

    public void SetHealthMultiplier(float multiplier)
    {
        _healthMultiplier = multiplier;

        _currentMaxHealth = _baseMaxHealth * _healthMultiplier;
        _currentHealth = _currentMaxHealth;

        CPrint.Log($"적 체력 설정 : 기본 {_baseMaxHealth} → 현재 {_currentMaxHealth}");

        HealthChanged?.Invoke();
    }

    public virtual void TakeDamage(float damage)
    {
        if (_currentHealth <= 0f)
        {
            return;
        }

        _currentHealth = Mathf.Max(_currentHealth - damage, 0f);

        CPrint.Log($"피격 Damage : {damage}, Current Health : {_currentHealth}");

        Hit?.Invoke();

        if (_currentHealth <= 0f)
        {
            _currentHealth = 0f;

            CPrint.Log("사망");

            Die();
        }
    }

    protected virtual void Die()
    {
        Died?.Invoke();
    }

    private void OnEnable()
    {
        _currentHealth = _currentMaxHealth;
    }
}