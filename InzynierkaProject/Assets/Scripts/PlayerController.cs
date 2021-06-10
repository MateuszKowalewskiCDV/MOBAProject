using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private Camera cam;

    private Transform cameraTransform;

    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private Rigidbody _rb;

    [ClientRpc]
    void Start()
    {
        if(isLocalPlayer)
        {
            agent = GetComponent<NavMeshAgent>();
            agent.enabled = true;
        }
        if (isLocalPlayer)
        {
            cam = Camera.main;
            cameraTransform = cam.gameObject.transform;  //Find main camera which is part of the scene instead of the prefab
        }
        if(!isLocalPlayer)
        {
            GetComponent<PlayerController>().enabled = false;
            GetComponentInChildren<Canvas>().enabled = false;
        }
    _rb = GetComponent<Rigidbody>();
    }


    [ClientCallback]
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if(Physics.Raycast(ray, out hit))
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
