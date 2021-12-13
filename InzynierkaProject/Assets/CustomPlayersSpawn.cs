using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomPlayersSpawn : NetworkBehaviour
{
    public GameObject[] spawnPositionsRed, spawnPositionsBlue, spawnPositionsGreen, spawnPositionsYellow;
    public int r = 0, g = 0, y = 0, b = 0;

    public void Start()
    {
        
    }

    public void OnPlayerConnected(NetworkIdentity player)
    {
        Debug.Log("Player " + " connected from " + player + ": " + player.assetId);

        if (player.gameObject.tag == "RedTeam")
        {
            player.gameObject.GetComponent<Rigidbody>().MovePosition(spawnPositionsRed[r].transform.position + new Vector3(0, 1f, 0));
            r++;
        }
        else if (player.gameObject.tag == "GreenTeam")
        {
            player.gameObject.GetComponent<Rigidbody>().MovePosition(spawnPositionsGreen[g].transform.position + new Vector3(0, 1f, 0));
            g++;
        }
        else if (player.gameObject.tag == "YellowTeam")
        {
            player.gameObject.GetComponent<Rigidbody>().MovePosition(spawnPositionsYellow[y].transform.position + new Vector3(0, 1f, 0));
            y++;
        }
        else if (player.gameObject.tag == "BlueTeam")
        {
            player.gameObject.GetComponent<Rigidbody>().MovePosition(spawnPositionsBlue[b].transform.position + new Vector3(0, 1f, 0));
            b++;
        }
        else
        {
            player.gameObject.transform.position = new Vector3(3, 1f, 3);
        }
    }
}
