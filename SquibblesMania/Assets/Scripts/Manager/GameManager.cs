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
    public int numberPlayers;

    public List<PlayerStateManager> players;

    public Transform[] playersSpawnPoints;

    public PlayerStateManager playerPref;
    
    public PlayerStateManager currentPlayerTurn;
    public bool isPathRefresh;
    private void Awake()
    {
        _gameManager = this;

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
    }

    // Start is called before the first frame update
    void Start() 
    {
        //Choose Randomly a player to start
        int numberPlayerToStart = Random.Range(0, players.Count);
        
        players[numberPlayerToStart].StartState();
        currentPlayerTurn = players[numberPlayerToStart];
    }

    public void ChangePlayerTurn(int playerNumberTurn)
    {
        players[playerNumberTurn].StartState();
        currentPlayerTurn = players[playerNumberTurn];
    }
    // Update is called once per frame
    void Update()
    {
    }


    /* public struct Player
     {
         public PlayerStateManager playerStateManager;
         public int playerNumber;
 
         public enum Team
         {
             Team1,
             Team2
         };
     }*/
}