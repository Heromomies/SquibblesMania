using System;
using System.Collections.Generic;
using TMPro;
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

    public void AddResourcesToInventory(Ressources resources, Player.PlayerTeam playerTeam)
    {
        foreach (Inventory playerTeamInventory in inventory)
        {
            if (playerTeamInventory.inventoryTeam == playerTeam)
            {
                switch (resources.ressourcesTypes)
                {
                    case Ressources.Types.Wood:
                        playerTeamInventory.items[0].isTeamHasObjet = true;
                        playerTeamInventory.items[0].objetTextUi.color = Color.green;
                        if (CheckForVictoryConditions(playerTeamInventory) && !playerTeamInventory.isAllObjetAcquired)
                        {
                            GameManager.Instance.isConditionVictory = true;
                            GameManager.Instance.ShowEndZone();
                        }

                        break;
                    case Ressources.Types.Rock:

                        playerTeamInventory.items[1].isTeamHasObjet = true;
                        playerTeamInventory.items[1].objetTextUi.color = Color.green;
                        if (CheckForVictoryConditions(playerTeamInventory) && !playerTeamInventory.isAllObjetAcquired)
                        {
                            GameManager.Instance.isConditionVictory = true;
                            GameManager.Instance.ShowEndZone();
                        }

                        break;
                    case Ressources.Types.Rope:

                        playerTeamInventory.items[2].isTeamHasObjet = true;
                        playerTeamInventory.items[2].objetTextUi.color = Color.green;
                        if (CheckForVictoryConditions(playerTeamInventory) && !playerTeamInventory.isAllObjetAcquired)
                        {
                            GameManager.Instance.isConditionVictory = true;
                            GameManager.Instance.ShowEndZone();
                        }

                        break;
                }
            }
        }
    }

    bool CheckForVictoryConditions(Inventory playerInventory)
    {
        for (int i = 0; i < playerInventory.items.Count; i++)
        {
            if (playerInventory.items[i].isTeamHasObjet == false)
            {
                return false;
            }
        }

        playerInventory.isAllObjetAcquired = true;
        return true;
    }
}

[System.Serializable]
public class Inventory
{
    public Player.PlayerTeam inventoryTeam;
    public List<Objet> items = new List<Objet>();
    public bool isAllObjetAcquired;

    [Serializable]
    public class Objet
    {
        public string objetName;
        public bool isTeamHasObjet;
        public TextMeshProUGUI objetTextUi;
    }
}