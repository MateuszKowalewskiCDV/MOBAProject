using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomCollider : MonoBehaviour
{
    public GameObject _explosionHolePrefab;
    private bool _readyToExplode;
    public float skillPrepareCooldown;
    private bool justExplode;

    public void Start()
    {
        justExplode = false;
        _readyToExplode = false;
        StartCoroutine(PrepareToExplode());
    }

    public void OnTriggerStay(Collider collision)
    {
        if ((collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy") && _readyToExplode == true)
        {
            transform.localScale *= 1.15f;
            if (transform.localScale.x >= 9f)
            {
                Instantiate(_explosionHolePrefab, new Vector3(transform.position.x, transform.position.y - 0.15f, transform.position.z), Quaternion.Euler(-90, 0, 0));
                Destroy(gameObject);
            }
        }
    }

    IEnumerator PrepareToExplode()
    {
        yield return new WaitForSeconds(skillPrepareCooldown);
        _readyToExplode = true;
    }
}
