using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Player : MonoBehaviour
{
    [Header("BASE PARAMETERS")]
    public string playerName;
    public int playerActionPoint;
    public bool isUsingCardPower;
    public GameObject playerPref;
    public PlayerTeam playerTeam;
    public Color playerColor;
    [Header("CUSTOMIZATIONS PARAMETERS")]
    public Renderer indicatorPlayerRenderer;
    public GameObject playerHat;
    public Renderer playerMesh;
    public int stunCount;
    public bool isPlayerStun;
    public bool isPlayerHide;
    public bool canSwitch = true;
    public GameObject currentCardEffect;
    public Sprite spritePlayerTeam;
    public Sprite spritePlayerHat;
    [HideInInspector] public GameObject vfxStun;
    public Rigidbody playerRigidbody;
    [Header("RESPAWN PARAMETERS")] public Transform playerRespawnPoint;
    
    [Header("ANIMATIONS")] public Animator playerAnimator;
    public enum PlayerTeam
    {
        TeamOne,
        TeamTwo,
        None
    }
}
