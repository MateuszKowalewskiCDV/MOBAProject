using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AutoAttack : NetworkBehaviour
{
    public float autoattackSpeed;
    public int autoAttackDamage;
    public float autoAttackRange;
    public OutlineOnMouseOver oomo;
    public GameObject enemy;
    public BeingHP _bh, _playerBh;
    public bool autoReady;
    public bool isProjectile;
    public GameObject projectile;
    private float timer;
    private bool targeted;

    public void Start()
    {
        _playerBh = GetComponent<BeingHP>();
        timer = 0;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if(hit.transform.gameObject.CompareTag("Enemy"))
                {
                    enemy = hit.transform.gameObject;
                    oomo = hit.transform.gameObject.GetComponentInChildren<OutlineOnMouseOver>();
                    oomo.pc = GetComponent<PlayerController>();
                    _bh = oomo.GetComponent<BeingHP>();
                    if (Vector3.Distance(transform.position, enemy.transform.position) <= autoAttackRange && enemy != null)
                    {
                        oomo.pc.agent.SetDestination(transform.position);
                    }
                    else
                    {
                        targeted = true;
                        oomo.pc.OnAttack(enemy.GetComponent<Transform>().gameObject);
                    }
                }
            }
        }

        if (targeted == true)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) <= autoAttackRange && enemy != null)
            {
                oomo.pc.agent.SetDestination(transform.position);
                targeted = false;
            }
            
        }

        if (autoReady == false)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                autoReady = true;
            }
        }
        else if(autoReady == true && oomo != null)
        {
            if (oomo._isChoosen == true && _bh.isAlive && Vector3.Distance(oomo.gameObject.transform.position, gameObject.transform.position) <= autoAttackRange)
            {
                CmdDealDamage();
                timer = autoattackSpeed;
                autoReady = false;
            }
        }
    }

    [Command]
    void CmdDealDamage()
    {
        if(isServer)
        {
            RpcDealDamage();
        }
    }

    [ClientRpc]
    void RpcDealDamage()
    {
        if(isServer)
        {
            _bh.LoseHp(autoAttackDamage + _playerBh.attackBoost, gameObject);
            if (isProjectile == true)
            {
                var prefabProjectile = (GameObject)Instantiate(projectile, gameObject.transform, false);
                prefabProjectile.GetComponent<ProjectileAnimation>().starter = gameObject;
                prefabProjectile.GetComponent<ProjectileAnimation>().target = enemy;
                NetworkServer.Spawn(prefabProjectile);
            }
        }
    }
}
