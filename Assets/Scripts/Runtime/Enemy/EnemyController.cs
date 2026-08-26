using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region 인스펙터
    [Header("이동")]
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _attackDistance = 2f;
    [SerializeField] private float _attackInterval = 1.5f;

    [Header("공격")]
    [SerializeField] private float _attackDamage = 10f;
    [SerializeField] private float _attackHitTime = 0.5f;

    [Header("피격")]
    [SerializeField] private float _hitDuration = 0.5f;

    [Header("사망")]
    [SerializeField] private float _deathDuration = 1.5f;

    [Header("플레이어")]
    [SerializeField] private Transform _target;

    [Header("애니메이터")]
    [SerializeField] private Animator _animator;
    #endregion

    #region 내부 변수
    private string _paramSpeed = "Speed";
    private string _paramAttack = "Attack";
    private string _paramHit = "Hit";
    private string _paramDeath = "Death";

    private int _hashSpeed;
    private int _hashAttack;
    private int _hashHit;
    private int _hashDeath;

    private float _nextAttackTime;
    private float _attackEndTime;
    private float _hitEndTime;
    private float _deathEndTime;

    private bool _isAttacking;
    private bool _isHit;
    private bool _isDead;
    private bool _hasAttackHit;

    private Health _health;
    private Collider _collider;
    #endregion

    private void Awake()
    {
        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }

        _health = GetComponent<Health>();
        _collider = GetComponent<Collider>();

        _hashSpeed = Animator.StringToHash(_paramSpeed);
        _hashAttack = Animator.StringToHash(_paramAttack);
        _hashHit = Animator.StringToHash(_paramHit);
        _hashDeath = Animator.StringToHash(_paramDeath);

        _health.Hit += OnHit;
        _health.Died += OnDied;
    }

    private void Update()
    {
        if (_isDead)
        {
            UpdateDeath();
            return;
        }

        if (_isHit)
        {
            UpdateHit();
            return;
        }

        if (_isAttacking)
        {
            UpdateAttack();
            return;
        }

        if (_target == null)
        {
            return;
        }

        UpdateCombat();
    }

    private void UpdateHit()
    {
        if (Time.time < _hitEndTime)
        {
            return;
        }

        _isHit = false;
    }

    private void UpdateAttack()
    {
        if (!_hasAttackHit && Time.time >= _attackEndTime)
        {
            _hasAttackHit = true;
            AttackTarget();
        }
    }

    private void UpdateCombat()
    {
        Vector3 targetDir = _target.position - transform.position;
        targetDir.y = 0f;

        float distance = targetDir.magnitude;

        if (distance <= _attackDistance)
        {
            Attack(targetDir);
            return;
        }

        Move(targetDir);
    }

    private void Move(Vector3 moveDir)
    {
        if (moveDir == Vector3.zero)
        {
            _animator.SetFloat(_hashSpeed, 0f);
            return;
        }

        moveDir.Normalize();

        transform.position += moveDir * _moveSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(moveDir);

        _animator.SetFloat(_hashSpeed, 1f);
    }

    private void Attack(Vector3 targetDir)
    {
        _animator.SetFloat(_hashSpeed, 0f);

        if (targetDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(targetDir);
        }

        if (Time.time < _nextAttackTime)
        {
            return;
        }

        _nextAttackTime = Time.time + _attackInterval;

        _isAttacking = true;
        _hasAttackHit = false;
        _attackEndTime = Time.time + _attackHitTime;

        _animator.SetTrigger(_hashAttack);
    }

    private void AttackTarget()
    {
        if (_target == null)
        {
            _isAttacking = false;
            return;
        }

        Vector3 targetDir = _target.position - transform.position;
        targetDir.y = 0f;

        if (targetDir.magnitude > _attackDistance)
        {
            _isAttacking = false;
            return;
        }

        Health targetHealth = _target.GetComponent<Health>();

        if (targetHealth != null)
        {
            CPrint.Log($"Enemy 공격 Damage : {_attackDamage}");

            targetHealth.TakeDamage(_attackDamage);
        }

        _isAttacking = false;
    }

    private void OnHit()
    {
        if (_isDead)
        {
            return;
        }

        _isHit = true;
        _isAttacking = false;

        _hitEndTime = Time.time + _hitDuration;

        _animator.SetFloat(_hashSpeed, 0f);
        _animator.SetTrigger(_hashHit);
    }

    private void OnDied()
    {
        _isDead = true;
        _isHit = false;
        _isAttacking = false;

        _collider.enabled = false;

        _animator.SetFloat(_hashSpeed, 0f);
        _animator.SetTrigger(_hashDeath);

        _deathEndTime = Time.time + _deathDuration;
    }

    private void UpdateDeath()
    {
        if (Time.time < _deathEndTime)
        {
            return;
        }

        gameObject.SetActive(false);
    }
}