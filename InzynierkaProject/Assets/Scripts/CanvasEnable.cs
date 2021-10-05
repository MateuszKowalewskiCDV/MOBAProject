using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class CanvasEnable : NetworkBehaviour
{
    private Canvas canvas;
    [SerializeField]
    public Image _skill1;
    [SerializeField]
    public TextMeshProUGUI _skill1Ammo;

    public void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    public override void OnStartAuthority()
    {
        canvas.gameObject.SetActive(true);
    }
}