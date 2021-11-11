using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    public MobScriptable _mob;
    public TextMeshPro _nameText;
    private NavMeshAgent _mobAgent;

    // Timer oraz pozycja startowa
    private float _timer = 0;
    private Vector3 _startingPoint;

    // String przechowujący tag
    private string _player = "Player";

    private Transform _targetedPlayer = null;
    private BeingHP _bh;


    void Start()
    {
        _nameText = GetComponentInChildren<TextMeshPro>();
        _nameText.text = _mob.name;
        _startingPoint = transform.position;

        _mobAgent = GetComponent<NavMeshAgent>();
        _bh = GetComponent<BeingHP>();

        _mobAgent.speed = _mob.speed;
        _mobAgent.stoppingDistance = _mob.range;
    }
    
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(_player))
        {
            if(_targetedPlayer == null || Vector3.Distance(transform.position, other.gameObject.transform.position) < Vector3.Distance(transform.position, _targetedPlayer.transform.position))
            {
                _targetedPlayer = other.transform;
            }

            if (Mathf.Floor(Vector3.Distance(_targetedPlayer.position, gameObject.transform.position)) >= _mob.range)
            {
                GoToPlayer(_targetedPlayer.gameObject);
            }
            else
            {
                Attack(_targetedPlayer);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        GoBack();
    }

    void GoToPlayer(GameObject player)
    {    
        if(Vector3.Distance(transform.position, _startingPoint) < _mob.followRange || _bh.isAttacked == true)
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
            player.GetComponent<BeingHP>().LoseHp(_mob.damage);
            _timer = 0;
        }
        _mobAgent.ResetPath();
    }

    void GoBack()
    {
        _mobAgent.SetDestination(_startingPoint);
        _bh.HpColorSwitch();
        _bh.HealUp();
    }
}
