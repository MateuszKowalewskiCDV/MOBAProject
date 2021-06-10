using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHolder : MonoBehaviour
{
    private float _theScreenWidth;
    private float _theScreenHeight;
    [SerializeField]
    private int _boundary;
    [SerializeField]
    private float _freeCameraSpeed;

    [SerializeField]
    private CameraMovement _partner;

    public void Start()
    {
        _theScreenWidth = Screen.width;
        _theScreenHeight = Screen.height;
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            if (Input.mousePosition.x > _theScreenWidth - _boundary)
            {
                transform.position += transform.right * Time.deltaTime * _freeCameraSpeed;
            }
            if (Input.mousePosition.x < 0 + _boundary)
            {
                transform.position -= transform.right * Time.deltaTime * _freeCameraSpeed;
            }
            if (Input.mousePosition.y > _theScreenHeight - _boundary)
            {
                transform.localPosition -= transform.forward * Time.deltaTime * _freeCameraSpeed;
            }
            if (Input.mousePosition.y < 0 + _boundary)
            {
                transform.localPosition += transform.forward * Time.deltaTime * _freeCameraSpeed;
            }
        }
        if(!Input.GetKey(KeyCode.Space))
        {
            transform.position = _partner.player.transform.position;
        }
    }
}
