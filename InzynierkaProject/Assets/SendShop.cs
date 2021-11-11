using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SendShop : NetworkBehaviour
{
    [SerializeField]
    private GameObject _shop;

    public GameObject Send()
    {
        if (isLocalPlayer)
            return _shop;
        else
            return null;
    }
}
