using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class ChampionClass : NetworkBehaviour
{
    private bool classAccess = false;
    [SerializeField]
    private string classOfPlayer = "";
    public string myTeam;

    [SerializeField]
    private Sprite Archer, Warrior, Mage, Support;

    [SerializeField]
    private Image _choosenClass;

    [SerializeField]
    private GameObject _shop;

    private SendShop _sendShop;

    public ClassManager cm;

    [SerializeField]
    private bool notSet = true;

    public void OnTriggerEnter(Collider other)
    {
        
        if (other.GetComponent<NetworkIdentity>().isLocalPlayer || other.GetComponent<NetworkIdentity>().isServer)
        {
            cm = other.GetComponent<ClassManager>();
            if (other.gameObject.tag == myTeam)
            {
                other.TryGetComponent<SendShop>(out _sendShop);
                _shop = _sendShop.Send();
            }
        }

        if (isLocalPlayer && notSet == true || isServer && notSet == true)
        {
            _sendShop.archer.onClick.AddListener(ChangeClassToArcher);
            _sendShop.warrior.onClick.AddListener(ChangeClassToWarrior);
            _sendShop.mage.onClick.AddListener(ChangeClassToMage);
            _sendShop.support.onClick.AddListener(ChangeClassToSupport);
            notSet = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            if (other.gameObject.tag == myTeam)
            {
                if (_shop == null)
                    return;
                _shop.SetActive(true);
                classAccess = true;
            }
        }
    }   

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            if (other.gameObject.tag == myTeam)
            {
                if (_shop == null)
                    return;
                _shop.SetActive(false);
                classAccess = false;
            }
        }
    }

    public void ChangeClassToArcher()
    {
        cm.SetClass(1);
        classOfPlayer = "Archer";
        _sendShop.choosenClass.sprite = Archer;
        if (isLocalPlayer || isServer)
            _sendShop.archer.interactable = false;
        _sendShop.warrior.interactable = false;
        _sendShop.mage.interactable = false;
        _sendShop.support.interactable = false;
    }

    public void ChangeClassToWarrior()
    {
        cm.SetClass(3);
        classOfPlayer = "Warrior";
        _sendShop.choosenClass.sprite = Warrior;
        if (isLocalPlayer || isServer)
            _sendShop.warrior.interactable = false;
        _sendShop.archer.interactable = false;
        _sendShop.mage.interactable = false;
        _sendShop.support.interactable = false;
    }

    public void ChangeClassToMage()
    {
        cm.SetClass(2);
        classOfPlayer = "Mage";
        _sendShop.choosenClass.sprite = Mage;
        if (isLocalPlayer || isServer)
            _sendShop.mage.interactable = false;
        _sendShop.archer.interactable = false;
        _sendShop.warrior.interactable = false;
        _sendShop.support.interactable = false;
    }

    public void ChangeClassToSupport()
    {
        cm.SetClass(4);
        classOfPlayer = "Support";
        _sendShop.choosenClass.sprite = Support;
        if (isLocalPlayer || isServer)
            _sendShop.support.interactable = false;
        _sendShop.archer.interactable = false;
        _sendShop.warrior.interactable = false;
        _sendShop.mage.interactable = false;
    }
}
