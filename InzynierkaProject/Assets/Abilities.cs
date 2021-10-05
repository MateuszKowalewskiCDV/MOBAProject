using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

public class Abilities : MonoBehaviour
{
    [SerializeField]
    private GameObject _fireMissile;
    private GameObject _fireMissileClone;
    private int speed = 1;
    [SerializeField]
    private float step;

    [SerializeField]
    private GameObject _player;

    private bool _skill1Ready = true;

    [SerializeField]
    private float _skill1Cooldown;

    [SerializeField]
    private int ammo = 0, ammoAmmount = 4;

    public Image imageSkill1;
    public TextMeshProUGUI skill1Ammount;

    private bool _ammoLoading = false;

    private CanvasEnable _partnerCanvasEnable;

    private Image _imageSKill1;

    public void Start()
    {
        _partnerCanvasEnable = FindObjectOfType<CanvasEnable>();

        ammo = 0;

        _imageSKill1 = _partnerCanvasEnable._skill1;
        skill1Ammount = _partnerCanvasEnable._skill1Ammo;

        if (GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                _player = GetComponent<Transform>().gameObject;
            }

        skill1Ammount.text = ammo.ToString();
    }

    void Update()
    {
        if (ammo < ammoAmmount)
        {
            if(_ammoLoading == false)
            {
                _ammoLoading = true;
                StartCoroutine(CooldownSkill1());
            }
        }
        if(Input.GetKeyDown(KeyCode.Q) && ammo > 0)
        {
            ammo = ammo - 1;
            skill1Ammount.text = ammo.ToString();
            ShootMushroom();
        }
    }

    void ShootMushroom()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        step = speed * Time.deltaTime;

        if (Physics.Raycast(ray, out hit))
        {
            _fireMissileClone = Instantiate(_fireMissile);
            _fireMissileClone.transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y - 0.9f, _player.transform.position.z);
        }
    }

    IEnumerator CooldownSkill1()
    {
        yield return new WaitForSeconds(_skill1Cooldown);
        ammo++;
        skill1Ammount.text = ammo.ToString();
        _ammoLoading = false;
    }
}
