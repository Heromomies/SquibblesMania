using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndZoneManager : MonoBehaviour
{
    public List<Transform> playerInEndZone;
    public Color baseColor, colorTo;

    public List<Transform> blocksChild = new List<Transform>();
    private static EndZoneManager _endZoneManager;

    public static EndZoneManager Instance => _endZoneManager;

    [SerializeField] private float pulsingLengthTimeInSeconds = 0.3f;
    [SerializeField] private Vector3 indicatorOffsetPos;
    [SerializeField] private GameObject endZoneIndicator;
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
        SpawnEndZoneIndicator();
    }

    public void PlayersIsOnEndZone()
    {
        GroupBlockDetection parent = blocksChild[0].parent.GetComponent<GroupBlockDetection>();
        playerInEndZone = parent.playersOnGroupBlock;
    }

    private void SpawnEndZoneIndicator()
    {
        Vector3 indicatorSpawnPos = Vector3.Lerp(blocksChild[0].position, blocksChild[blocksChild.Count - 1].position, 0.5f) + indicatorOffsetPos;
        Instantiate(endZoneIndicator, indicatorSpawnPos, Quaternion.identity, gameObject.transform.parent);
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

    // Update is called once per frame
    void Update() // Pulsing bloc
    {
        PulsingBloc.PulsingEmissiveColorSquareBlocList(baseColor, colorTo, blocksChild, pulsingLengthTimeInSeconds);
    }
}