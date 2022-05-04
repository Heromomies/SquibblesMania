using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EndZoneManager : MonoBehaviour
{
    public List<Transform> playerInEndZone;

    public List<Transform> blocksChild = new List<Transform>();
    private static EndZoneManager _endZoneManager;

    public static EndZoneManager Instance => _endZoneManager;

    [SerializeField] private GameObject planeEndZone;
    private void Awake()
    {
        _endZoneManager = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform tr in transform.parent)
        {
            blocksChild.Add(tr);
        }

        //Remove this object from the list
        blocksChild.RemoveAt(blocksChild.Count - 1);
        SpawnEndZone();
    }

   
    public void PlayersIsOnEndZone()
    {
        GroupBlockDetection parent = blocksChild[0].parent.GetComponent<GroupBlockDetection>();
        playerInEndZone = parent.playersOnGroupBlock;
    }

    private void SpawnEndZone()
    {
        for (int i = 0; i < blocksChild.Count; i++)
        {
            Vector3 planeEndZonePos = blocksChild[i].transform.position + new Vector3(0f, 1.03f, 0f);
            Instantiate(planeEndZone, planeEndZonePos, Quaternion.identity, blocksChild[i]);
        }
    }

    public void CheckPlayersTeam() // Check the players team
    {
        if (playerInEndZone.Count > 1)
        {
            int playerCountTeamOne = 0;
            int playerCountTeamTwo = 0;
            
            foreach (Transform player in playerInEndZone)
            {
                PlayerStateManager playerStateManager = player.GetComponent<PlayerStateManager>();
                if (playerStateManager.playerTeam == Player.PlayerTeam.TeamOne)
                {
                    playerCountTeamOne++;
                    if (playerCountTeamOne >= 2)
                    {
                        Inventory inventoryTeamOne = TeamInventoryManager.Instance.inventory[0];
                        if (inventoryTeamOne.boatObject.Count == 3)
                        {
                            GameManager.Instance.PlayerTeamWin(Player.PlayerTeam.TeamOne);
                        }
                    }
                }
                else if (playerStateManager.playerTeam == Player.PlayerTeam.TeamTwo)
                {
                    playerCountTeamTwo++;

                    if (playerCountTeamTwo >= 2)
                    {
                        Inventory inventoryTeamTwo = TeamInventoryManager.Instance.inventory[1];
                        if (inventoryTeamTwo.boatObject.Count == 3)
                        {
                            GameManager.Instance.PlayerTeamWin(Player.PlayerTeam.TeamTwo);
                        }
                    }
                }
            }
        }
    }
    
}