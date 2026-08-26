using UnityEngine;

public class Bullet : MonoBehaviour
{
    #region 인스펙터
    [Header("총알")]
    [SerializeField] private float _moveSpeed = 20f;
    [SerializeField] private float _lifeTime = 3f;

    [Header("충돌")]
    [SerializeField] private LayerMask _enemyLayer;
    #endregion

    #region 내부 변수
    private float _destroyTime;
    private float _damage;

    private BulletPool _pool;
    #endregion

    public void Initialize(Vector3 dir, float damage, BulletPool pool)
    {
        transform.forward = dir.normalized;

        _destroyTime = Time.time + _lifeTime;
        _damage = damage;
        _pool = pool;
    }

    private void Update()
    {
        if (Time.time >= _destroyTime)
        {
            ReturnToPool();
            return;
        }

        float moveDistance = _moveSpeed * Time.deltaTime;

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, moveDistance, _enemyLayer, QueryTriggerInteraction.Collide))
        {
            CPrint.Log($"총알 Raycast 충돌: {hit.collider.name}");

            Enemy enemy = hit.collider.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                float damage = _damage;

                EnemyHitBox hitBox = hit.collider.GetComponent<EnemyHitBox>();

                if (hitBox != null)
                {
                    damage *= hitBox.DamageMultiplier;

                    CPrint.Log($"헤드샷! Damage : {damage}");
                }
                else
                {
                    CPrint.Log($"Enemy 피격 Damage : {damage}");
                }

                enemy.TakeDamage(damage);
            }

            ReturnToPool();
            return;
        }

        transform.position += transform.forward * _moveSpeed * Time.deltaTime;
    }

    private void ReturnToPool()
    {
        _pool.Return(this);
    }
}