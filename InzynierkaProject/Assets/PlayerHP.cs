using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    public Rigidbody player;
    public int hp;

    void Start()
    {
        hp = 100;
    }

    public int LoseHp(int loss)
    {
        hp = hp - loss;

        return hp;
    }
}
