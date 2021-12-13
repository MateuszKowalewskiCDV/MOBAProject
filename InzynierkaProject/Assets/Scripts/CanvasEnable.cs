using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CanvasEnable : NetworkBehaviour
{
    private NetworkIdentity _nt;
    public Canvas cnv;

    public void Start()
    {
        _nt = GetComponentInParent<NetworkIdentity>();

        if (_nt.isLocalPlayer)
            cnv.gameObject.SetActive(true);

        if (isServer && !_nt.isLocalPlayer)
            cnv.gameObject.SetActive(true);
    }
}