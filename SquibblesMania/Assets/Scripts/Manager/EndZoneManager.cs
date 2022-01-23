using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndZoneManager : MonoBehaviour
{
    public List<Transform> playerInEndZone;

    public List<Transform> blocksChilds = new List<Transform>();
    private static EndZoneManager _endZoneManager;

    public static EndZoneManager Instance => _endZoneManager;


    private void Awake()
    {
        _endZoneManager = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform tr in transform.parent)
        {
            blocksChilds.Add(tr);
        }

        //Remove this object from the list
        blocksChilds.RemoveAt(blocksChilds.Count - 1);
        foreach (Transform block in blocksChilds)
        {
            block.GetComponent<Renderer>().material.color = Color.green;
        }
    }

    public void PlayersIsOnEndZone()
    {
        GroupBlockDetection parent = blocksChilds[0].parent.GetComponent<GroupBlockDetection>();
        playerInEndZone = parent.playersOnGroupBlock;
    }

    public void CheckPlayersTeam()
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
                        if (inventoryTeamOne.isAllObjetAcquired)
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
                        if (inventoryTeamTwo.isAllObjetAcquired)
                        {
                            GameManager.Instance.PlayerTeamWin(Player.PlayerTeam.TeamTwo);
                        }
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}