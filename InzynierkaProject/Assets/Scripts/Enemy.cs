using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    public int hp = 100;
    public string name = "Enemy";
    public string description = "I'm enemy";
    public Sprite icon;
    public int attackRange;

    public Enemy(int hp, string name, string description, Sprite icon, int attackRange)
    {
        this.hp = hp;
        this.name = name;
        this.description = description;
        this.icon = icon;
        this.attackRange = attackRange;
    }
}
