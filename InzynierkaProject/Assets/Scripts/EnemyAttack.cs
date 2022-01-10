using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;
using Mirror;

public class EnemyAttack : NetworkBehaviour
{
    public MobScriptable _mob;
    public TextMeshPro _nameText;

    public NavMeshAgent _mobAgent;

    private float _timer = 0;
    private Vector3 _startingPoint;

    private Transform _targetedPlayer = null;

    public BeingHP _bh;

    public SpriteRenderer _attackedIndicator;

    public Sprite _calm, _angry;

    private bool _playerInside = false;


    void Start()
    {
        _nameText = GetComponentInChildren<TextMeshPro>();
        _nameText.text = _mob.name;
        _startingPoint = transform.position;
        _mobAgent.speed = _mob.speed;
        _mobAgent.stoppingDistance = _mob.range;
    }

    public void Update()
    {
        if(_bh.isAttacked == true)
        {
            _attackedIndicator.sprite = _angry;
        }
        else if(_playerInside == false)
        {
            _attackedIndicator.sprite = _calm;
        }
        else if(_playerInside == true)
        {
            _attackedIndicator.sprite = _angry;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if ((other.CompareTag("RedTeam") || other.CompareTag("BlueTeam") || other.CompareTag("GreenTeam") || other.CompareTag("YellowTeam")) && _mobAgent.enabled == true)
        {
                if (_targetedPlayer == null || Vector3.Distance(transform.position, other.gameObject.transform.position) < Vector3.Distance(transform.position, _targetedPlayer.transform.position))
                {
                    _targetedPlayer = other.transform;
                }

                if (Mathf.Floor(Vector3.Distance(_targetedPlayer.position, gameObject.transform.position)) >= _mob.range)
                {
                    _playerInside = true;

                    if(isServer)
                        RpcGoToPlayer(_targetedPlayer.gameObject);
                    else
                        CmdGoToPlayer(_targetedPlayer.gameObject);
                }
                else
                {
                    if (isServer)
                        RpcAttack(_targetedPlayer, gameObject);
                    else
                        CmdAttack(_targetedPlayer, gameObject);
                }


            if(!(other.CompareTag("RedTeam") || other.CompareTag("BlueTeam") || other.CompareTag("GreenTeam") || other.CompareTag("YellowTeam")) && _playerInside == false)
            {
                if (isServer)
                    RpcGoBack();
                else
                    CmdGoBack();
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
            if (other.CompareTag("RedTeam") || other.CompareTag("BlueTeam") || other.CompareTag("GreenTeam") || other.CompareTag("YellowTeam"))
            {
                _playerInside = false;
            }

            if (isServer)
                RpcGoBack();
            else
                CmdGoBack();
    }

    [Command]
    void CmdGoToPlayer(GameObject player)
    {
        RpcGoToPlayer(player);
    }

    [ClientRpc]
    void RpcGoToPlayer(GameObject player)
    {
        _attackedIndicator.sprite = _angry;
        if (Vector3.Distance(transform.position, _startingPoint) < _mob.followRange || _bh.isAttacked == true)
        {
            _mobAgent.SetDestination(player.transform.position);
        }
        else
        {
            if (isServer)
                RpcGoBack();
            else
                CmdGoBack();
        }
    }

    [Command]
    void CmdAttack(Transform player, GameObject owner)
    {
        RpcAttack(player, owner);
    }

    [ClientRpc]
    void RpcAttack(Transform player, GameObject owner)
    {
        _timer += Time.deltaTime;
        if(_timer >= _mob.attackSpeed)
        {
            if(isServer)
            {
                DealDamageToPlayer(player.gameObject, owner);
                _timer = 0;
            }
        }
        if(isServer)
            _mobAgent.ResetPath();
    }

    [ClientRpc]
    void DealDamageToPlayer(GameObject player, GameObject owner)
    {
        if(isServer)
            player.gameObject.GetComponent<BeingHP>().LoseHp(_mob.damage, owner);
    }

    [Command]
    void CmdGoBack()
    {
        RpcGoBack();
    }

    [ClientRpc]
    void RpcGoBack()
    {
        if(isServer)
        {
            if (!_bh.isAttacked == true)
            {
                _mobAgent.SetDestination(_startingPoint);
                _bh.RPCHpColor();
                _bh.HealUp();
                _attackedIndicator.sprite = _calm;
            }
        }
    }
}
