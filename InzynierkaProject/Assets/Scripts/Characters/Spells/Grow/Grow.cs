using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grow : MonoBehaviour
{
    public Spell _spell;
    private Transform _player;
    [SerializeField]
    private AudioClip _s;
    [SerializeField]
    private AudioSource _as;
    private BeingHP _playerHP;
    private int maxHpHolder;

    void Start()
    {
        _playerHP = GetComponentInParent<BeingHP>();
        _player = _playerHP.gameObject.GetComponent<Transform>();
        _as.PlayOneShot(_s);
        StartCoroutine(GrowBuffApply());
    }

    IEnumerator GrowBuffApply()
    {
        maxHpHolder = _playerHP.maxHp;
        _player.localScale *= _spell.buffValue;
        _playerHP.maxHp += maxHpHolder;
        _playerHP.actualHp += maxHpHolder;
        yield return new WaitForSeconds(_spell.duration);
        _player.localScale /= _spell.buffValue;
        _playerHP.maxHp -= maxHpHolder;
        if(_playerHP.actualHp > _playerHP.maxHp)
        {
            _playerHP.actualHp = _playerHP.maxHp;
        }
        Destroy(gameObject);
        yield break;
    }
}
