using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class triggerBushes : NetworkBehaviour
{
    public int[] playerBushNumber;
    public int instanceNumber;
    public Material[] materialOfObject;
    public Color[] col;

    public void Start()
    {
        instanceNumber = 0;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("RedTeam") || other.CompareTag("BlueTeam") || other.CompareTag("GreenTeam") || other.CompareTag("YellowTeam"))
        {
            if (instanceNumber >= 16)
            {
                instanceNumber = 0;
            }

            instanceNumber++;

            other.GetComponent<BeingHP>().myBushNumber = instanceNumber;

            materialOfObject[instanceNumber] = other.GetComponent<MeshRenderer>().material;
            col[instanceNumber] = materialOfObject[instanceNumber].color;

            if (other.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                col[instanceNumber].a = 0.5f;
                materialOfObject[instanceNumber].color = col[instanceNumber];
            }
            else
            {
                RPCMakeMeInvisible(0f, instanceNumber, other.gameObject);
            }
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("RedTeam") || other.CompareTag("BlueTeam") || other.CompareTag("GreenTeam") || other.CompareTag("YellowTeam"))
        {
            RPCMakeMeInvisibleExit(1f, other.GetComponent<BeingHP>().myBushNumber, other.gameObject);
        }
    }

    [ClientRpc]
    void RPCMakeMeInvisible(float alphaValue, int myNumber, GameObject other)
    {
        other.GetComponent<BeingHP>()._manaBar.enabled = false;
        other.GetComponent<BeingHP>()._hpBar.enabled = false;
        int number = other.GetComponent<BeingHP>().myBushNumber;
        col[number].a = alphaValue;
        materialOfObject[number].color = col[number];
    }

    [ClientRpc]
    void RPCMakeMeInvisibleExit(float alphaValue, int myNumber, GameObject other)
    {
        other.GetComponent<BeingHP>()._manaBar.enabled = true;
        other.GetComponent<BeingHP>()._hpBar.enabled = true;
        int number = other.GetComponent<BeingHP>().myBushNumber;
        col[number].a = alphaValue;
        materialOfObject[number].color = col[number];
        number = 0;
    }
}
