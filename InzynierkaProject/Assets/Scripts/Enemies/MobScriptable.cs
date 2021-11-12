using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mob", menuName = "Mob", order = 1)]
public class MobScriptable : ScriptableObject
{
    public string enemyName;
    public string description;
    public int hp;
    public int mana;
    public int spawnTime;
    public Sprite sprite;
    public float speed;
    public int damage;
    public int attackSpeed;
    public float range;
    public float followRange;
    public GameObject enemyPrefab;
}
