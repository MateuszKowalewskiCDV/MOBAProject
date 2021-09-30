using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissapearAfterTime : MonoBehaviour
{
    private Material _material;
    private MeshRenderer _mr;
    private Color color;
    [SerializeField]
    private float _fadeSpeed;

    public void Start()
    {
        color = this.GetComponent<MeshRenderer>().material.color;
        _mr = gameObject.GetComponent<MeshRenderer>();
    }
    void Update()
    {
        color.a -= Time.deltaTime * _fadeSpeed;
        if(color.a < 0f)
        {
            Destroy(gameObject);
        }
        _mr.material.color = color;
    }
}
