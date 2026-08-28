using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    #region 인스펙터
    [Header("스폰 위치")]
    [SerializeField] private List<Transform> _spawnPoints;
    #endregion

    #region 프로퍼티
    public List<Transform> SpawnPoints => _spawnPoints;
    #endregion
}
