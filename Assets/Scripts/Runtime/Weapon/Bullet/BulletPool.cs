using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    #region 인스펙터
    [Header("총알")]
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private int _poolSize = 20;
    #endregion

    #region 내부 변수
    private List<Bullet> _bullets = new List<Bullet>();
    #endregion

    private void Awake()
    {
        CreatePool();
    }

    private void CreatePool()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            Bullet bullet = Instantiate(_bulletPrefab, transform);

            bullet.gameObject.SetActive(false);

            _bullets.Add(bullet);
        }
    }

    public Bullet Get()
    {
        foreach (Bullet bullet in _bullets)
        {
            if (bullet == null)
            {
                continue;
            }

            if (!bullet.gameObject.activeSelf)
            {
                bullet.gameObject.SetActive(true);

                return bullet;
            }
        }

        return null;
    }

    public void Return(Bullet bullet)
    {
        if (bullet == null)
        {
            return;
        }

        bullet.gameObject.SetActive(false);
    }

}
