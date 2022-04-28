using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Ressources : Item
{
    public GameObject particleSystemPrefab;


    private void OnTriggerEnter(Collider other) // When we collide with a player 
    {   //Check if current player doesnt already collide with the ressources
        if (other.gameObject.CompareTag("Player"))
        {
            var player = other.gameObject.GetComponent<PlayerStateManager>();

            if (player.playerTeam == Player.PlayerTeam.TeamOne && TeamInventoryManager.Instance.inventory[0].objectAcquired < 3 || 
                player.playerTeam == Player.PlayerTeam.TeamTwo && TeamInventoryManager.Instance.inventory[1].objectAcquired < 3)
            {
                AudioManager.Instance.Play("CratePickup");
                AudioManager.Instance.Play("CrateFirework");

                TeamInventoryManager.Instance.AddResourcesToInventory(1, player.playerTeam);
                var particle = Instantiate(particleSystemPrefab, transform.position, Quaternion.identity);
                Destroy(particle, 2f);

                gameObject.SetActive(false);
            }
        }
    }
}
