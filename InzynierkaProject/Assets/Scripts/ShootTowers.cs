using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ShootTowers : NetworkBehaviour
{
    [SerializeField]
    private string team1, team2, team3, teamWithBell;

    [SerializeField]
    private GameObject projectile;

    public BellInteraction bellState;

    private TowerShotPrefab projectileTarget;

    public void Start()
    {
        projectileTarget = projectile.GetComponent<TowerShotPrefab>();
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag(team1) || other.gameObject.CompareTag(team2) || other.gameObject.CompareTag(team3))
        {
            if(bellState.currentOwner != null)
            {
                teamWithBell = bellState.currentOwner;
            }


            if (teamWithBell == null || !other.gameObject.CompareTag(teamWithBell))
            {
                projectile.GetComponent<TowerShotPrefab>().onRange = true;
                if (projectileTarget.target == null)
                    projectile.GetComponent<TowerShotPrefab>().target = other.gameObject.GetComponent<BeingHP>();
            }
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(team1) || other.gameObject.CompareTag(team2) || other.gameObject.CompareTag(team3))
        {
            projectile.GetComponent<TowerShotPrefab>().onRange = false;
        }
    }
}
