using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class SpawnMob : NetworkBehaviour
{
    public MobScriptable _mob;
    private GameObject _mobInstance;
    private GameObject _mobInstanceSpawned;
    private bool notSpawning;
    private Vector3 _startPostion;

    void Start()
    {
        _startPostion = transform.position;
        _mobInstance = _mob.enemyPrefab;
        _mobInstanceSpawned = Instantiate(_mobInstance, transform, false);
        NetworkServer.Spawn(_mobInstanceSpawned);
        _mobInstanceSpawned.transform.position = transform.position;
    }

    void Update()
    {
        if(_mobInstanceSpawned == null && notSpawning == true)
        {
            StartCoroutine(SpawnInstanceMob());
        }
        else
        {
            return;
        }
    }

    IEnumerator SpawnInstanceMob()
    {
        notSpawning = false;
        yield return new WaitForSeconds(_mob.spawnTime);
        _mobInstanceSpawned = Instantiate(_mobInstance, transform.position = new Vector3(transform.position.x, transform.position.y + 1.1f, transform.position.z), Quaternion.Euler(0,0,0));
        _mobInstanceSpawned.GetComponent<EnemyAttack>()._mob = _mob;
        _mobInstanceSpawned.GetComponent<BeingHP>()._mob = _mob;
        notSpawning = true;
    }
}
