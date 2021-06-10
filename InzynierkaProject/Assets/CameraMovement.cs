using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraMovement : MonoBehaviour
{

    [SerializeField]
    private float _cameraZoomSpeed = 500.0f;

    public GameObject player;

    public Vector3 cameraOffset;

    [Range(0.01f, 1.0f)]
    public float smoothFactor = 0.5f;

    public bool lookAtPlayer = false;

    public bool rotateAroundPlayer = true;

    public float rotationsSpeed = 5.0f;

    [SerializeField]
    private int _boundary;
    [SerializeField]
    private int _speed;
    [SerializeField]
    private float _freeCameraSpeed;

    void LateUpdate()
    {
        if(player == null)
        {
            return;
        }
        else if(!Input.GetKey(KeyCode.Space))
        {
            if (rotateAroundPlayer && Input.GetKey(KeyCode.Mouse2))
            {
                Quaternion camTurnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotationsSpeed, Vector3.up);
                cameraOffset = camTurnAngle * cameraOffset;
            }

            Vector3 newPos = player.transform.position + cameraOffset;

            transform.position = Vector3.Slerp(transform.position, newPos, smoothFactor);

            if (lookAtPlayer || rotateAroundPlayer)
            {
                transform.LookAt(player.transform);
            }

            if (Input.mouseScrollDelta.y > 0f)
            {
                Camera.main.fieldOfView += _cameraZoomSpeed * Time.deltaTime;
            }
            if (Input.mouseScrollDelta.y < 0f)
            {
                Camera.main.fieldOfView += -_cameraZoomSpeed * Time.deltaTime;
            }
        }
    }
}
