using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineOnMouseOver : MonoBehaviour
{
    private Outline _outline;
    private bool _isChoosen;
    private bool _isOnObject;
    private Camera cam;

    public void Start()
    {
        cam = Camera.main;
        _outline = GetComponent<Outline>();
    }
         
    public void OnMouseOver()
    {
        _isOnObject = true;
        _outline.enabled = true;
        if(_isChoosen == false)
            _outline.OutlineWidth = 1;
        if (Input.GetMouseButtonDown(1))
        {
            _outline.OutlineWidth = 2;
            _isChoosen = true;
        }
    }

    public void Update()
    {
        if (Input.GetMouseButton(1) && _isOnObject == false)
        {
            _outline.enabled = false;
            _isChoosen = false;
        }
    }

    public void OnMouseExit()
    {
        _isOnObject = false;
        if(_isChoosen == false)
        {
            _outline.enabled = false;
        }
    }
}
