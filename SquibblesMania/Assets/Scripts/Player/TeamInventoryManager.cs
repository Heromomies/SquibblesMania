using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class TeamInventoryManager : MonoBehaviour
{
    public Inventory[] inventory;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddRessourcesToInventory(Ressources ressources, Player.PlayerTeam playerTeam)
    {
        foreach (Inventory vInventory in inventory)
        {
            if (vInventory.inventoryTeam == playerTeam)
            {
                vInventory.ressources.Add(ressources);
            }
            else
            {
                return;
            }
        }
     
    }
    
    void CheckForRessources(Ressources.Types types)
    {
        //TODO Une fois arrivé au point précis on check si le joueur a remplis toute les conditions pour pouvoir gagner la partie
      
    }
    
}
[System.Serializable]
public class Inventory
{
    public Player.PlayerTeam inventoryTeam;
    
    public List<Ressources> ressources;
    public int totalRessources;
    
    public int woodCount;
    public int rockCount;
    public int ropeCount;

}
