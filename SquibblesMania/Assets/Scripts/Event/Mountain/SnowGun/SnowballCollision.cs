using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballCollision : MonoBehaviour
{
    public GameObject iceStunPlayer;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.name != GameManager.Instance.currentPlayerTurn.name)
        {
            var player = other.GetComponent<PlayerStateManager>();
           
            PlayerStateEventManager.Instance.PlayerStunTriggerEnter(player, 1);
            if (GameManager.Instance.currentPlayerTurn.isPlayerStun) 
            { 
                PlayerStateEventManager.Instance.PlayerStunTextTriggerEnter(GameManager.Instance.actualCamPreset.presetNumber, true);
            }
            player.vfxStun = Instantiate(iceStunPlayer, other.transform.position + new Vector3(0, 0.25f, 0), Quaternion.identity, player.transform);
            AudioManager.Instance.Play("FreezeOn");
            
            gameObject.SetActive(false);
        }
    }
}
