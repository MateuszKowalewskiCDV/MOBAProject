using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BellInteraction : MonoBehaviour
{
    public AudioSource bellAs;
    public Animator bellAm;
    public AudioClip bellRingSound;
    public Material teamRed, teamBlue, teamGreen, teamYellow;
    public MeshRenderer mr;
    public string team;
    public string currentOwner;

    public void PlayBell(string currentBellOwner)
    {
        StartCoroutine(PlayBellCoroutine(currentBellOwner));
    }

    public IEnumerator PlayBellCoroutine(string currentBellOwner)
    {
        currentOwner = currentBellOwner;

        if (currentBellOwner == "RedTeam")
        {
            mr.material = teamRed;
        }
        if (currentBellOwner == "GreenTeam")
        {
            mr.material = teamGreen;
        }
        if (currentBellOwner == "BlueTeam")
        {
            mr.material = teamBlue;
        }
        if (currentBellOwner == "YellowTeam")
        {
            mr.material = teamYellow;
        }

        bellAm.Play("BellPlay");
        bellAs.PlayOneShot(bellRingSound, 1F);
        yield return new WaitForSeconds(2);
        bellAs.PlayOneShot(bellRingSound, 1F);
    }
}