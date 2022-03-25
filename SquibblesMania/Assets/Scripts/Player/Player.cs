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
    public GameObject meshRenderer;
    
    public int stunCount;
    public bool isPlayerStun;
    public bool canSwitch = true;
    
    public enum PlayerTeam
    {
        TeamOne,
        TeamTwo,
        None
    }
}
