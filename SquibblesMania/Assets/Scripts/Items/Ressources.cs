using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Ressources : Item
{
    public GameObject particleSystemPrefab;
    private List<PlayerStateManager> _playersList = new List<PlayerStateManager>();

    private void OnTriggerEnter(Collider other) // When we collide with a player 
    {   //Check if current player doesnt already collid with the ressource
        if (other.gameObject.CompareTag("Player"))
        {
            var player = other.gameObject.GetComponent<PlayerStateManager>();
            if (!_playersList.Contains(player))
            {
                _playersList.Add(player);
                TeamInventoryManager.Instance.AddResourcesToInventory(1, player.playerTeam);
                var particle = Instantiate(particleSystemPrefab, transform.position, Quaternion.identity);
                Destroy(particle, 2f);
                
                gameObject.SetActive(false);
            }
        }
    }
}