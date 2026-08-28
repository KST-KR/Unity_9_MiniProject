using UnityEngine;

public class RangedEnemyController : MonoBehaviour
{
    #region 인스펙터
    [Header("이동")]
    [SerializeField] private float _moveSpeed = 2f;
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

    private void Awake()
    {
        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
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

        gameObject.SetActive(false);
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
            StopAndLook(moveDir);
            return;
        }

        moveDir.Normalize();

        transform.position += moveDir * _moveSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(moveDir);

        _animator.SetFloat(_hashSpeed, 1f);
    }

    private void StopAndLook(Vector3 targetDir)
    {
        _animator.SetFloat(_hashSpeed, 0f);

        if (targetDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(targetDir);
        }
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
        _hasAttackShot = false;

        _attackEndTime = Time.time + _attackDuration;
        _attackShootEndTime = Time.time + _attackShootTime;

        _animator.SetTrigger(_hashAttack);
    }

    private void UpdateAttack()
    {
        if (!_hasAttackShot && Time.time >= _attackShootEndTime)
        {
            CPrint.Log("Shoot 실행");

            _hasAttackShot = true;
            Shoot();
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

        _hitEndTime = Time.time + _hitDuration;

        _animator.SetFloat(_hashSpeed, 0f);
        _animator.SetTrigger(_hashHit);
    }

    private void OnDied()
    {
        _isDead = true;
        _isHit = false;
        _isAttacking = false;

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

        _animator.SetInteger(_hashEnemyType, 1);
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }
}