using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private float _moveSpeed = 20f;
    [SerializeField] private float _lifeTime = 3f;
    #endregion

    private void OnEnable()
    {
        Invoke(nameof(ReturnBullet), _lifeTime);
    }
    
    void Update()
    {
        transform.position += transform.forward * _moveSpeed * Time.deltaTime;
    }

    private void ReturnBullet()
    {
        gameObject.SetActive(false);
    }
}
