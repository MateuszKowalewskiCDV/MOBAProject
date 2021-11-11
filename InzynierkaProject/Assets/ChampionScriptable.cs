using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Characters", order = 1)]

public class ChampionScriptable : ScriptableObject
{
    public string className;
    public Sprite spriteClass, spriteElement;
    private enum MyClasses { SpellScriptableSkillshot, SpellScriptableBuff, SpellScriptableSkillshotDebuff, SpellScriptable }
    [SerializeField] private MyClasses spellQ, spellW, spellE, spellR;
}
