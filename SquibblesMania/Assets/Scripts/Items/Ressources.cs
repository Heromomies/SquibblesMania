using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ressources : Item
{
    [Serializable]
    public enum Types
    {
        Wood,
        Rock,
        Rope
    };

    public Types ressourcesTypes;

  
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (other.gameObject.GetComponent<PlayerStateManager>())
        {
            Debug.Log(other);
            PlayerStateManager player = other.gameObject.GetComponent<PlayerStateManager>();
            TeamInventoryManager.Instance.AddRessourcesToInventory(this, player.playerTeam);
            Destroy(gameObject);
        }
    }
}
