using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Ressources : Item
{
    public GameObject particleSystemPrefab;
    private List<PlayerStateManager> playersList = new List<PlayerStateManager>();

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
    {   //Check if current player doesnt already collid with the ressource
        if (other.gameObject.GetComponent<PlayerStateManager>())
        {
            var player = other.gameObject.GetComponent<PlayerStateManager>();
            if (!playersList.Contains(player))
            {
                Debug.Log(player);
                playersList.Add(player);
                TeamInventoryManager.Instance.AddResourcesToInventory(this, player.playerTeam);
                var particle = Instantiate(particleSystemPrefab, transform.position, Quaternion.identity);
                Destroy(particle, 2f);
            }
        }
    }
}