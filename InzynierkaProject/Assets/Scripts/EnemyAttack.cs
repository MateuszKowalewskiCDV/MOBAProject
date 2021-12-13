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
            
            if(_targetedPlayer == null || Vector3.Distance(transform.position, other.gameObject.transform.position) < Vector3.Distance(transform.position, _targetedPlayer.transform.position))
            {
                _targetedPlayer = other.transform;
            }

            if (Mathf.Floor(Vector3.Distance(_targetedPlayer.position, gameObject.transform.position)) >= _mob.range)
            {
                _playerInside = true;
                GoToPlayer(_targetedPlayer.gameObject);
            }
            else
            {
                Attack(_targetedPlayer);
            }
        }

        if(!(other.CompareTag("RedTeam") || other.CompareTag("BlueTeam") || other.CompareTag("GreenTeam") || other.CompareTag("YellowTeam")) && _playerInside == false)
        {
            GoBack();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("RedTeam") || other.CompareTag("BlueTeam") || other.CompareTag("GreenTeam") || other.CompareTag("YellowTeam"))
        {
            _playerInside = false;
        }
        GoBack();
    }

    void GoToPlayer(GameObject player)
    {
        _attackedIndicator.sprite = _angry;
        if (Vector3.Distance(transform.position, _startingPoint) < _mob.followRange || _bh.isAttacked == true)
        {
            _mobAgent.SetDestination(player.transform.position);
        }
        else
        {
            GoBack();
        }
    }

    void Attack(Transform player)
    {
        _timer += Time.deltaTime;
        if(_timer >= _mob.attackSpeed)
        {
            player.GetComponent<BeingHP>().LoseHp(_mob.damage, gameObject);
            _timer = 0;
        }
        _mobAgent.ResetPath();
    }

    void GoBack()
    {
        if(!_bh.isAttacked == true)
        {
            _mobAgent.SetDestination(_startingPoint);
            _bh.HpColorSwitch();
            _bh.HealUp();
            _attackedIndicator.sprite = _calm;
        }
    }
}
