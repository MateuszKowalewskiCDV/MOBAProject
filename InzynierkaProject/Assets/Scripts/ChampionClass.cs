using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChampionClass : MonoBehaviour
{
    private bool abilityOneAccess = false, abilityTwoAccess = false;
    private string abilityOne = "", abilityTwo = "";

    [SerializeField]
    private Sprite Archer, Warrior, Mage, Water, Fire, Ground, Air;

    [SerializeField]
    private Image _choosenClass, _choosenStone;

    [SerializeField]
    private GameObject _shop;

    private SendShop _sendShop;

    void Update()
    {
        if (abilityOneAccess == true)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                abilityOne = "Archer";
                _choosenClass.sprite = Archer;
                MyClass(abilityOne, abilityTwo);
                abilityTwoAccess = true;
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                abilityOne = "Mage";
                _choosenClass.sprite = Mage;
                MyClass(abilityOne, abilityTwo);
                abilityTwoAccess = true;
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                abilityOne = "Warrior";
                _choosenClass.sprite = Warrior;
                MyClass(abilityOne, abilityTwo);
                abilityTwoAccess = true;
            }
        }
        if (abilityTwoAccess == true)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                abilityTwo = "Fire";
                _choosenStone.sprite = Fire;
                MyClass(abilityOne, abilityTwo);
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                abilityTwo = "Water";
                _choosenStone.sprite = Water;
                MyClass(abilityOne, abilityTwo);
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                abilityTwo = "Air";
                _choosenStone.sprite = Air;
                MyClass(abilityOne, abilityTwo);
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                abilityTwo = "Ground";
                _choosenStone.sprite = Ground;
                MyClass(abilityOne, abilityTwo);
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.TryGetComponent<SendShop>(out _sendShop);
            _shop = _sendShop.Send();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (_shop == null)
                return;
            _shop.SetActive(true);
            abilityOneAccess = true;
            abilityTwoAccess = true;
        }
    }   

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (_shop == null)
                return;
            _shop.SetActive(false);
            abilityOneAccess = false;
            abilityTwoAccess = false;
        }
    }

    public string MyClass(string abilityOne, string abilityTwo)
    {
        string myClass = "";
        myClass = abilityOne.ToString() + " " + abilityTwo.ToString();
        Debug.Log(myClass);
        return myClass;
    }
}
