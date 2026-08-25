using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    #region 인스펙터
    [Header("총알")]
    [SerializeField] private float _moveSpeed = 20f;
    [SerializeField] private float _lifeTime = 3f;
    #endregion

    #region 내부 변수
    private Vector3 _moveDirection;
    private float _destroyTime;
    private BulletPool _pool;
    #endregion

    public void Initialize(Vector3 dir, BulletPool pool)
    {
        _moveDirection = dir.normalized;
        _destroyTime = Time.time + _lifeTime;
        _pool = pool;
    }
    
    void Update()
    {
        if (Time.time >= _destroyTime)
        {
            ReturnToPool();
            return;
        }

        transform.position += transform.forward * _moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(10f);
        }

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        _pool.Return(this);
    }
}
