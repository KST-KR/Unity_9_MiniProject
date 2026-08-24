using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region 인스펙터
    [Header("체력")]
    [SerializeField] private float _maxHealth = 100.0f;

    [Header("이동")]
    [SerializeField] private float _moveSpeed = 2f;

    [Header("플레이어")]
    [SerializeField] private Transform _target;
    #endregion

    #region 내부 변수
    private float _currentHealth;
    #endregion

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    void Update()
    {
        Move();    
    }

    private void Move()
    {
        if (_target == null)
        {
            return;
        }

        Vector3 moveDir = _target.position - transform.position;
        moveDir.y = 0.0f;

        if (moveDir == Vector3.zero)
        {
            return;
        }

        moveDir.Normalize();

        transform.position += moveDir * _moveSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(moveDir);
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;

        if (_currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        gameObject.SetActive(false);
    }
}
