using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAnimation : MonoBehaviour
{
    public GameObject starter;
    public GameObject target;

    void Start()
    {
        transform.position = starter.transform.position;
        StartCoroutine(Dissapear());
    }

    void Update()
    {
        if(Vector3.Distance(starter.transform.position, target.transform.position) > 10)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator Dissapear()
    {
        yield return new WaitForSeconds(0.01f);
        transform.position = target.transform.position;
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}
