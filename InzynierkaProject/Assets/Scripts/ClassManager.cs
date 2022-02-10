using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassManager : MonoBehaviour
{
    public ClassScriptable classOfPlayer;
    public int classOfPlayerChoose;
    [SerializeField]
    private ClassScriptable[] availableClasses;
    public HoverTip q, w, e, r;
    public SkillUsage qUsage, wUsage, eUsage, rUsage;

    void Start()
    {
        classOfPlayer = availableClasses[0];
        SetClass(0);
    }

    public void SetClass(int classOfPlayerChoose)
    {
        q.spell = availableClasses[classOfPlayerChoose].QSpell;
        w.spell = availableClasses[classOfPlayerChoose].WSpell;
        e.spell = availableClasses[classOfPlayerChoose].ESpell;
        r.spell = availableClasses[classOfPlayerChoose].RSpell;
        qUsage._skill = availableClasses[classOfPlayerChoose].QSpell;
        wUsage._skill = availableClasses[classOfPlayerChoose].WSpell;
        eUsage._skill = availableClasses[classOfPlayerChoose].ESpell;
        rUsage._skill = availableClasses[classOfPlayerChoose].RSpell;
    }

}
