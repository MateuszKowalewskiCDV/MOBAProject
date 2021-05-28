using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CanvasEnable : NetworkBehaviour
{
    [SerializeField] private GameObject canvas;

    public override void OnStartAuthority()
    {
        canvas.SetActive(true);
    }
}