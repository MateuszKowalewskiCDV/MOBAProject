using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class Slime : MonoBehaviour
{
    public int hp;
    public string description;
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    [SerializeField]
    private Sprite _icon;
    public int slimeSpeed;
    public int attackRange;
    private NavMeshAgent _agent;
    [SerializeField]
    public float attackSpeed;
    private float _timer = 0;
    private Vector3 _startingPoint;

    [SerializeField]
    private TextMeshPro nameBox;

    void Awake()
    {
        _startingPoint = transform.position;
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _agent = GetComponent<NavMeshAgent>();
        Enemy slime = new Enemy(hp, "Slime", description, _icon, attackRange);
        _spriteRenderer.sprite = slime.icon;
        nameBox = GetComponentInChildren<TextMeshPro>();
        nameBox.text = slime.name;
    }

    void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            if(Vector3.Distance(other.gameObject.transform.position, gameObject.transform.position) >= attackRange)
            {
                GoToPlayer(other.gameObject);
            }
            else
            {
                Attack();
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            _agent.SetDestination(_startingPoint);
        }
    }

    void GoToPlayer(GameObject player)
    {
        _agent.SetDestination(player.transform.position);
    }

    void Attack()
    {
        _timer += Time.deltaTime;
        if(_timer >= attackSpeed)
        {
            Debug.Log("Attack");
            _timer = 0;
        }
        _agent.SetDestination(transform.position);
    }
}
