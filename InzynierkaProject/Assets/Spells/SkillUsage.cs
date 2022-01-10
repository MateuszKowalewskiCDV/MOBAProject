using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SkillUsage : NetworkBehaviour
{
    public GameObject _prefabSkill, _indicator;

    [SerializeField]
    private KeyCode _abilityKey;
    [SerializeField]
    private Spell _skill;

    private Vector3 velocity;

    private bool _indicatorExiste;

    [SyncVar]
    public bool cooldownReady;

    public void Start()
    {
        cooldownReady = true;
        _prefabSkill = _skill.spellPrefab;
        _indicator = _skill.indicator;
        _indicatorExiste = false;
        _indicator = Instantiate(_indicator, gameObject.transform);
        _indicator.transform.localScale = new Vector3(_indicator.transform.localScale.x * _skill.range, _indicator.transform.localScale.y, _indicator.transform.localScale.z);
    }
    
    void Update()
    {
        if (!isLocalPlayer)
            return;
        if (_skill.skillType == "skillshot")
        {
            if (Input.GetKey(_abilityKey) && cooldownReady == true)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (_indicatorExiste == false)
                    {
                        _indicator.SetActive(true);
                        _indicatorExiste = true;
                    }

                    _indicator.transform.LookAt(hit.point);
                    _indicator.transform.rotation = Quaternion.Euler(90f, _indicator.transform.rotation.eulerAngles.y - 90f, _indicator.transform.rotation.eulerAngles.z);
                }
            }
            if (Input.GetKeyUp(_abilityKey) && cooldownReady == true)
            {
                UseSkillshot();
            }
        }
        if (_skill.skillType == "buff")
        {
            if (Input.GetKeyDown(_abilityKey) && cooldownReady == true)
            {
                if (isServer)
                    RPCUseBuff();
                else
                    CMDUseBuff();
            }
        }
    }


    void UseSkillshot()
    {
        StartCoroutine(CooldownApply());
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 bulletDirection = hit.point - _indicator.transform.position;
            bulletDirection.Normalize();
            velocity = bulletDirection * _skill.speed;
            Vector3 velocityOfSkillshot = velocity;

            Shoot(velocity);
        }
        _indicator.SetActive(false);
        _indicatorExiste = false;
    }

    public void Shoot(Vector3 velocity)
    {
        if(isLocalPlayer)
            CMDShootPrefab(velocity, gameObject);
    }

    [Command]
    void CMDUseBuff()
    {
        RPCUseBuff();
    }

    [ClientRpc]
    void RPCUseBuff()
    {
        StartCoroutine(CooldownApply());
        var _prefabSkillInstance = (GameObject)Instantiate(_prefabSkill, gameObject.transform, false);
        if (_skill.spellName == "SpeedUp")
            _prefabSkillInstance.GetComponent<SpeedUpBuff>()._spell = _skill;
        if (_skill.spellName == "Grow")
            _prefabSkillInstance.GetComponent<Grow>()._spell = _skill;
    }

    [Command]
    void CMDShootPrefab(Vector3 velocityOfSkillshot, GameObject owner)
    {
        var _prefabSkillInstance = (GameObject)Instantiate(_prefabSkill, transform.position, Quaternion.identity);
        _prefabSkillInstance.GetComponent<Rigidbody>().velocity = velocityOfSkillshot;
        Physics.IgnoreCollision(GetComponent<Collider>(), _prefabSkillInstance.GetComponent<Collider>());

        if (_skill.spellName == "Fireball")
        {
            _prefabSkillInstance.GetComponent<FireballPrefab>().velocity = velocityOfSkillshot;
            _prefabSkillInstance.GetComponent<FireballPrefab>()._spell = _skill;
            _prefabSkillInstance.GetComponent<FireballPrefab>().myTeam = gameObject.tag;
            _prefabSkillInstance.GetComponent<FireballPrefab>().ownerOfAttack = owner;
            NetworkServer.Spawn(_prefabSkillInstance);
        }
    }

    IEnumerator CooldownApply()
    {
        cooldownReady = false;
        yield return new WaitForSeconds(_skill.cooldown);
        cooldownReady = true;
    }
}
