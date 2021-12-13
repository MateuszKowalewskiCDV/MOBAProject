using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class SendShop : NetworkBehaviour
{
    [SerializeField]
    private GameObject _shop;
    public Button archer, warrior, mage, support;
    public Image choosenClass;

    public void Start()
    {
        
    }

    public GameObject Send()
    {
        if (isLocalPlayer)
            return _shop;
        else
            return null;
    }
}
