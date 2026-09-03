using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    #region 인스펙터
    [Header("이동")]
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _attackDistance = 2f;
    [SerializeField] private float _attackInterval = 1.5f;

    [Header("공격")]
    [SerializeField] private float _attackDamage = 10f;
    [SerializeField] private float _attackHitTime = 1f;
    [SerializeField] private float _attackDuration = 3f;

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
    private string _paramEnemyType = "EnemyType";

    private int _hashSpeed;
    private int _hashAttack;
    private int _hashHit;
    private int _hashDeath;
    private int _hashEnemyType;

    private float _nextAttackTime;
    private float _attackEndTime;
    private float _attackDurationEndTime;
    private float _hitEndTime;
    private float _deathEndTime;
    private float _baseAttackDamage;
    private float _currentAttackDamage;

    private bool _isAttacking;
    private bool _isHit;
    private bool _isDead;
    private bool _hasAttackHit;

    private Health _health;
    private Collider[] _colliders;
    private NavMeshAgent _agent;
    #endregion

    #region 이벤트
    public event System.Action DeathAnimationCompleted;
    #endregion

    public void SetDamageMultiplier(float multiplier)
    {
        _currentAttackDamage = _baseAttackDamage * multiplier;

        CPrint.Log($"근거리 적 공격력 설정 : 기본 {_baseAttackDamage} → 현재 {_currentAttackDamage}");
    }

    private void Awake()
    {
        _baseAttackDamage = _attackDamage;
        _currentAttackDamage = _attackDamage;

        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }

        _health = GetComponent<Health>();
        _colliders = GetComponentsInChildren<Collider>();
        _agent = GetComponent<NavMeshAgent>();

        _hashSpeed = Animator.StringToHash(_paramSpeed);
        _hashAttack = Animator.StringToHash(_paramAttack);
        _hashHit = Animator.StringToHash(_paramHit);
        _hashDeath = Animator.StringToHash(_paramDeath);
        _hashEnemyType = Animator.StringToHash(_paramEnemyType);

        _health.Hit += OnHit;
        _health.Died += OnDied;

        if (_agent != null)
        {
            _agent.speed = _moveSpeed;
        }
        else
        {
            CPrint.Warn($"NavMeshAgent 없음 : {name}");
        }
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

        if (Time.time >= _attackDurationEndTime)
        {
            _isAttacking = false;
        }
    }

    private void UpdateCombat()
    {
        Vector3 targetDir = _target.position - transform.position;
        targetDir.y = 0f;

        float distance = targetDir.magnitude;

        if (distance <= _attackDistance)
        {
            StopAgent();
            Attack(targetDir);
            return;
        }

        Move();
    }

    private void Move()
    {
        if (_agent == null)
        {
            return;
        }

        if (!_agent.isOnNavMesh)
        {
            CPrint.Warn($"NavMesh 위에 있지 않음 : {name}");
            return;
        }

        _agent.isStopped = false;
        _agent.SetDestination(_target.position);

        _animator.SetFloat(_hashSpeed, 1f);
    }

    private void StopAgent()
    {
        if (_agent == null)
        {
            return;
        }

        if (!_agent.isOnNavMesh)
        {
            return;
        }

        _agent.isStopped = true;
        _agent.ResetPath();
        _agent.velocity = Vector3.zero;

        _animator.SetFloat(_hashSpeed, 0f);
    }

    private void Attack(Vector3 targetDir)
    {
        CPrint.Log($"공격 거리 진입 : {name}");

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
        _attackDurationEndTime = Time.time + _attackDuration;

        CPrint.Log("근거리 적 Attack 애니메이션 실행");

        _animator.SetTrigger(_hashAttack);
    }

    private void AttackTarget()
    {
        if (_target == null)
        {
            return;
        }

        Vector3 targetDir = _target.position - transform.position;
        targetDir.y = 0f;

        if (targetDir.magnitude > _attackDistance)
        {
            return;
        }

        Health targetHealth = _target.GetComponent<Health>();

        if (targetHealth != null)
        {
            CPrint.Log($"Enemy 공격 Damage : {_currentAttackDamage}");

            targetHealth.TakeDamage(_currentAttackDamage);
        }
    }

    private void OnHit()
    {
        if (_isDead)
        {
            return;
        }

        StopAgent();

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

        StopAgent();

        foreach (Collider collider in _colliders)
        {
            collider.enabled = false;
        }

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

        DeathAnimationCompleted?.Invoke();
    }

    private void OnEnable()
    {
        _isAttacking = false;
        _isHit = false;
        _isDead = false;
        _hasAttackHit = false;

        if (_colliders == null)
        {
            _colliders = GetComponentsInChildren<Collider>();
        }

        foreach (Collider collider in _colliders)
        {
            collider.enabled = true;
        }

        if (_agent == null)
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        if (_agent != null)
        {
            _agent.enabled = true;
            _agent.speed = _moveSpeed;
            _agent.isStopped = false;
            _agent.ResetPath();
        }

        _animator.SetInteger(_hashEnemyType, 0);
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }
}