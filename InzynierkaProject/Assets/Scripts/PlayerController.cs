using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private Camera cam;

    public NavMeshAgent agent;
    public BellInteraction bell;
    public float bellDistance;

    [SerializeField]
    private Rigidbody _rb;

    public GameObject movementIndicator;

    public AutoAttack aa;

    void Start()
    {
        bell = FindObjectOfType<BellInteraction>();
        aa = GetComponent<AutoAttack>();
        if(isLocalPlayer)
        {
            gameObject.layer = 8;
            agent = GetComponent<NavMeshAgent>();
            agent.enabled = true;
            _rb = GetComponent<Rigidbody>();
            cam = Camera.main;
        }
        if(!isLocalPlayer)
        {
            gameObject.layer = 12;
            GetComponent<PlayerController>().enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if(Physics.Raycast(ray, out hit) && !hit.transform.gameObject.CompareTag("Enemy"))
            {
                agent.SetDestination(hit.point);
            }

            if(Physics.Raycast(ray, out hit) && hit.transform.gameObject.CompareTag("Bell"))
            {
                if(Vector3.Distance(bell.gameObject.transform.position, transform.position) <= bellDistance)
                    bell.PlayBell(gameObject.tag);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        _rb.isKinematic = true;
    }
    void OnCollisionExit(Collision collision)
    {
        _rb.isKinematic = false;
    }

    public void OnAttack(GameObject enemy)
    {
        agent.SetDestination(enemy.transform.position);
    }

    public void OnAttackRange()
    {
        agent.SetDestination(transform.position);
    }
}
