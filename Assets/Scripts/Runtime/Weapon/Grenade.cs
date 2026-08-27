using UnityEngine;

public class Grenade : MonoBehaviour
{
    #region 인스펙터
    [Header("수류탄")]
    [SerializeField] private float _throwForce = 10f;
    [SerializeField] private float _lifeTime = 3f;

    [Header("폭발")]
    [SerializeField] private float _explosionRadius = 5f;
    [SerializeField] private float _explosionDamage = 50f;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private GameObject _explosionEffect;
    #endregion

    #region 내부변수
    private Rigidbody _rb;
    private float _destroyTime;
    #endregion

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Initialize(Vector3 dir)
    {
        CPrint.Log($"수류탄 방향 : {dir}");
        CPrint.Log($"수류탄 Rigidbody : {_rb}");

        _destroyTime = Time.time + _lifeTime;

        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        _rb.AddForce(dir.normalized * _throwForce, ForceMode.VelocityChange);
    }

    private void Update()
    {
        if (Time.time >= _destroyTime)
        {
            Explode();
        }
    }

    private void Explode()
    {
        Vector3 explosionPos = transform.position;

        CPrint.Log($"수류탄 폭발 위치 : {transform.position}");

        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRadius, _enemyLayer);

        foreach (Collider collider in colliders)
        {
            Health health = collider.GetComponentInParent<Health>();

            if (health == null)
            {
                continue;
            }

            CPrint.Log($"수류탄 피격 : {collider.name}, Damage : {_explosionDamage}");

            health.TakeDamage(_explosionDamage);
        }

        if (_explosionEffect != null)
        {
            Instantiate(_explosionEffect, explosionPos, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}