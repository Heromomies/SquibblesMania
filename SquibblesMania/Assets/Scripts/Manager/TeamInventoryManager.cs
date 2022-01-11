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
                        playerTeamInventory.items[0].isTeamHasObjet = true;
                        if (CheckForVictoryConditions(playerTeamInventory))
                        {
                            GameManager.Instance.isConditionVictory = true;
                            GameManager.Instance.ShowEndZone();
                        }

                        break;
                    case Ressources.Types.Rock:
                       
                        playerTeamInventory.items[1].isTeamHasObjet = true;
                        if (CheckForVictoryConditions(playerTeamInventory))
                        {
                            GameManager.Instance.isConditionVictory = true;
                            GameManager.Instance.ShowEndZone();
                        }

                        break;
                    case Ressources.Types.Rope:
              
                        playerTeamInventory.items[2].isTeamHasObjet = true;
                        if (CheckForVictoryConditions(playerTeamInventory))
                        {
                            GameManager.Instance.isConditionVictory = true;
                            GameManager.Instance.ShowEndZone();
                        }


                        break;
                }
            }
        }
    }

    bool CheckForVictoryConditions(Inventory inventory)
    {
        for (int i = 0; i < inventory.items.Count; i++)
        {
            if (inventory.items[i].isTeamHasObjet == false)
            {
                return false;
            }
        }

        return true;
    }
}

[System.Serializable]
public class Inventory
{
    public Player.PlayerTeam inventoryTeam;
    public List<Objet> items = new List<Objet>();
    
    [Serializable]
    public class Objet
    {
        public string objetName;
        public bool isTeamHasObjet;
    }

    /* public bool isTeamHasWood;
     public bool isTeamHasRock;
     public bool isTeamHasRope;*/
}