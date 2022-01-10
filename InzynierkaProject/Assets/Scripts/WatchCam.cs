using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchCam : MonoBehaviour
{
    public Camera cam;

    public void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}
