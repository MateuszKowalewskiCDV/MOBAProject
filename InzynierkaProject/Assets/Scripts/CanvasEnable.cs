using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CanvasEnable : NetworkBehaviour
{
    private NetworkIdentity _nt;

    public void Start()
    {
        _nt = GetComponentInParent<NetworkIdentity>();

        if (_nt.isLocalPlayer)
            GetComponent<Canvas>().gameObject.SetActive(true);
    }
}