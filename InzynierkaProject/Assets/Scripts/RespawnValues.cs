using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnValues : MonoBehaviour
{
    public int respawnTime;
    public float timer;

    public void Update()
    {
        timer += Time.deltaTime;
        respawnTime = respawnTime + (int)Mathf.Round(timer)/60;
    }
}
