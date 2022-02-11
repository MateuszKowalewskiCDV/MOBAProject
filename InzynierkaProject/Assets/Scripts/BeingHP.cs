using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using UnityEngine.AI;

public class BeingHP : NetworkBehaviour
{
    public Animator am;
    public int attackBoost;
    public GameObject _holder;
    public TextMeshPro _hpIndicator;
    private NetworkConnection _nt;
    public Rigidbody player;
    public SpriteRenderer _hpBar;
    public SpriteRenderer _manaBar;
    public int maxHp;
    public int maxMana;
    public float speed;
    public string myTeamColor;

    public SkinnedMeshRenderer smr;
    public MeshRenderer[] mr;
    public bool smrModel, mrModel;

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

    public bool _isMob, isAttacked, _isNexus;
    public float _isAttackedTimer;
    public MobScriptable _mob;

    private Vector3 _startingPosition;
    private NavMeshAgent _navMeshAgent;

    public int myBushNumber = 0;

    public int playerRespawnTime;
    public int mobRespawnTime;

    public bool isAlive;
    private bool auraCheck;

    public void Start()
    {
        isAlive = true;
        _startingPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        if (_isMob)
        {
            myTeamColor = "noTeam";
            maxHp = _mob.hp;
            actualHp = _mob.hp;
            _manaBar.gameObject.SetActive(false);
            player = GetComponent<Rigidbody>();
            _hpBar.color = Color.green;
            _hpBar.transform.localScale = new Vector3(0.08f / maxHp * actualHp, _hpBar.transform.localScale.y, _hpBar.transform.localScale.z);
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _navMeshAgent.speed = speed;
            mobRespawnTime = _mob.spawnTime;
        }
        else if(_isNexus)
        {
            actualHp = maxHp;
            player = GetComponent<Rigidbody>();
            _hpBar.color = Color.green;
            _hpBar.transform.localScale = new Vector3(0.08f / maxHp * actualHp, _hpBar.transform.localScale.y, _hpBar.transform.localScale.z);
        }
        else
        {
            myTeamColor = gameObject.tag;
            actualHp = maxHp;
            actualMana = maxMana;
            player = GetComponent<Rigidbody>();
            _hpBar.color = Color.green;
            _hpBar.transform.localScale = new Vector3(0.08f / maxHp * actualHp, _hpBar.transform.localScale.y, _hpBar.transform.localScale.z);
            _manaBar.transform.localScale = new Vector3(0.08f / maxMana * actualMana, _manaBar.transform.localScale.y, _manaBar.transform.localScale.z);
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _navMeshAgent.speed = speed;
        }

        _hpIndicator.text = actualHp.ToString();
    }

    public void Update()
    {
        hpTimer += Time.deltaTime;
        manaTimer += Time.deltaTime;

        if (hpTimer >= _hpRegenTime && actualHp < maxHp)
        {
            if(actualHp + _hpRegen > maxHp)
            {
                actualHp = maxHp;
            }
            else
            {
                actualHp += _hpRegen;
            }

            if (isServer)
                RPCHpColor();
            hpTimer = 0;
        }

        if (manaTimer >= _manaRegenTime && actualMana < maxMana)
        {
            actualMana += _manaRegen;
            _manaBar.transform.localScale = new Vector3(0.08f / maxMana * actualMana, _manaBar.transform.localScale.y, _manaBar.transform.localScale.z);
            manaTimer = 0;
        }

        if(isAttacked != false)
        {
            if(isServer)
                isAttackedRecently();
        }
    }

    public void LoseHp(int loss, GameObject ownerOfAttack)
    {
        if(!ownerOfAttack.CompareTag(myTeamColor))
        {
            actualHp -= loss;
            _hpIndicator.text = actualHp.ToString();

            _isAttackedTimer = 2;
            isAttacked = true;

            if (actualHp <= 0)
            {
                if (_isMob)
                {
                    if (isServer)
                    {
                        ownerOfAttack.TryGetComponent(out PlayerLevel playerLevel);
                        playerLevel.AddExp(_mob.expValue, ownerOfAttack);
                    }
                }

                if (isServer)
                {
                    StartCoroutine(Respawn());
                }
            }

            ShowHpToPlayersSwitch(loss);
        }
    }

    public void ShowHpToPlayersSwitch(int loss)
    {
        if(isServer)
        {
            RPCShowHpToPlayers(loss);
        }
        else
        {
            CMDShowHpToPlayers(loss);
        }
    }

    void CMDShowHpToPlayers(int loss)
    {
        RPCShowHpToPlayers(loss);
    }

    public void RPCShowHpToPlayers(int loss)
    {
        if(isServer)
        {
            RPCHpColor();

            var _damageIndicatorInstance = Instantiate(_damageIndicator, gameObject.transform, false);
            _damageIndicatorInstance.text = loss.ToString();
            NetworkServer.Spawn(_damageIndicatorInstance.gameObject);
        }
    }

    public int LoseMana(int loss)
    {
        actualMana = actualMana - loss;
        _manaBar.transform.localScale = new Vector3(0.08f / maxMana * actualMana, _manaBar.transform.localScale.y, _manaBar.transform.localScale.z);

        return actualMana;
    }

    public void CmdHpColor()
    {
        RPCHpColor();
    }

    public void RPCHpColor()
    {
        
        if (actualHp > maxHp/2)
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

        _hpIndicator.text = actualHp.ToString();
        _hpBar.transform.localScale = new Vector3(0.08f / maxHp * actualHp, _hpBar.transform.localScale.y, _hpBar.transform.localScale.z);
    }

    public void HealUp()
    {
        actualHp = maxHp;
        _hpIndicator.text = actualHp.ToString();

        if (isServer)
            RPCHpColor();
        else
            CmdHpColor();
    }

    public IEnumerator Respawn()
    {
        if(isServer)
        {
            if (_isNexus)
            {
                NexusDown();
            }
            else if (_isMob)
            {
                MobRespawn();

                yield return new WaitForSeconds(mobRespawnTime);

                MobRespawn2();
            }
            else
            {
                PlayerRespawn();

                yield return new WaitForSeconds(playerRespawnTime);

                PlayerRespawn2();
            }
        }
    }

    void MobRespawn()
    {
        isAlive = false;
        gameObject.transform.position = _startingPosition;
        _navMeshAgent.enabled = false;
        if (smrModel == true)
            smr.enabled = false;
        if (mrModel == true)
        {
            for (int i = 2; i >= 0; i--)
            {
                mr[i].enabled = false;
            }
        }
        GetComponent<BeingHP>().enabled = false;
        GetComponent<EnemyAttack>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
        _holder.SetActive(false);
    }

    void MobRespawn2()
    {
        isAlive = true;
        _holder.SetActive(true);
        _navMeshAgent.enabled = true;
        GetComponent<CapsuleCollider>().enabled = true;
        if (smrModel == true)
            smr.enabled = false;
        if (mrModel == true)
        {
            for (int i = 2; i >= 0; i--)
            {
                mr[i].enabled = true;
            }
        }
        GetComponent<BeingHP>().enabled = true;
        actualHp = maxHp;
        GetComponent<EnemyAttack>().enabled = true;
        am.SetInteger("AnimationState", 0);
        if (isServer)
            RPCHpColor();
        else
            CmdHpColor();
    }

    void PlayerRespawn()
    {
        isAlive = false;
        gameObject.transform.position = _startingPosition;
        _navMeshAgent.enabled = false;
        GetComponent<PlayerController>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<BeingHP>().enabled = false;
        GetComponent<SkillUsage>().enabled = false;
        _holder.SetActive(false);
    }

    void PlayerRespawn2()
    {
        isAlive = true;
        _holder.SetActive(true);
        _navMeshAgent.enabled = true;
        GetComponent<PlayerController>().enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<BeingHP>().enabled = true;
        actualHp = maxHp;
        GetComponent<SkillUsage>().enabled = true;
        if (isServer)
            RPCHpColor();
        else
            CmdHpColor();
    }

    void NexusDown()
    {
        Destroy(gameObject);
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

    public void AuraBuff(float value, GameObject gameObject)
    {
        StartCoroutine(AuraBuffCoroutine(value, gameObject));
    }

    public IEnumerator AuraBuffCoroutine(float value, GameObject gameObject)
    {
        if(auraCheck == false)
        {
            _navMeshAgent.speed *= value;
            auraCheck = true;
            yield return new WaitForSeconds(0.5f);
            _navMeshAgent.speed /= value;
            auraCheck = false;
        }
    }
}
