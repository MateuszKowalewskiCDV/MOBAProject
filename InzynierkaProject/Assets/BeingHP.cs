using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using UnityEngine.AI;

public class BeingHP : NetworkBehaviour
{
    public GameObject _holder;
    private NetworkConnection _nt;
    public Rigidbody player;
    public SpriteRenderer _hpBar;
    public SpriteRenderer _manaBar;
    public int maxHp;
    public int maxMana;

    [SyncVar]
    public int actualHp;
    [SyncVar]
    public int actualMana;

    [SerializeField]
    private int _hpRegen;
    [SerializeField]
    private int _manaRegen;

    private int _hpRegenTime = 1;
    private int _manaRegenTime = 1;

    private float hpTimer, manaTimer;

    public TextMeshPro _damageIndicator, _damageIndicatorInstance;

    private float startValueHP, startValueMANA;

    public bool _isMob, isAttacked, _isNexus;
    public float _isAttackedTimer;
    public MobScriptable _mob;

    private Vector3 _startingPosition;
    private NavMeshAgent _navMeshAgent;

    public int myBushNumber = 0;

    public int playerRespawnTime;
    public int mobRespawnTime;

    public int expValue;

    public void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _startingPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        if (_isMob)
        {
            startValueHP = _hpBar.transform.localScale.x;
            startValueMANA = _manaBar.transform.localScale.x;
            actualHp = _mob.hp;
            _manaBar.gameObject.SetActive(false);
            player = GetComponent<Rigidbody>();
            _hpBar.color = Color.green;
            _hpBar.transform.localScale = new Vector3(startValueHP / maxHp * actualHp, _hpBar.transform.localScale.y, _hpBar.transform.localScale.z);
        }
        else
        {
            startValueHP = _hpBar.transform.localScale.x;
            startValueMANA = _manaBar.transform.localScale.x;
            actualHp = maxHp;
            actualMana = maxMana;
            player = GetComponent<Rigidbody>();
            _hpBar.color = Color.green;
            _hpBar.transform.localScale = new Vector3(startValueHP / maxHp * actualHp, _hpBar.transform.localScale.y, _hpBar.transform.localScale.z);
        }
    }

    public void Update()
    {
        hpTimer += Time.deltaTime;
        manaTimer += Time.deltaTime;

        if (hpTimer >= _hpRegenTime && actualHp < maxHp)
        {
            actualHp += _hpRegen;
            HpColorSwitch();
            hpTimer = 0;
        }
        if (manaTimer >= _manaRegenTime && actualMana < maxMana)
        {
            actualMana += _manaRegen;
            _manaBar.transform.localScale = new Vector3(startValueMANA / maxMana * actualMana, _manaBar.transform.localScale.y, _manaBar.transform.localScale.z);
            manaTimer = 0;
        }

        if(isAttacked != false)
        {
            isAttackedRecently();
        }
    }

    public int LoseHp(int loss, GameObject who)
    {
        actualHp -= loss;

        _isAttackedTimer = 2;
        isAttacked = true;

        if (actualHp <= 0)
        {
            if(_isMob)
            {
                who.TryGetComponent(out PlayerLevel playerLevel);
                playerLevel.AddExp(expValue);
            }

            StartCoroutine(Respawn());
            return actualHp;
        }

        ShowHpToPlayersSwitch(loss);

        return actualHp;
    }

    public void ShowHpToPlayersSwitch(int loss)
    {
        if (isClient)
            CMDShowHpToPlayers(loss);
        else
            RPCShowHpToPlayers(loss);
    }

    [Command(requiresAuthority = false)]
    void CMDShowHpToPlayers(int loss)
    {
        RPCShowHpToPlayers(loss);
    }

    [ClientRpc]
    public void RPCShowHpToPlayers(int loss)
    {
            HpColorSwitch();
            _damageIndicatorInstance = Instantiate(_damageIndicator, gameObject.transform, false);
            _damageIndicatorInstance.text = loss.ToString();
    }

    public int LoseMana(int loss)
    {
        actualMana = actualMana - loss;
        _manaBar.transform.localScale = new Vector3(startValueMANA / maxMana * actualMana, _manaBar.transform.localScale.y, _manaBar.transform.localScale.z);

        return actualMana;
    }

    public void HpColorSwitch()
    {
        if(isClient)
            CMDHpColor();
        else if(isServer)
            RPCHpColor();
    }

    [Command(requiresAuthority = false)]
    public void CMDHpColor()
    {
        RPCHpColor();
    }

    [ClientRpc]
    public void RPCHpColor()
    {
        if(actualHp > maxHp/2)
        {
            _hpBar.color = Color.green;
        }

        if (actualHp <= maxHp / 2 && actualHp > maxHp / 4)
        {
            _hpBar.color = Color.yellow;
        }

        if (actualHp <= maxHp / 4)
        {
            _hpBar.color = Color.red;
        }

        _hpBar.transform.localScale = new Vector3(startValueHP / maxHp * actualHp, _hpBar.transform.localScale.y, _hpBar.transform.localScale.z);   
    }

    public void HealUp()
    {
        actualHp = maxHp;
    }

    public IEnumerator Respawn()
    {
        if(_isNexus)
        {
            Destroy(gameObject);
        }
        else if(_isMob)
        {
            gameObject.transform.position = _startingPosition;
            _navMeshAgent.enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<BeingHP>().enabled = false;
            GetComponent<EnemyAttack>().enabled = false;
            _holder.SetActive(false);
            yield return new WaitForSeconds(mobRespawnTime);
            _holder.SetActive(true);
            _navMeshAgent.enabled = true;
            GetComponent<MeshRenderer>().enabled = true;
            GetComponent<BeingHP>().enabled = true;
            actualHp = maxHp;
            GetComponent<EnemyAttack>().enabled = true;
            RPCHpColor();
        }
        else 
        {
            gameObject.transform.position = _startingPosition;
            _navMeshAgent.enabled = false;
            GetComponent<PlayerController>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<BeingHP>().enabled = false;
            GetComponent<SkillUsage>().enabled = false;
            _holder.SetActive(false);
            yield return new WaitForSeconds(playerRespawnTime);
            _holder.SetActive(true);
            _navMeshAgent.enabled = true;
            GetComponent<PlayerController>().enabled = true;
            GetComponent<MeshRenderer>().enabled = true;
            GetComponent<BeingHP>().enabled = true;
            actualHp = maxHp;
            HpColorSwitch();
            GetComponent<SkillUsage>().enabled = true;
            RPCHpColor();
        }
    }

    void isAttackedRecently()
    {
        if(_isAttackedTimer >= 0)
        {
            _isAttackedTimer -= Time.deltaTime;
        }

        if(_isAttackedTimer <= 0 && isAttacked != false)
        {
            isAttacked = false;
        }
    }
}
