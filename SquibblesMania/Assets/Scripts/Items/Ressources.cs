using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ressources : Item
{
    public GameObject particleSystemPrefab;
    
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
        if (other.gameObject.GetComponent<PlayerStateManager>())
        {
            PlayerStateManager player = other.gameObject.GetComponent<PlayerStateManager>();
            TeamInventoryManager.Instance.AddResourcesToInventory(this, player.playerTeam);
            GameObject particle = Instantiate(particleSystemPrefab, transform.position, Quaternion.identity);
            Destroy(particle, 2f);
        }
    }
}
