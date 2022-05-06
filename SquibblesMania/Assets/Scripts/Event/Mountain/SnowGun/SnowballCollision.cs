using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballCollision : MonoBehaviour
{
    public GameObject breakableIce;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.name != GameManager.Instance.currentPlayerTurn.name)
        {
            var player = other.GetComponent<PlayerStateManager>();
            player.StunPlayer(player, 2);
            Instantiate(breakableIce, other.transform.position + new Vector3(0, 0.25f, 0), Quaternion.identity, player.transform);
            gameObject.SetActive(false);
        }
    }
}
