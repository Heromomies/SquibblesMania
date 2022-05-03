using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class RespawnZone : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;
        
        
        if (other.TryGetComponent(out PlayerStateManager player))
        {
            RespawnManager.Instance.RespawnPlayer(player);
        }

    }
}
