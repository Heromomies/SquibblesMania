using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player : MonoBehaviour
{
    [Header("BASE PARAMETERS")]
    public string playerName;
    public int playerActionPoint;
    public bool isUsingCardPower;
    public GameObject playerPref;
    public PlayerTeam playerTeam;
    public GameObject indicatorPlayer;
    public GameObject hat;
    public Renderer playerMesh;
    public int stunCount;
    public bool isPlayerStun;
    public bool isPlayerHide;
    public bool canSwitch = true;
    public GameObject currentCardEffect;

    [HideInInspector] public GameObject vfxStun;
    
    [Header("RESPAWN PARAMETERS")] public Vector3 playerRespawnPoint;
    
    [Header("ANIMATIONS")] public Animator playerAnimator;
    public enum PlayerTeam
    {
        TeamOne,
        TeamTwo,
        None
    }
}
