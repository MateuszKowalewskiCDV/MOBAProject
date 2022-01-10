using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class HoverTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private GameObject _tipWindow;

    [SerializeField]
    private Image _imageSplash, _image, _imageBg;

    [SerializeField]
    private TextMeshProUGUI _skillDescription, _skillManaCost, skillCooldown, _actualCooldown;

    public Spell spell;

    [SerializeField]
    private SkillUsage _sklUsg;

    public float timer;

    void Start()
    {
        _imageSplash.sprite = spell.sprite;
        _imageBg.sprite = spell.sprite;
    }

    void Update()
    {
        if(_sklUsg.cooldownReady == false)
        {
            if(timer <= 0)
            {
                timer = spell.cooldown;
            }
            _actualCooldown.text = Mathf.Round(timer).ToString();
            timer -= Time.deltaTime;
            _imageSplash.fillAmount = timer/spell.cooldown;
        }
        else
        {
            _actualCooldown.text = null;
            _imageSplash.fillAmount = 1;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _tipWindow.SetActive(true);
        _image.sprite = spell.sprite;
        _skillDescription.text = spell.description;
        _skillManaCost.text = spell.manaCost.ToString();
        skillCooldown.text = spell.cooldown.ToString();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tipWindow.SetActive(false);
    }
}
