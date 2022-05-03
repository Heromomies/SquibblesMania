using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    private static RespawnManager _respawnManager;

    public static RespawnManager Instance => _respawnManager;
    [SerializeField]
    private Vector3 offSetRespawnPos = new Vector3(0f, 1f, 0f);
    // Start is called before the first frame update
    void Awake()
    {
        _respawnManager = this;
    }

    public void RespawnPlayer(PlayerStateManager player)
    {
        player.transform.position = player.playerRespawnPoint + offSetRespawnPos;
        Physics.SyncTransforms();
        StartCoroutine(player.PlayerRespawnUpdateBlocBelow(player)); 
        
    }
}
