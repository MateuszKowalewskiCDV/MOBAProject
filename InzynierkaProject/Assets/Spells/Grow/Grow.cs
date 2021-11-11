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

    void Start()
    {
        _playerHP = GetComponentInParent<BeingHP>();
        _player = _playerHP.gameObject.GetComponent<Transform>();
        _as.PlayOneShot(_s);
        StartCoroutine(GrowBuffApply());
    }

    IEnumerator GrowBuffApply()
    {
        _player.localScale *= _spell.buffValue;
        _playerHP.maxHp *= 2;
        _playerHP.actualHp += _playerHP.maxHp / 2;
        yield return new WaitForSeconds(_spell.duration);
        _player.localScale /= _spell.buffValue;
        _playerHP.maxHp /= 2;
        if(_playerHP.actualHp > _playerHP.maxHp)
        {
            _playerHP.actualHp = _playerHP.maxHp;
        }
        Destroy(gameObject);
        yield break;
    }
}
