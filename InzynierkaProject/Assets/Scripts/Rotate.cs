using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public int rotationValueX, rotationValueY, rotationValueZ;
    public float speed;

    public void Start()
    {

    }

    void Update()
    {
        transform.Rotate(new Vector3(rotationValueX * speed, rotationValueY * speed, rotationValueZ * speed));
    }
}
