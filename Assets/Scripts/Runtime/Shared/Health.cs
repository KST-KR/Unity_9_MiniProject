using UnityEngine;

public class Health : MonoBehaviour
{
    #region 인스펙터
    [Header("체력")]
    [SerializeField] private float _maxHealth = 100f;
    #endregion

    #region 내부 변수
    private float _currentHealth;
    #endregion

    #region 파라미터
    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;
    #endregion

    #region 이벤트
    public event System.Action Hit;
    public event System.Action Died;
    #endregion

    protected virtual void Awake()
    {
        _currentHealth = _maxHealth;
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
        _currentHealth = _maxHealth;
    }
}