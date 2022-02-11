using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiteRunner : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DestroyGameobject());
    }

    IEnumerator DestroyGameobject()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
