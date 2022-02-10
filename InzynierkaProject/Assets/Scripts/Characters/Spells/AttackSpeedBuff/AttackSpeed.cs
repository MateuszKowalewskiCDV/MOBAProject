using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class AttackSpeed : MonoBehaviour
{
    public AutoAttack aa;
    public Spell _spell;
    [SerializeField]
    private AudioClip _s;
    [SerializeField]
    private AudioSource _as;
    
    void Start()
    {
        aa = GetComponentInParent<AutoAttack>();
        _as.PlayOneShot(_s);
        StartCoroutine(AttackSpeedApply());
    }

    IEnumerator AttackSpeedApply()
    {
        aa.autoAttackSpeed /= _spell.buffValue;
        yield return new WaitForSeconds(_spell.duration);
        aa.autoAttackSpeed *= _spell.buffValue;
        Destroy(gameObject);
        yield break;
    }
}
