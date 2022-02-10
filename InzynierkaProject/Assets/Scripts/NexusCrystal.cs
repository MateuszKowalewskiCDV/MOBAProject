using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NexusCrystal : MonoBehaviour
{
    Vector3 positionOffset = new Vector3();
    Vector3 temporaryPosition = new Vector3();

    [SerializeField]
    private float degreesPerSecond = 60f;

    [SerializeField]
    private float amplitude = 0.3f;

    [SerializeField]
    private float frequency = 0.7f;

    void Start()
    {
        positionOffset = transform.position;
    }

    void Update()
    {
        transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);

        temporaryPosition = positionOffset;
        temporaryPosition.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.position = temporaryPosition;
    }


}