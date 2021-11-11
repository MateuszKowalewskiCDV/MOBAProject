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

    public void Start()
    {
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
        collision.gameObject.TryGetComponent<BeingHP>(out BeingHP being);
        if(being)
        {
            being.LoseHp(_spell.damage);
        }
        StartCoroutine(EndLifeHit());
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
