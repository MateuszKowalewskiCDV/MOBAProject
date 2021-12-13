using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TowerShotPrefab : NetworkBehaviour
{
    public BeingHP target;
    public Rigidbody rb;
    public Vector3 tempVect;
    public bool onRange;

    public float speed;

    public string team1, team2, team3;
    private Vector3 startingPosition;
    private float timer;
    public float castTime;
    public int damage;

    public void Start()
    {
        onRange = false;
        startingPosition = transform.position;
    }

    public void Update()
    {
        timer += Time.deltaTime;
        if(onRange)
        {
            if(timer >= castTime)
            {
                target.LoseHp(damage, gameObject);
                rb.transform.position = target.transform.position;
                timer = 0;
            }
            else
            {
                rb.transform.position = startingPosition;
            }
        }
        else
        {
            if(timer > 1)
            {
                timer = 0;
                rb.transform.position = startingPosition;
            }
        }
    }


    public void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag(team1) || collision.gameObject.CompareTag(team2) || collision.gameObject.CompareTag(team3))
        {
            CMDDealDamage(collision.gameObject.GetComponent<BeingHP>());
        }
    }

    [Command]
    public void CMDDealDamage(BeingHP target)
    {
        RPCDealDamage(target);
    }

    [ClientRpc]
    public void RPCDealDamage(BeingHP target)
    {
        target.LoseHp(damage, gameObject);
    }
}
