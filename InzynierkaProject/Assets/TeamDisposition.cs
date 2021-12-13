using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamDisposition : MonoBehaviour
{
    public string teamChoosen;
    public MeshRenderer mr;
    public Material red, green, blue, yellow;

    void Start()
    {
        if (teamChoosen == "blue")
        {
            mr.material = blue;
        }
        if (teamChoosen == "red")
        {
            mr.material = red;
        }
        if (teamChoosen == "green")
        {
            mr.material = green;
        }
        if (teamChoosen == "yellow")
        {
            mr.material = yellow;
        }
    }
}
