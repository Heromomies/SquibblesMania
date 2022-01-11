using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class TeamInventoryManager : MonoBehaviour
{
    public Inventory[] inventory;


    private static TeamInventoryManager _teamInventoryManager;

    public static TeamInventoryManager Instance => _teamInventoryManager;

    // Start is called before the first frame update
    void Awake()
    {
        _teamInventoryManager = this;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void AddRessourcesToInventory(Ressources ressources, Player.PlayerTeam playerTeam)
    {
        foreach (Inventory playerTeamInventory in inventory)
        {
            if (playerTeamInventory.inventoryTeam == playerTeam)
            {
                switch (ressources.ressourcesTypes)
                {
                    case Ressources.Types.Wood:
                        playerTeamInventory.isTeamHasWood = true;
                        CheckForVictoryConditions(playerTeamInventory);
                        break;
                    case Ressources.Types.Rock:
                        playerTeamInventory.isTeamHasRock = true;
                        CheckForVictoryConditions(playerTeamInventory);
                        break;
                    case Ressources.Types.Rope:
                        playerTeamInventory.isTeamHasRope = true;
                        CheckForVictoryConditions(playerTeamInventory);
                        break;
                }
            }
        }
    }

    void CheckForVictoryConditions(Inventory inventory)
    {
        if (inventory.isTeamHasRock && inventory.isTeamHasRope && inventory.isTeamHasWood)
        {
            GameManager.Instance.isConditionVictory = true;
            GameManager.Instance.ShowEndZone();
        }
        
    }
}

[System.Serializable]
public class Inventory
{
    public Player.PlayerTeam inventoryTeam;

    public bool isTeamHasWood;
    public bool isTeamHasRock;
    public bool isTeamHasRope;
}