using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineChildBlocker : MonoBehaviour
{
    public bool OutlineOn = false;
    private bool set = false;
    private Outline[] OutlineScripts;

    void Start()
    {
        OutlineScripts = GetComponentsInChildren<Outline>();
    }

    void Update()
    {
        if (OutlineOn != set)
        {
            set = OutlineOn;
            SetOutline();
        }
    }

    void SetOutline()
    {
        foreach (Outline outline in OutlineScripts)
        {
            outline.enabled = OutlineOn;
        }
    }
}
