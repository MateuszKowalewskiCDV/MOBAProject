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
    public void AddExp(int exp)
    {
        if(isServer)
        {
            _exp += exp;
            if (_exp >= _expValues[_level] && _level < 11)
            {
                GiveStats();
                _exp = _exp - _expValues[_level];
                _level += 2;
                _levelIndicator.text = (_level).ToString();
                _level -= 1;
                _expSlider.maxValue = _expValues[_level];
            }
            _expSlider.value = _exp;
            if (_level >= 11)
            {
                bg.SetActive(false);
                sld.SetActive(false);
                img.color = Color.grey;
                return;
            }
        }
    }

    [ClientRpc]
    public void GiveStats()
    {
        _bh.maxHp += 100;

        if (isServer)
            _bh.RPCHpColor();
        else
            _bh.CmdHpColor();
    }
}
