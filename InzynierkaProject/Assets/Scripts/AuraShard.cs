using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AuraShard : MonoBehaviour
{
    private string team1, team2, team3, myTeam;
    public GameObject ownerOfAttack;
    public Spell _spell;
    public bool isMain;
    private float skillFrequency, timer;

    public void Start()
    {
        if (isMain)
            StartCoroutine(SetOff());

        myTeam = GetComponentInParent<BeingHP>().gameObject.tag;

        if (myTeam == "RedTeam")
        {
            team1 = "BlueTeam";
            team2 = "GreenTeam";
            team3 = "YellowTeam";
        }
        else if (myTeam == "BlueTeam")
        {
            team1 = "RedTeam";
            team2 = "GreenTeam";
            team3 = "YellowTeam";
        }
        else if (myTeam == "YellowTeam")
        {
            team1 = "BlueTeam";
            team2 = "GreenTeam";
            team3 = "RedTeam";
        }
        else if (myTeam == "GreenTeam")
        {
            team1 = "BlueTeam";
            team2 = "RedTeam";
            team3 = "YellowTeam";
        }

        timer = 0;
        skillFrequency = _spell.speed;

        ownerOfAttack.GetComponent<BeingHP>().AuraBuff(_spell.buffValue, gameObject);
    }

    public void OnTriggerStay(Collider collision)
    {
        if(collision.gameObject.CompareTag(myTeam))
        {
            if (collision.gameObject.TryGetComponent(out BeingHP target))
            {
                timer += Time.deltaTime;
                if(timer >= skillFrequency)
                {
                    target.AuraBuff(_spell.buffValue, gameObject);
                }
            }
        }
    }

    IEnumerator SetOff()
    {
        yield return new WaitForSeconds(_spell.duration);
        gameObject.SetActive(false);
    }
}
