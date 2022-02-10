using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUp : MonoBehaviour
{
    [SerializeField]
    private float speed;

    public void Start()
    {
        StartCoroutine(DestroyMe());
    }
    void Update()
    {
        transform.Translate(transform.up * Time.deltaTime * speed);
    }
    IEnumerator DestroyMe()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
