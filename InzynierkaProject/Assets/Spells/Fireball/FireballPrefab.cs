using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FireballPrefab : NetworkBehaviour
{
    public Spell _spell;
    private Vector3 _startingPosition;
    private Vector3 _actualPosition;
    private Vector3 _startScale;
    public GameObject _trail;
    public AudioSource _fireball;
    public AudioClip _clipCast, _clipHit;
    public string myTeam;
    [SerializeField]
    private string team1, team2, team3;
    public GameObject ownerOfAttack;
    public SphereCollider sc;
    public Vector3 velocity;
    public Rigidbody rb;
    public BeingHP _bh;

    public void Awake()
    {
        transform.position += velocity/80;

        if (myTeam == "RedTeam")
        {
            team1 = "BlueTeam";
            team2 = "GreenTeam";
            team3 = "YellowTeam";
        }
        else if (myTeam == "BlueTeam")
        {
            team1 = "RedTeam";
            team2 = "GreenTeam";
            team3 = "YellowTeam";
        }
        else if (myTeam == "YellowTeam")
        {
            team1 = "BlueTeam";
            team2 = "GreenTeam";
            team3 = "RedTeam";
        }
        else if (myTeam == "GreenTeam")
        {
            team1 = "BlueTeam";
            team2 = "RedTeam";
            team3 = "YellowTeam";
        }

        _startScale = transform.localScale;
        _startingPosition = transform.position;
        _fireball.PlayOneShot(_clipCast);
    }

    public void Update()
    {
        _actualPosition = transform.position;

        if(Vector3.Distance(_startingPosition, _actualPosition) >= _spell.range*5)
        {
            StartCoroutine(EndLifeNotHit());
        }
    }
    
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(myTeam))
        {
            if(isLocalPlayer)
                CmdTeammateHit(collision.gameObject);
            if(isServer)
                RpcTeammateHit(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (isLocalPlayer)
                CmdEnemyHit(collision.gameObject);
            if (isServer)
                RpcEnemyHit(collision.gameObject);
        }
        if (collision.gameObject.CompareTag(team1) || collision.gameObject.CompareTag(team2) || collision.gameObject.CompareTag(team3))
        {
            if (isLocalPlayer)
                CmdEnemyPlayerHit(collision.gameObject);
            if (isServer)
                RpcEnemyPlayerHit(collision.gameObject);
        }
        if(!collision.gameObject.CompareTag(myTeam))
        {
            StartCoroutine(EndLifeHit());
        }
    }

    [Command]
    public void CmdEnemyHit(GameObject collision)
    {
        if (isServer) return;
            RpcEnemyHit(collision);
    }

    [Command]
    public void CmdTeammateHit(GameObject collision)
    {
        if (isServer) return;
            RpcTeammateHit(collision);
    }

    [Command]
    public void CmdEnemyPlayerHit(GameObject collision)
    {
        if (isServer) return;
            RpcEnemyPlayerHit(collision);
    }

    [ClientRpc]
    public void RpcEnemyHit(GameObject collision)
    {
        if(isServer)
        {
            collision.gameObject.TryGetComponent(out BeingHP being);
            if (being)
            {
                being.LoseHp(_spell.damage + _bh.attackBoost, ownerOfAttack);
            }
            StartCoroutine(EndLifeHit());
        }
    }

    [ClientRpc]
    public void RpcTeammateHit(GameObject collision)
    {
        rb.velocity += velocity;
        sc.enabled = false;
        rb.velocity += velocity;
        sc.enabled = true;
    }

    [ClientRpc]
    public void RpcEnemyPlayerHit(GameObject collision)
    {
        if (isServer)
        {
            collision.gameObject.TryGetComponent(out BeingHP being);
            if (being)
            {
                being.LoseHp(_spell.damage + _bh.attackBoost, ownerOfAttack);
            }
            StartCoroutine(EndLifeHit());
        }
    }

    IEnumerator EndLifeHit()
    {
        _fireball.PlayOneShot(_clipHit);
        gameObject.GetComponent<Collider>().enabled = false;
        transform.localScale = _startScale * 1.1f;
        yield return new WaitForSeconds(0.02f);
        transform.localScale = _startScale * 1.3f;
        yield return new WaitForSeconds(0.03f);
        transform.localScale = _startScale * 1.5f;
        yield return new WaitForSeconds(0.05f);
        transform.localScale = _startScale * 2f;
        yield return new WaitForSeconds(0.03f);
        transform.localScale = _startScale * 0.6f;
        yield return new WaitForSeconds(0.02f);
        Destroy(gameObject);
    }

    IEnumerator EndLifeNotHit()
    {
        gameObject.GetComponent<Collider>().enabled = false;
        Destroy(_trail);
        transform.localScale = transform.localScale * 0.9f;
        yield return new WaitForSeconds(0.04f);
        transform.localScale = transform.localScale * 0.9f;
        yield return new WaitForSeconds(0.02f);
        Destroy(gameObject);
    }
}
