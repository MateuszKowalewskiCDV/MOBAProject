using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class PlayerLevel : NetworkBehaviour
{
    private int _exp;
    private int _level;
    private int[] _expValues = { 100, 140, 200, 300, 450, 650, 1000, 1500, 2000, 2500, 3000, 5000};
    [SerializeField]
    private TextMeshProUGUI _levelIndicator;
    [SerializeField]
    private Slider _expSlider;
    [SerializeField]
    private GameObject bg, sld;
    [SerializeField]
    private Image img;
    private BeingHP _bh;
    public TextMeshPro playerLevel;

    void Start()
    {
        _bh = GetComponent<BeingHP>();
        _exp = 0;
        _level = 1;
        _levelIndicator.text = _level.ToString();
        _level -= 1;
        _expSlider.maxValue = _expValues[0];
        _expSlider.value = _exp;
    }

    [ClientRpc]
    public void AddExp(int exp, GameObject owner)
    {
        if(isServer)
        {
            var temporary = owner.GetComponent<PlayerLevel>();
            temporary._exp += exp;
            if (temporary._exp >= temporary._expValues[_level] && temporary._level < 11)
            {
                GiveStats(owner);
                temporary._exp = temporary._exp - temporary._expValues[temporary._level];
                temporary._level += 2;
                temporary.playerLevel.text = temporary._level.ToString();
                temporary._levelIndicator.text = temporary._level.ToString();
                temporary._level -= 1;
                temporary._expSlider.maxValue = temporary._expValues[temporary._level];
            }
            temporary._expSlider.value = temporary._exp;
            if (temporary._level >= 11)
            {
                temporary.bg.SetActive(false);
                temporary.sld.SetActive(false);
                return;
            }
        }
    }

    [ClientRpc]
    public void GiveStats(GameObject owner)
    {
        owner.GetComponent<BeingHP>().maxHp += 100;
        owner.GetComponent<BeingHP>().attackBoost += 10;
        owner.GetComponent<BeingHP>().HealUp();

        if (isServer)
            owner.GetComponent<BeingHP>().RPCHpColor();
        else
            owner.GetComponent<BeingHP>().CmdHpColor();
    }
}
