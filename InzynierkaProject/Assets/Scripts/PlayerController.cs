using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private Rigidbody _rb;

    public GameObject movementIndicator;

    void Start()
    {
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
}
