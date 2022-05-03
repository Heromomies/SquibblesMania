using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    private static RespawnManager _respawnManager;

    public static RespawnManager Instance => _respawnManager;
    [SerializeField]
    private Vector3 offsetRespawnPos = new Vector3(0f, 1f, 0f);
    void Awake()
    {
        _respawnManager = this;
    }

    public void RespawnPlayer(PlayerStateManager player)
    {
        player.transform.position = player.playerRespawnPoint + offsetRespawnPos;
        Physics.SyncTransforms();

        if (player.isPlayerInActionCardState)
        {
            StartCoroutine(player.PlayerRespawnUpdateBlocBelow(player)); 
        }
        
    }
}