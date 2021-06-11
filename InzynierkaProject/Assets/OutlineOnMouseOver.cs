using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineOnMouseOver : MonoBehaviour
{
    private Outline _outline;

    public void Start()
    {
        _outline = GetComponent<Outline>();
    }
         
    public void OnMouseOver()
    {
        _outline.enabled = true;
    }

    public void OnMouseExit()
    {
        _outline.enabled = false;
    }
}
