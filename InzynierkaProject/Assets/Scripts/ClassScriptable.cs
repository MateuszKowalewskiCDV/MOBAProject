using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Class", menuName = "Class", order = 1)]
public class ClassScriptable : ScriptableObject
{
    public string className;
    public Sprite sprite;
    public int hpRaise;
    public float attackSpeedRaise;
    public int damageRaise;
    public float range;

    public Spell QSpell;
    public Spell WSpell;
    public Spell ESpell;
    public Spell RSpell;
}
