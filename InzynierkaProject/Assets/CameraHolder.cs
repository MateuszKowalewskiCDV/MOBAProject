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

    public bool _freeCamEnabled = false;

    [SerializeField]
    private GameObject _camChild;

    [SerializeField]
    private GameObject _camera;

    public void Start()
    {
        _theScreenWidth = Screen.width;
        _theScreenHeight = Screen.height;
        _camChild.transform.position = Camera.main.transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_freeCamEnabled)
            {
                _freeCamEnabled = false;
            }
            else
            {
                _freeCamEnabled = true;
                _camChild.transform.position = Camera.main.transform.position;
                transform.rotation = Quaternion.Euler(transform.rotation.x, Camera.main.transform.eulerAngles.y, transform.rotation.z);
                _camChild.transform.position = Camera.main.transform.position;
            }
        }
        if (_freeCamEnabled)
        {
            FreeCam();
        }
    }

    void FreeCam()
    {
        Camera.main.transform.position = _camChild.transform.position;

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
            transform.position += transform.forward * Time.deltaTime * _freeCameraSpeed;
        }
        if (Input.mousePosition.y < 0 + _boundary)
        {
            transform.position -= transform.forward * Time.deltaTime * _freeCameraSpeed;
        }
    }
}
