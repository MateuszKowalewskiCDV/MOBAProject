using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class SpeedUpBuff : NetworkBehaviour
{
    private NavMeshAgent _player;
    public Spell _spell;
    private float _playerSpeed;
    [SerializeField]
    private AudioClip _s;
    [SerializeField]
    private AudioSource _as;

    void Start()
    {
        _player = GetComponentInParent<NavMeshAgent>();
        _as.PlayOneShot(_s);
        StartCoroutine(SpeedUpBuffApply());
        _playerSpeed = 5;
    }

    IEnumerator SpeedUpBuffApply()
    {
        _player.speed *= _spell.buffValue;
         yield return new WaitForSeconds(_spell.duration);
        _player.speed = _playerSpeed;
        Destroy(gameObject);
        yield break;
    }
}
