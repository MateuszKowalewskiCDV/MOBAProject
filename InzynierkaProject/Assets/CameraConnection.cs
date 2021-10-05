using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraConnection : MonoBehaviour
{
    private CameraMovement _partner;

    void Start()
    {
        _partner = Camera.main.GetComponent<CameraMovement>();
        if (GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                if (_partner.player == null)
                {
                     Camera.main.GetComponent<CameraMovement>().player = gameObject;
                }
                _partner.cameraOffset = new Vector3(0,20,20);
            }
    }
}
