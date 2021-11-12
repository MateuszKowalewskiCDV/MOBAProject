using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QAir : MonoBehaviour
{
    [SerializeField]
    private GameObject _shroom;
    private GameObject _shroomClone;

    private GameObject _player;

    void Start()
    {
        _player = GetComponentInParent<Transform>().gameObject;
    }

    public void UseSpell()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            _shroomClone = Instantiate(_shroom);
            _shroomClone.transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y - 0.9f, _player.transform.position.z);
        }
    }
}
