using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
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
    public bool canSwitch = true;
    public GameObject currentCardEffect;
    [Header("ANIMATIONS")] public Animator playerAnimator;
    public enum PlayerTeam
    {
        TeamOne,
        TeamTwo,
        None
    }
}
