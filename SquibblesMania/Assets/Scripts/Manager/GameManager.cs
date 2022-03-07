using System;
using System.Collections.Generic;
using DG.Tweening;
using DigitalRubyShared;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private static GameManager _gameManager;

    public static GameManager Instance => _gameManager;
    public int maxHeightBlocMovement, minHeightBlocMovement;

    [Header("PLAYERS MANAGER PARAMETERS")] public List<PlayerStateManager> players;
    public Transform[] playersSpawnPoints;
    public int numberPlayers;
    public PlayerStateManager playerPref;

    public PlayerStateManager currentPlayerTurn;
  
    public int turnCount;
    [Header("CAMERA PARAMETERS")] public FingersPanOrbitComponentScript cameraTouchScript;

    public CamPreSets actualCamPreset;
   
    public List<CamPreSets> camPreSets;
    [Header("CAMERA ROTATIONS")]
    [SerializeField]
    private float camRotateXMaxDegrees = 50f;
    [SerializeField]
    private float camRotateXMinDegrees = 20f;
    private int _count;
    [SerializeField] 
    private float smoothTransitionTime = 0.3f;

    [SerializeField] private float cameraOrthoBaseSize = 11f;
    [Serializable]
    public struct CamPreSets
    {
        public int presetNumber;
        [Space(2f)] public Vector3 camPos;
        public Vector3 camRot;
        public GameObject playerUiButtons;
        public GameObject buttonNextTurn;
        public TextMeshProUGUI actionPointText;
    }


    [Header("VICTORY CONDITIONS")] public bool isConditionVictory;
    public GameObject crown;
    public float heightCrownSpawn;
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

        Application.targetFrameRate = 60;
    }


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < allBlocks.Count; i++)
        {
            int randomLocation = Random.Range(minHeightBlocMovement, maxHeightBlocMovement);
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
                               new Vector3(0, 0.5f, 0);

            PlayerStateManager player = Instantiate(playerPref, spawnPos, Quaternion.identity);
            player.currentBlockPlayerOn = playersSpawnPoints[i].transform;
            player.gameObject.name = "Player " + (i + 1);
            player.playerNumber = i;

            players.Add(player);
        }
        SetUpPlayers();
      
    }

    void SetUpPlayers()
    {
        players[0].playerTeam = Player.PlayerTeam.TeamOne;
        players[0].gameObject.GetComponentInChildren<Renderer>().material.color = Color.red;
        players[0].indicatorPlayer.SetActive(false);
        players[1].playerTeam = Player.PlayerTeam.TeamTwo;
        players[1].gameObject.GetComponentInChildren<Renderer>().material.color = Color.blue;
        players[1].indicatorPlayer.SetActive(false);
        players[2].playerTeam = Player.PlayerTeam.TeamOne;
        players[2].gameObject.GetComponentInChildren<Renderer>().material.color = Color.red;
        players[2].indicatorPlayer.SetActive(false);
        players[3].playerTeam = Player.PlayerTeam.TeamTwo;
        players[3].gameObject.GetComponentInChildren<Renderer>().material.color = Color.blue;
        players[3].indicatorPlayer.SetActive(false);
    }

    void StartGame()
    {
        //Choose Randomly a player to start

        int numberPlayerToStart = 0;
        turnCount++;
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
            actualCamPreset.playerUiButtons.SetActive(false);
            actualCamPreset.buttonNextTurn.SetActive(false);
        }

        TouchManager.Instance.RemoveFingerScriptPassThroughObject();

        Transform cameraTransform = cameraTouchScript.transform;
        Quaternion target = Quaternion.Euler(camPreSets[countTurn].camRot);
        
        //Smooth Transition
        cameraTransform.DOMove(camPreSets[countTurn].camPos, smoothTransitionTime);
        cameraTransform.DORotateQuaternion(target, smoothTransitionTime);
        
        actualCamPreset = camPreSets[countTurn];
        
        //UI SWITCH
        UiManager.Instance.SwitchUiForPlayer(actualCamPreset.buttonNextTurn, actualCamPreset.actionPointText);
        actualCamPreset.playerUiButtons.SetActive(true);
        TouchManager.Instance.AddFingerScriptPassTroughObject();
            
        
        if (actualCamPreset.presetNumber == 1 || actualCamPreset.presetNumber == 2)
        {
            ResetCamVars();
            cameraTouchScript.OrbitYMaxDegrees = 0;
            cameraTouchScript.orbitXMaxDegrees = camRotateXMaxDegrees;
            cameraTouchScript.orbitXMinDegrees = -camRotateXMinDegrees;
        }
        else
        {
            ResetCamVars();
            cameraTouchScript.orbitXMaxDegrees = camRotateXMinDegrees;
            cameraTouchScript.orbitXMinDegrees = -camRotateXMaxDegrees;
            cameraTouchScript.OrbitYMaxDegrees = 0;
        }

      // cameraTouchScript.OrbitTarget = currentPlayerTurn.transform;
        _count++;
        if (_count >= camPreSets.Count)
        {
            _count = 0;
        }
    }
    public void ResetCamVars()
    {
        cameraTouchScript.panVelocity = Vector2.zero;
        cameraTouchScript.xDegrees = 0f;
        cameraTouchScript.cameraSize = cameraOrthoBaseSize;
        Camera.main.orthographicSize = cameraOrthoBaseSize;
        foreach (var camera in cameraTouchScript.cams)
        {
            camera.orthographicSize = cameraOrthoBaseSize;
        }
        
    }

    private void IncreaseDemiCycle()
    {
        EventManager.Instance.CyclePassed();
        PowerManager.Instance.CyclePassed();
    }

    public void ChangePlayerTurn(int playerNumberTurn)
    {
        if (playerNumberTurn == players[0].playerNumber || playerNumberTurn == players[2].playerNumber)
        {
            IncreaseDemiCycle();
            cycleCount++;
        }

        turnCount++;

        currentPlayerTurn = players[playerNumberTurn];
        currentPlayerTurn.StartState();

        NFCManager.Instance.PlayerChangeTurn();
        CamConfig(_count);
      
    }

    public void ShowEndZone()
    {
        if (isConditionVictory && !_isEndZoneShowed)
        {
            int randomNumberEndSpawnPoint = Random.Range(0, conditionVictory.endZoneSpawnPoints.Length);
            GameObject endZone = Instantiate(conditionVictory.endZone,
                conditionVictory.endZoneSpawnPoints[randomNumberEndSpawnPoint]);
            endZone.transform.position = conditionVictory.endZoneSpawnPoints[randomNumberEndSpawnPoint].position;

            isConditionVictory = false;
            _isEndZoneShowed = true;
        }
    }

    public void PlayerTeamWin(Player.PlayerTeam playerTeam)
    {
        Debug.Log("Player Win");
        StartCoroutine(NFCManager.Instance.ColorOneByOneAllTheAntennas());
        Time.timeScale = 0f;
        UiManager.Instance.WinSetUp(playerTeam);
    }

    private void OnApplicationQuit()
    {
        StopAllCoroutines();
    }
}