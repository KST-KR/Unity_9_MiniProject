using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHairUI : MonoBehaviour
{
    #region ¿ŒΩ∫∆Â≈Õ
    [SerializeField] private ThirdPersonCamera _cameraController;
    [SerializeField] private GameObject _crosshair;
    #endregion

    void Update()
    {
        _crosshair.SetActive(_cameraController.IsAiming);   
    }
}
