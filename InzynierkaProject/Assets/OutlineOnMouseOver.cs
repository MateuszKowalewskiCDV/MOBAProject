using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineOnMouseOver : MonoBehaviour
{
    public bool _isChoosen;
    private bool _isOnObject;
    private Camera cam;
    public SpriteRenderer targetSprite;
    private Color32 hidden, onMouseOver, targeted;
    public PlayerController pc;

    public void Start()
    {
        pc = GetComponent<PlayerController>();
        hidden = new Color32(255, 0, 0, 0);
        onMouseOver = new Color32(255, 0, 0, 100);
        targeted = new Color32(255, 0, 0, 255);
        cam = Camera.main;
    }

    public void OnMouseOver()
    {
        _isOnObject = true;
        if (_isChoosen == false)
            targetSprite.color = onMouseOver;
        if (Input.GetMouseButtonDown(1))
        {
            targetSprite.color = targeted;
            _isChoosen = true;
        }
    }

    public void Update()
    {
        if (Input.GetMouseButton(1) && _isOnObject == false)
        {
            _isChoosen = false;
            targetSprite.color = hidden;
        }
    }

    public void OnMouseExit()
    {
        _isOnObject = false;
        if (_isChoosen == false)
        {
            targetSprite.color = hidden;
        }
    }
}