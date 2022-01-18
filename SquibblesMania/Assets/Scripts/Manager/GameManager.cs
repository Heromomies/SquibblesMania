using System;
using System.Collections;
using System.Collections.Generic;
using DigitalRubyShared;
using TMPro;
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
    [HideInInspector] public bool isPathRefresh;
    public int turnCount;
    [Header("CAMERA PARAMETERS")] public FingersPanOrbitComponentScript cameraScript;

    public CamPreSets actualCamPreset;

    public List<CamPreSets> camPreSets;
    private int _count;

    [Serializable]
    public struct CamPreSets
    {
        public int presetNumber;
        [Space(2f)] public Vector3 camPos;
        public Vector3 camRot;
        public float rotateClamp;
        public List<GameObject> uiGameObjects;
    }

    public TextMeshProUGUI playerPlaying;
    [Header("VICTORY CONDITIONS")] public bool isConditionVictory;
    public ConditionVictory conditionVictory;
    public List<GameObject> allBlocks;

    private void Awake()
    {
        _gameManager = this;
        if (cameraScript != null)
        {
            cameraScript = Camera.main.GetComponent<FingersPanOrbitComponentScript>();
        }

       
    }


    // Start is called before the first frame update
    void Start()
    {
       
        SpawnPlayers();
        StartGame();
        for (int i = 0; i < allBlocks.Count; i++)
        {
            int randomLocation = Random.Range(0, 2);
            allBlocks[i].transform.position = new Vector3(allBlocks[i].transform.position.x, randomLocation,
                allBlocks[i].transform.position.z);
        }
        
    }

    void SpawnPlayers()
    {
        for (int i = 0; i < playersSpawnPoints.Length; i++)
        {
            //Spawn player at specific location
            Vector3 spawnPos = playersSpawnPoints[i].gameObject.GetComponent<Node>().GetWalkPoint() +
                               new Vector3(0, playerPref.transform.localScale.y/2f, 0);

            PlayerStateManager player = Instantiate(playerPref, spawnPos, Quaternion.identity);
            player.gameObject.name = "Player " + (i + 1);
            player.playerNumber = i;

            players.Add(player);
        }

        players[0].playerTeam = Player.PlayerTeam.TeamOne;
        players[1].playerTeam = Player.PlayerTeam.TeamTwo;
        players[2].playerTeam = Player.PlayerTeam.TeamOne;
        players[3].playerTeam = Player.PlayerTeam.TeamOne;
    }

    void StartGame()
    {
        //Choose Randomly a player to start
        int numberPlayerToStart = Random.Range(0, players.Count);
        turnCount++;
        UiManager.Instance.UpdateCurrentTurnCount(turnCount);
        players[numberPlayerToStart].StartState();
        currentPlayerTurn = players[numberPlayerToStart];
        CamConfig(_count);
        //playerPlaying.text = "Player turn : " + players[numberPlayerToStart].name;
    }

    void CamConfig(int countTurn)
    {
        Transform cameraTransform = cameraScript.transform;
        cameraTransform.position = camPreSets[countTurn].camPos;
        cameraTransform.eulerAngles = camPreSets[countTurn].camRot;
        actualCamPreset = camPreSets[countTurn];


        if (actualCamPreset.presetNumber == 2 || actualCamPreset.presetNumber == 3)
        {
            cameraScript.OrbitYMaxDegrees = actualCamPreset.rotateClamp;
            cameraScript.OrbitXMaxDegrees = 0;

        }
        else
        {
            cameraScript.OrbitXMaxDegrees = actualCamPreset.rotateClamp;
            cameraScript.OrbitYMaxDegrees = 0;
        }

        _count++;
        if (_count >= camPreSets.Count)
        {
            _count = 0;
        }
    }

    public void ChangePlayerTurn(int playerNumberTurn)
    {
        turnCount++;
        UiManager.Instance.UpdateCurrentTurnCount(turnCount);
        players[playerNumberTurn].StartState();
        currentPlayerTurn = players[playerNumberTurn];
        playerPlaying.text = "Player turn : " + players[playerNumberTurn].name;

        CamConfig(_count);
    }

    public void ShowEndZone()
    {
        if (isConditionVictory)
        {
            int randomNumberEndSpawnPoint = Random.Range(0, conditionVictory.endZoneSpawnPoints.Length);
            GameObject endZone = Instantiate(conditionVictory.endZone,
                conditionVictory.endZoneSpawnPoints[randomNumberEndSpawnPoint]);
            endZone.transform.position = conditionVictory.endZoneSpawnPoints[randomNumberEndSpawnPoint].position;
        }
    }

    public void PlayerTeamWin(Player.PlayerTeam playerTeam)
    {
        //TODO L'équipe x a gagner la partie on ouvre un panel (dans UIManager) et on met le jeu en pause
        Time.timeScale = 0f;
        UiManager.Instance.WinSetUp(playerTeam);
    }

    // Update is called once per frame
    void Update()
    {
    }
}