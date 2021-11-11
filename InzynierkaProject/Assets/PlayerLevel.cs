using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLevel : MonoBehaviour
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

    void Start()
    {
        _exp = 0;
        _level = 1;
        _levelIndicator.text = _level.ToString();
        _level -= 1;
        _expSlider.maxValue = _expValues[0];
        _expSlider.value = _exp;
    }

    public void Update()
    {
        if(Input.GetKey(KeyCode.J))
        {
            AddExp(20);
        }
    }

    void AddExp(int exp)
    {
        _exp += exp;
        if(_exp >= _expValues[_level] && _level < 11)
        {
            _exp = _exp - _expValues[_level];
            _level += 2;
            _levelIndicator.text = (_level).ToString();
            _level -= 1;
            _expSlider.maxValue = _expValues[_level];
        }
        _expSlider.value = _exp;
        if(_level >= 11)
        {
            bg.SetActive(false);
            sld.SetActive(false);
            img.color = Color.grey;
            return;
        }
    }
}
