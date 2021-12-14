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
    public PlayerStateManager playerPrefab;


    private void Awake()
    {
        _gameManager = this;

        /* for (int i = 0; i < numberPlayers; i++)
         {
             PlayerStateManager player = Instantiate(playerPrefab, Vector3.up, quaternion.identity);
             player.playerNumber = i;
             players.Add(player);
         }*/
    }

    // Start is called before the first frame update
    void Start()
    {
        //Choose Randomly a player to start
        int numberPlayerToStart = Random.Range(0, players.Count);
        players[numberPlayerToStart].StartState();
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