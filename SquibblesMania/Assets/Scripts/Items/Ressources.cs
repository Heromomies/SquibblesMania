using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Ressources : Item
{
    public GameObject particleSystemPrefab;

    private void OnTriggerEnter(Collider other) // When we collide with a player 
    {   //Check if current player doesnt already collide with the ressources
        if (other.gameObject.CompareTag("Player"))
        {
            var player = other.gameObject.GetComponent<PlayerStateManager>();
           
            TeamInventoryManager.Instance.AddResourcesToInventory(1, player.playerTeam);
            var particle = Instantiate(particleSystemPrefab, transform.position, Quaternion.identity);
            Destroy(particle, 2f);
                
            gameObject.SetActive(false);
        }
    }
}