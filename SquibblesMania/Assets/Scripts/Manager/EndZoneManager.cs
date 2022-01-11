using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndZoneManager : MonoBehaviour
{
    public List<PlayerStateManager> playerInEndZone;

    private List<Transform> blocksChilds = new List<Transform>();
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

        blocksChilds.RemoveAt(blocksChilds.Count - 1);
    }

    public void PlayersIsOnEndZone(List<Transform> playerActualPathfinding, PlayerStateManager player)
    {
        foreach (Transform block in blocksChilds)
        {
            if (playerActualPathfinding.Contains(block) && !playerInEndZone.Contains(player))
            {
                playerInEndZone.Add(player);
            }
        }
    }

    public void CheckPlayersTeam()
    {
        if (playerInEndZone.Count > 1)
        {
            int playerCountTeamOne = 0;
            int playerCountTeamTwo = 0;

            foreach (PlayerStateManager player in playerInEndZone)
            {
                if (player.playerTeam == Player.PlayerTeam.TeamOne)
                {
                    playerCountTeamOne++;
                }
                else if (player.playerTeam == Player.PlayerTeam.TeamTwo)
                {
                    playerCountTeamTwo++;
                }
            }

            if (playerCountTeamOne >= 2)
            {
                GameManager.Instance.PlayerTeamWin(Player.PlayerTeam.TeamOne);
            }
            else if (playerCountTeamTwo >= 2)
            {
                GameManager.Instance.PlayerTeamWin(Player.PlayerTeam.TeamTwo);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}