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
        teamChoosen = gameObject.tag;

        if (teamChoosen == "BlueTeam")
        {
            mr.material = blue;
        }
        if (teamChoosen == "RedTeam")
        {
            mr.material = red;
        }
        if (teamChoosen == "GreenTeam")
        {
            mr.material = green;
        }
        if (teamChoosen == "YellowTeam")
        {
            mr.material = yellow;
        }
    }
}
