using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Ressources : Item
{
    public GameObject particleSystemPrefab;
    public GameObject winT1;
    public GameObject winT2;

    public int nbObjectiveT1 = 0;
    public int nbObjectiveT2 = 0;

    public Sprite green;

    private void OnTriggerEnter(Collider other) // When we collide with a player 
    {   //Check if current player doesnt already collide with the ressources
        if (other.gameObject.CompareTag("Player"))
        {
            var player = other.gameObject.GetComponent<PlayerStateManager>();

            AudioManager.Instance.Play("CratePickup");
            AudioManager.Instance.Play("CrateFirework");

            TeamInventoryManager.Instance.AddResourcesToInventory(1, player.playerTeam);
            var particle = Instantiate(particleSystemPrefab, transform.position, Quaternion.identity);
            Destroy(particle, 2f);

            gameObject.SetActive(false);

            if (player.playerTeam == Player.PlayerTeam.TeamOne)
            {
                winT1.transform.GetChild(nbObjectiveT1).gameObject.GetComponent<Image>().sprite = green;
                nbObjectiveT1++;
            }
            else if(player.playerTeam == Player.PlayerTeam.TeamTwo)
            {
                winT2.transform.GetChild(nbObjectiveT2).gameObject.GetComponent<Image>().sprite = green;
                nbObjectiveT2++;
            }
        }
    }
}
