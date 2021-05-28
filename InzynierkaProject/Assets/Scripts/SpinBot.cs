using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinBot : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(Vector3.left, 30 * Time.deltaTime);
    }
}
