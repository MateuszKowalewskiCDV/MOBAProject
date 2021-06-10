using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abilities : MonoBehaviour
{
    private GameObject _missile;

    void Update()
    {
        if(Input.GetKey(KeyCode.Q))
        {
            ShootMissile();
        }
    }

    void ShootMissile()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    }

}
