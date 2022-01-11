using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private static GameManager _gameManager;

    public static GameManager Instance => _gameManager;
   

    [Header("PLAYERS MANAGER PARAMETERS")] public List<PlayerStateManager> players;

    public Transform[] playersSpawnPoints;
    public int numberPlayers;
    public PlayerStateManager playerPref;

    public PlayerStateManager currentPlayerTurn;
    [HideInInspector]
    public bool isPathRefresh;

    public int turnCount;
    [Header("VICTORY CONDITIONS")] public bool isConditionVictory;
    public ConditionVictory conditionVictory;
    private void Awake()
    {
        _gameManager = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < playersSpawnPoints.Length; i++)
        {
            //Spawn player at specific location
            Vector3 spawnPos = playersSpawnPoints[i].gameObject.GetComponent<Node>().GetWalkPoint() +
                               new Vector3(0, playerPref.transform.localScale.y / 2f, 0);

            PlayerStateManager player = Instantiate(playerPref, spawnPos, Quaternion.identity);
            player.gameObject.name = "Player " + (i + 1);
            player.playerNumber = i;
            
            players.Add(player);
        }

        players[0].playerTeam = Player.PlayerTeam.TeamOne;
        players[1].playerTeam = Player.PlayerTeam.TeamTwo;
        players[2].playerTeam = Player.PlayerTeam.TeamOne;
        players[3].playerTeam = Player.PlayerTeam.TeamOne;

        //Choose Randomly a player to start
        int numberPlayerToStart = Random.Range(0, players.Count);
        turnCount++;
        UiManager.Instance.UpdateCurrentTurnCount(turnCount);
        players[numberPlayerToStart].StartState();
        currentPlayerTurn = players[numberPlayerToStart];
    }

    public void ChangePlayerTurn(int playerNumberTurn)
    {
        turnCount++;
        UiManager.Instance.UpdateCurrentTurnCount(turnCount);
        players[playerNumberTurn].StartState();
        currentPlayerTurn = players[playerNumberTurn];
    }

   public void ShowEndZone()
    {
        if (isConditionVictory)
        {
            conditionVictory.endZone.gameObject.SetActive(true);
        }
    }
    public void PlayerTeamWin()
    {
        //TODO L'équipe x a gagner la partie on ouvre un panel (dans UIManager) et on met le jeu en pause
    }

    // Update is called once per frame
    void Update()
    {
    }

    
}