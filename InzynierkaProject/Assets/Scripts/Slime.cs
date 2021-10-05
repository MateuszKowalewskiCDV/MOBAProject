using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class Slime : MonoBehaviour
{
    //Opis przecinika
    private string _slime = "Slime";
    public int hp;
    public string description;
    public int slimeSpeed;
    public int attackRange;
    public float attackSpeed;
    [SerializeField]
    private int _campRange;

    [SerializeField]
    private Sprite _icon;

    private SpriteRenderer _spriteRenderer;
    private NavMeshAgent _agent;

    // Timer oraz pozycja startowa
    private float _timer = 0;
    private Vector3 _startingPoint;

    // TMP na nazwę
    private TextMeshPro _nameBox;

    // String przechowujący tag
    private string _player = "Player";

    private Transform _targetedPlayer = null;


    void Awake()
    {
        _startingPoint = transform.position;

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _nameBox = GetComponentInChildren<TextMeshPro>();
        _agent = GetComponent<NavMeshAgent>();

        Enemy slime = new Enemy(hp, _slime, description, _icon, attackRange);

        _spriteRenderer.sprite = slime.icon;
        _nameBox.text = slime.name;

        _agent.speed = slimeSpeed;
        _agent.stoppingDistance = attackRange;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(_player))
        {
            if(_targetedPlayer == null || Vector3.Distance(transform.position, other.gameObject.transform.position) < Vector3.Distance(transform.position, _targetedPlayer.transform.position))
            {
                _targetedPlayer = other.transform;
            }
            //Debug.Log(Vector3.Distance(_targetedPlayer.position, gameObject.transform.position));
            Debug.Log(gameObject.transform.position + " :Moja pozycja");
            Debug.Log(_targetedPlayer.transform.position + " :Wybranego gracza");

            if (Mathf.Floor(Vector3.Distance(_targetedPlayer.position, gameObject.transform.position)) >= attackRange)
            {
                GoToPlayer(_targetedPlayer.gameObject);
            }
            else
            {
                Attack(_targetedPlayer);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag(_player))
        {
            GoBack();
        }
    }

    void GoToPlayer(GameObject player)
    {
        Debug.Log("Siemka");       
        if(Vector3.Distance(transform.position, _startingPoint) < _campRange)
        {
            _agent.SetDestination(player.transform.position);
        }
        else
        {
            GoBack();
        }
    }

    void Attack(Transform player)
    {
        _timer += Time.deltaTime;
        if(_timer >= attackSpeed)
        {
            // Tu będzie atak
            Debug.Log("Attack " + player.gameObject.name);
            _timer = 0;
        }
        _agent.ResetPath();
    }

    void GoBack()
    {
        _agent.SetDestination(_startingPoint);
    }
}
