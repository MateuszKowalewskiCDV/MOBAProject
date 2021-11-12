using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;

public class Abilities : MonoBehaviour
{
    [SerializeField]
    private GameObject _player;

    private bool _skillQReady = true;

    [SerializeField]
    private float _skillQCooldown, _skillWCooldown;

    private bool _skillWCooldownReady = true;

    [SerializeField]
    private int ammo = 0, ammoAmmount = 4, _skillWValue;

    public Image imageSkill1;
    public TextMeshProUGUI skill1Ammount;

    private bool _ammoLoading = false, isCooldownQ, isCooldownW, isCooldownE, isCooldownR;

    private CanvasEnable _partnerCanvasEnable;

    private BeingHP _playerIndicators;

    [SerializeField]
    private int qCost, wCost;

    [SerializeField]
    private int _skillWDuration;

    [SerializeField]
    private NavMeshAgent _playerAgent;

    private float cooldownTimerQ, cooldownTimeQ, cooldownTimerW, cooldownTimeW, cooldownTimerE, cooldownTimeE, cooldownTimerR, cooldownTimeR;

    private TextMeshProUGUI textCooldownQ, textCooldownW, textCooldownE, textCooldownR;
    private Image imageCooldownQ, imageCooldownW, imageCooldownE, imageCooldownR;
    private Image imageEdgeQ, imageEdgeW, imageEdgeE, imageEdgeR;

    private GameObject spellOnCooldownText;

    [SerializeField]
    private Image imageCooldown, imageEdge;

    [SerializeField]
    private KeyCode _keyBindingQ, _keyBindingW, _keyBindingE, _keyBindingR;

    private QAir _airSkill;

    //private QSpell QSpell;
    //private WSpell WSpell;
    //private ESpell ESpell;
    //private RSpell RSpell;

    public void Start()
    {
        _playerIndicators = GetComponent<BeingHP>();
        _partnerCanvasEnable = FindObjectOfType<CanvasEnable>();
        _playerAgent = GetComponent<NavMeshAgent>();

        if (GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                _player = GetComponentInParent<Transform>().gameObject;
            }
    }

    void Update()
    {
        if (Input.GetKeyDown(_keyBindingQ))
        {
            UseSpellQ();
        }
        if (isCooldownQ)
        {
            ApplyCooldownQ();
        }


        if (Input.GetKeyDown(_keyBindingW))
        {
            UseSpellW();
        }
        if (isCooldownW)
        {
            ApplyCooldownW();
        }


        if (Input.GetKeyDown(_keyBindingE))
        {
            UseSpellE();
        }
        if (isCooldownE)
        {
            ApplyCooldownE();
        }


        if (Input.GetKeyDown(_keyBindingR))
        {
            UseSpellR();
        }
        if (isCooldownR)
        {
            ApplyCooldownR();
        }


        //if (ammo < ammoAmmount)
        //{
        //    if(_ammoLoading == false)
        //    {
        //        _ammoLoading = true;
        //        StartCoroutine(CooldownSkillQ());
        //    }
        //}
        //if (Input.GetKeyDown(KeyCode.Q) && ammo > 0)
        //{
        //    if (_playerIndicators.actualMana >= qCost)
        //    {
        //        GetComponent<BeingHP>().LoseMana(qCost);
        //        ammo = ammo - 1;
        //        skill1Ammount.text = ammo.ToString();
        //        ShootMushroom();
        //    }
        //}
    }



    //IEnumerator CooldownSkillQ()
    //{
    //    yield return new WaitForSeconds(_skillQCooldown);
    //    ammo++;
    //    skill1Ammount.text = ammo.ToString();
    //    _ammoLoading = false;
    //}



    void ApplyCooldownQ()
    {
        cooldownTimerQ -= Time.deltaTime;

        if (cooldownTimerQ < 0.0f)
        {
            isCooldownQ = false;
            textCooldownQ.gameObject.SetActive(false);
            imageCooldown.fillAmount = 0.0f;
        }
        else
        {
            textCooldownQ.text = Mathf.RoundToInt(cooldownTimerQ).ToString();
            imageCooldownQ.fillAmount = cooldownTimerQ / cooldownTimeQ;

            imageEdgeQ.transform.localEulerAngles = new Vector3(0, 0, 360f * (cooldownTimerQ / cooldownTimeQ));
        }
    }
    void ApplyCooldownW()
    {
        cooldownTimerW -= Time.deltaTime;

        if (cooldownTimerW < 0.0f)
        {
            isCooldownW = false;
            textCooldownW.gameObject.SetActive(false);
            imageCooldownW.fillAmount = 0.0f;
        }
        else
        {
            textCooldownW.text = Mathf.RoundToInt(cooldownTimerW).ToString();
            imageCooldownW.fillAmount = cooldownTimerW / cooldownTimeW;

            imageEdgeW.transform.localEulerAngles = new Vector3(0, 0, 360f * (cooldownTimerW / cooldownTimeW));
        }
    }
    void ApplyCooldownE()
    {
        cooldownTimerE -= Time.deltaTime;

        if (cooldownTimerE < 0.0f)
        {
            isCooldownE = false;
            textCooldownE.gameObject.SetActive(false);
            imageCooldownE.fillAmount = 0.0f;
        }
        else
        {
            textCooldownE.text = Mathf.RoundToInt(cooldownTimerE).ToString();
            imageCooldownE.fillAmount = cooldownTimerE / cooldownTimeE;

            imageEdgeE.transform.localEulerAngles = new Vector3(0, 0, 360f * (cooldownTimerE / cooldownTimeE));
        }
    }
    void ApplyCooldownR()
    {
        cooldownTimerR -= Time.deltaTime;

        if (cooldownTimerR < 0.0f)
        {
            isCooldownR = false;
            textCooldownR.gameObject.SetActive(false);
            imageCooldownR.fillAmount = 0.0f;
        }
        else
        {
            textCooldownR.text = Mathf.RoundToInt(cooldownTimerR).ToString();
            imageCooldownR.fillAmount = cooldownTimerR / cooldownTimeR;

            imageEdgeR.transform.localEulerAngles = new Vector3(0, 0, 360f * (cooldownTimerR / cooldownTimeR));
        }
    }

    void UseSpellQ()
    {
        if (isCooldownQ)
        {
            StartCoroutine(SpellOnCooldown());
        }
        else
        {
           // qSpell.UseSpell();
        }
    }
    void UseSpellW()
    {
        if (isCooldownW)
        {
            StartCoroutine(SpellOnCooldown());
        }
        else
        {
           // wSpell.UseSpell();
        }
    }
    void UseSpellE()
    {
        if (isCooldownE)
        {
            StartCoroutine(SpellOnCooldown());
        }
        else
        {
           // eSpell.UseSpell();
        }
    }
    void UseSpellR()
    {
        if (isCooldownR)
        {
            StartCoroutine(SpellOnCooldown());
        }
        else
        {
          //  rSpell.UseSpell();
        }
    }

    IEnumerator SpellOnCooldown()
    {
        spellOnCooldownText.SetActive(true);
        yield return new WaitForSeconds(1);
        spellOnCooldownText.SetActive(false);
    }
}
