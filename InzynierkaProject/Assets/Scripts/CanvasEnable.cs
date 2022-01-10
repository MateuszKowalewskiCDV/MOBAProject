using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CanvasEnable : MonoBehaviour
{
    private NetworkIdentity _nt;
    public Canvas cnv;

    public void Start()
    {
        _nt = GetComponent<NetworkIdentity>();

        if (_nt.isLocalPlayer)
            cnv.gameObject.SetActive(true);
    }
}