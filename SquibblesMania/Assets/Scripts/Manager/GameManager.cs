using System;

using System.Collections.Generic;
using DigitalRubyShared;
using TMPro;

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
    public bool isPathRefresh;
    public int turnCount;
    [Header("CAMERA PARAMETERS")] public FingersPanOrbitComponentScript cameraTouchScript;

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
        public GameObject uiGameObject;
        public GameObject panelButtonEvent;
        public GameObject buttonNextTurn;
        public TextMeshProUGUI actionPointText;
    }


    [Header("VICTORY CONDITIONS")] public bool isConditionVictory;
    public ConditionVictory conditionVictory;
    private bool _isEndZoneShowed;
    public List<GameObject> allBlocks;
    [HideInInspector] public int cycleCount;
    private void Awake()
    {
        _gameManager = this;
        if (cameraTouchScript != null)
        {
            cameraTouchScript = Camera.main.GetComponent<FingersPanOrbitComponentScript>();
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < allBlocks.Count; i++)
        {
            int randomLocation = Random.Range(0, 2);
            allBlocks[i].transform.position = new Vector3(allBlocks[i].transform.position.x, randomLocation,
                allBlocks[i].transform.position.z);
        }

        SpawnPlayers();
        StartGame();
    }

    void SpawnPlayers()
    {
        for (int i = 0; i < playersSpawnPoints.Length; i++)
        {
            //Spawn player at specific location
            Vector3 spawnPos = playersSpawnPoints[i].gameObject.GetComponent<Node>().GetWalkPoint() +
                               new Vector3(0, playerPref.transform.localScale.y / 2f, 0);

            PlayerStateManager player = Instantiate(playerPref, spawnPos, Quaternion.identity);
            player.currentBlockPlayerOn = playersSpawnPoints[i].transform;
            player.gameObject.name = "Player " + (i + 1);
            player.playerNumber = i;

            players.Add(player);
        }

        players[0].playerTeam = Player.PlayerTeam.TeamOne;
        players[0].gameObject.GetComponent<Renderer>().material.color = Color.red;
        players[1].playerTeam = Player.PlayerTeam.TeamTwo;
        players[1].gameObject.GetComponent<Renderer>().material.color = Color.blue;
        players[2].playerTeam = Player.PlayerTeam.TeamOne;
        players[2].gameObject.GetComponent<Renderer>().material.color = Color.red;
        players[3].playerTeam = Player.PlayerTeam.TeamTwo;
        players[3].gameObject.GetComponent<Renderer>().material.color = Color.blue;
        
    }

    void StartGame()
    {
        //Choose Randomly a player to start
       
        int numberPlayerToStart = 0;
        turnCount++;
        UiManager.Instance.UpdateCurrentTurnCount(turnCount);
        
        currentPlayerTurn = players[numberPlayerToStart];
        currentPlayerTurn.StartState();
        
        CamConfig(_count);
        NFCManager.Instance.PlayerChangeTurn();
        //playerPlaying.text = "Player turn : " + players[numberPlayerToStart].name;
    }
    
    void CamConfig(int countTurn)
    {
        if (actualCamPreset.presetNumber > 0)
        {
            actualCamPreset.uiGameObject.SetActive(false);
            actualCamPreset.buttonNextTurn.SetActive(false);
        }
        TouchManager.Instance.RemoveFingerScriptPassThroughObject();
        
        Transform cameraTransform = cameraTouchScript.transform;

        cameraTransform.position = camPreSets[countTurn].camPos;

        Quaternion target = Quaternion.Euler(camPreSets[countTurn].camRot);
        
        cameraTransform.rotation = target;
        actualCamPreset = camPreSets[countTurn];
        
        //UI SWITCH
        UiManager.Instance.SwitchUiForPlayer(actualCamPreset.buttonNextTurn, actualCamPreset.actionPointText);
        actualCamPreset.uiGameObject.SetActive(true);
        TouchManager.Instance.AddFingerScriptPassTroughObject();
       
        
        if (actualCamPreset.presetNumber == 2 || actualCamPreset.presetNumber == 3)
        {
           cameraTouchScript.OrbitYMaxDegrees = 0;
           cameraTouchScript.OrbitXMaxDegrees = 0;
        }
        else
        {
            cameraTouchScript.OrbitXMaxDegrees = 0;
            cameraTouchScript.OrbitYMaxDegrees = 0;
        }

        _count++;
        if (_count >= camPreSets.Count)
        {
            _count = 0;
        }
    }

    private void IncreaseCycle()
    {
        EventManager.Instance.CyclePassed();
    }
    public void ChangePlayerTurn(int playerNumberTurn)
    {
        if (playerNumberTurn == players[0].playerNumber)
        {
            IncreaseCycle();
            cycleCount++;
        }
        
        turnCount++;
        UiManager.Instance.UpdateCurrentTurnCount(turnCount);
        
        currentPlayerTurn = players[playerNumberTurn];
        currentPlayerTurn.StartState();
       
        NFCManager.Instance.PlayerChangeTurn();
        CamConfig(_count);
        if (CameraButtonManager.Instance.enabled)
        {
            UiManager.Instance.PlayerChangeCamButton();
        }
    }

    public void ShowEndZone()
    {
        if (isConditionVictory && !_isEndZoneShowed)
        {
            int randomNumberEndSpawnPoint = Random.Range(0, conditionVictory.endZoneSpawnPoints.Length);
            GameObject endZone = Instantiate(conditionVictory.endZone, conditionVictory.endZoneSpawnPoints[randomNumberEndSpawnPoint]);
            endZone.transform.position = conditionVictory.endZoneSpawnPoints[randomNumberEndSpawnPoint].position;
            isConditionVictory = false;
            _isEndZoneShowed = true;
        }
    }

    public void PlayerTeamWin(Player.PlayerTeam playerTeam)
    {

       StartCoroutine( NFCManager.Instance.ColorOneByOneAllTheAntennas());
        //TODO L'équipe x a gagner la partie on ouvre un panel (dans UIManager) et on met le jeu en pause
        Time.timeScale = 0f;
        UiManager.Instance.WinSetUp(playerTeam);
    }

    // Update is called once per frame
    void Update()
    {
    }
}