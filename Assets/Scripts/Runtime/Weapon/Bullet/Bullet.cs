using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum BulletOwner
    {
        Player,
        Enemy
    }

    #region 인스펙터
    [Header("총알")]
    [SerializeField] private float _moveSpeed = 20f;
    [SerializeField] private float _lifeTime = 3f;

    [Header("충돌")]
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private LayerMask _playerLayer;
    #endregion

    #region 내부 변수
    private float _destroyTime;
    private float _damage;

    private BulletPool _pool;
    private BulletOwner _owner;
    #endregion

    public void Initialize(Vector3 dir, float damage, BulletPool pool, BulletOwner owner)
    {
        transform.forward = dir.normalized;

        _destroyTime = Time.time + _lifeTime;
        _damage = damage;
        _pool = pool;
        _owner = owner;
    }

    private void Update()
    {
        if (Time.time >= _destroyTime)
        {
            ReturnToPool();
            return;
        }

        float moveDistance = _moveSpeed * Time.deltaTime;

        LayerMask targetLayer = _owner == BulletOwner.Player ? _enemyLayer : _playerLayer;

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, moveDistance, targetLayer, QueryTriggerInteraction.Collide))
        {
            CPrint.Log($"충돌 오브젝트: {hit.collider.gameObject.name}");
            
            if (_owner == BulletOwner.Player)
            {
                HitEnemy(hit);
            }
            else
            {
                HitPlayer(hit);
            }

            ReturnToPool();
            return;
        }

        transform.position += transform.forward * _moveSpeed * Time.deltaTime;
    }

    private void HitEnemy(RaycastHit hit)
    {
        Enemy enemy = hit.collider.GetComponentInParent<Enemy>();

        if (enemy == null)
        {
            return;
        }

        float damage = _damage;

        EnemyHitBox hitBox = hit.collider.GetComponent<EnemyHitBox>();

        if (hitBox != null)
        {
            damage *= hitBox.DamageMultiplier;

            CPrint.Log($"헤드샷 Damage : {damage}");
        }
        else
        {
            CPrint.Log($"Enemy 피격 Damage : {damage}");
        }

        enemy.TakeDamage(damage);
    }

    private void HitPlayer(RaycastHit hit)
    {
        Health health = hit.collider.GetComponentInParent<Health>();

        if (health == null)
        {
            CPrint.Log($"Enemy 총알이 맞춘 대상에 Health 없음: {hit.collider.name}"); // 임시 추가
            return;
        }

        CPrint.Log($"Player 피격 Damage : {_damage}");

        health.TakeDamage(_damage);
    }

    private void ReturnToPool()
    {
        _pool.Return(this);
    }
}