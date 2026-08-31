using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyController : MonoBehaviour
{
    #region 인스펙터
    [Header("이동")]
    [SerializeField] private float _minAttackDistance = 6f;
    [SerializeField] private float _attackDistance = 10f;

    [Header("공격")]
    [SerializeField] private float _attackInterval = 1.5f;
    [SerializeField] private float _attackDuration = 0.8f;
    [SerializeField] private float _attackShootTime = 0.4f;

    [Header("피격")]
    [SerializeField] private float _hitDuration = 0.5f;

    [Header("사망")]
    [SerializeField] private float _deathDuration = 1.5f;

    [Header("무기")]
    [SerializeField] private EnemyPistol _pistol;

    [Header("플레이어")]
    [SerializeField] private Transform _target;

    [Header("애니메이터")]
    [SerializeField] private Animator _animator;

    [Header("NavMesh")]
    [SerializeField] private NavMeshAgent _agent;

    [Header("시야 확인")]
    [SerializeField] private float _eyeHeight = 1.5f;
    [SerializeField] private float _targetHeightOffset = 1f;
    [SerializeField] private LayerMask _lineOfSightLayer;

    [Header("시야 레이어")]
    [SerializeField] private LayerMask _coverLayer;
    [SerializeField] private LayerMask _playerLayer;

    [Header("시야 Gizmos")]
    [SerializeField] private bool _showLineOfSightGizmos = true;
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
    private float _attackShootEndTime;
    private float _hitEndTime;
    private float _deathEndTime;

    private bool _isAttacking;
    private bool _hasAttackShot;
    private bool _isHit;
    private bool _isDead;

    private Health _health;
    private Collider[] _colliders;
    #endregion

    #region 이벤트
    public event System.Action DeathAnimationCompleted;
    #endregion

    private void Awake()
    {
        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }

        if (_agent == null)
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        _health = GetComponent<Health>();
        _colliders = GetComponentsInChildren<Collider>();

        _hashSpeed = Animator.StringToHash(_paramSpeed);
        _hashAttack = Animator.StringToHash(_paramAttack);
        _hashHit = Animator.StringToHash(_paramHit);
        _hashDeath = Animator.StringToHash(_paramDeath);
        _hashEnemyType = Animator.StringToHash(_paramEnemyType);

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

    private void UpdateDeath()
    {
        if (Time.time < _deathEndTime)
        {
            return;
        }

        DeathAnimationCompleted?.Invoke();
    }

    private void UpdateCombat()
    {
        Vector3 targetDir = _target.position - transform.position;
        targetDir.y = 0f;

        float distance = targetDir.magnitude;

        if (distance < _minAttackDistance)
        {
            MoveAwayFromTarget();
            return;
        }

        if (distance <= _attackDistance && HasLineOfSight())
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
            StopAndLook(moveDir);
            return;
        }

        moveDir.Normalize();

        if (_agent != null)
        {
            _agent.isStopped = false;
            _agent.SetDestination(_target.position);
        }

        _animator.SetFloat(_hashSpeed, 1f);
    }

    private void MoveAwayFromTarget()
    {
        if (_agent == null)
        {
            return;
        }

        if (!_agent.isOnNavMesh)
        {
            return;
        }

        Vector3 awayDir = transform.position - _target.position;
        awayDir.y = 0f;

        if (awayDir == Vector3.zero)
        {
            return;
        }

        awayDir.Normalize();

        Vector3 destination = transform.position + awayDir * _minAttackDistance;

        _agent.isStopped = false;
        _agent.SetDestination(destination);

        _animator.SetFloat(_hashSpeed, 1f);
    }

    private void StopAndLook(Vector3 targetDir)
    {
        _animator.SetFloat(_hashSpeed, 0f);

        if (_agent != null)
        {
            _agent.isStopped = true;
        }

        if (targetDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(targetDir);
        }
    }

    private void Attack(Vector3 targetDir)
    {
        _animator.SetFloat(_hashSpeed, 0f);

        if (_agent != null)
        {
            _agent.isStopped = true;
        }

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
        _hasAttackShot = false;

        _attackEndTime = Time.time + _attackDuration;
        _attackShootEndTime = Time.time + _attackShootTime;

        _animator.SetTrigger(_hashAttack);
    }

    private void UpdateAttack()
    {
        if (!_hasAttackShot && Time.time >= _attackShootEndTime)
        {
            _hasAttackShot = true;

            if (HasLineOfSight())
            {
                CPrint.Log("Shoot 실행");
                Shoot();
            }
            else
            {
                CPrint.Log("발사 순간 Cover에 막힘 → 발사 취소");
            }
        }

        if (Time.time >= _attackEndTime)
        {
            _isAttacking = false;
        }
    }

    private void Shoot()
    {
        CPrint.Log("Shoot 호출");

        if (_pistol == null)
        {
            CPrint.Log("Enemy_Pistol 없음");
            return;
        }

        if (_target == null)
        {
            CPrint.Log("Target 없음");
            return;
        }

        _pistol.Fire(_target);
    }

    private bool HasLineOfSight()
    {
        if (_target == null)
        {
            return false;
        }

        Vector3 eyePosition = transform.position + Vector3.up * _eyeHeight;
        Vector3 targetPosition = _target.position + Vector3.up * _targetHeightOffset;

        Vector3 direction = targetPosition - eyePosition;

        if (direction == Vector3.zero)
        {
            return true;
        }

        float distance = direction.magnitude;
        direction.Normalize();

        if (!Physics.Raycast(eyePosition, direction, out RaycastHit hit, distance, _lineOfSightLayer, QueryTriggerInteraction.Ignore))
        {
            CPrint.Log("시야 Ray 충돌 없음");
            return false;
        }

        CPrint.Log($"시야 Ray 충돌 : {hit.collider.gameObject.name}");

        int hitLayer = 1 << hit.collider.gameObject.layer;

        if ((_coverLayer & hitLayer) != 0)
        {
            CPrint.Log("Cover에 막힘 → 공격 불가");
            return false;
        }

        if ((_playerLayer & hitLayer) != 0)
        {
            CPrint.Log("Player 확인 → 공격 가능");
            return true;
        }

        CPrint.Log("Cover / Player가 아닌 오브젝트 → 공격 불가");

        return false;
    }

    private void OnHit()
    {
        CPrint.Log("OnHit 호출");

        if (_isDead)
        {
            return;
        }

        CPrint.Log("원거리 적 Hit");

        _isHit = true;
        _isAttacking = false;

        if (_agent != null)
        {
            _agent.isStopped = true;
        }

        _hitEndTime = Time.time + _hitDuration;

        _animator.SetFloat(_hashSpeed, 0f);
        _animator.SetTrigger(_hashHit);
    }

    private void OnDied()
    {
        _isDead = true;
        _isHit = false;
        _isAttacking = false;

        if (_agent != null)
        {
            _agent.isStopped = true;
        }

        foreach (Collider collider in _colliders)
        {
            collider.enabled = false;
        }

        _animator.SetFloat(_hashSpeed, 0f);
        _animator.SetTrigger(_hashDeath);

        _deathEndTime = Time.time + _deathDuration;
    }

    private void OnEnable()
    {
        _isAttacking = false;
        _isHit = false;
        _isDead = false;
        _hasAttackShot = false;

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
            _agent.isStopped = false;
            _agent.ResetPath();
        }

        _animator.SetInteger(_hashEnemyType, 1);
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    private void OnDrawGizmos()
    {
        if (!_showLineOfSightGizmos)
        {
            return;
        }

        if (_target == null)
        {
            return;
        }

        Vector3 eyePosition = transform.position + Vector3.up * _eyeHeight;
        Vector3 targetPosition = _target.position + Vector3.up * _targetHeightOffset;

        Vector3 direction = targetPosition - eyePosition;

        if (direction == Vector3.zero)
        {
            return;
        }

        float distance = direction.magnitude;
        direction.Normalize();

        Gizmos.DrawLine(eyePosition, targetPosition);

        if (Physics.Raycast(eyePosition, direction, out RaycastHit hit, distance, _lineOfSightLayer, QueryTriggerInteraction.Ignore))
        {
            Gizmos.DrawSphere(hit.point, 0.1f);
            Gizmos.DrawLine(eyePosition, hit.point);
        }
    }
}