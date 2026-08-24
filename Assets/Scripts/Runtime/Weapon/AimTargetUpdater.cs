using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimTargetUpdater : MonoBehaviour
{
    [SerializeField] private ThirdPersonCamera _cameraController;

    private void Update()
    {
        transform.position = _cameraController.GetAimPosition();
    }
}
