using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "Spell", order = 1)]
public class Spell : ScriptableObject
{
    public string spellName;
    public string description;
    public int manaCost;
    public float cooldown;
    public Sprite sprite;
    public string skillType;
    public float speed;
    public int damage;
    public float range;
    public bool castableOnAlly;
    public float buffValue;
    public float duration;
    public GameObject spellPrefab;
    public GameObject indicator;
}
