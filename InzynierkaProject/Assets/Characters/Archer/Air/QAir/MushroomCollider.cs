using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomCollider : MonoBehaviour
{
    public GameObject _explosionHolePrefab;
    private bool _readyToExplode;
    public float skillPrepareCooldown;
    [SerializeField]
    private int mineDamage;

    [SerializeField]
    private int radius;

    public void Start()
    {
        _readyToExplode = false;
        StartCoroutine(PrepareToExplode());
    }

    public void OnTriggerStay(Collider collision)
    {
        if ((collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy") && _readyToExplode == true)
        {
            transform.localScale *= 1.1f;
            if (transform.localScale.x >= 9f)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
                Instantiate(_explosionHolePrefab, new Vector3(transform.position.x, transform.position.y - 0.15f, transform.position.z), Quaternion.Euler(-90, 0, 0));
                foreach (Collider nearbyObjects in colliders)
                {
                    if (nearbyObjects.gameObject.TryGetComponent(out BeingHP beingHP) == true)
                    {
                        beingHP.LoseHp(mineDamage);
                    }
                }
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
