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
    public PlayerStateManager playerPref;

    public PlayerStateManager currentPlayerTurn;
  
    public int turnCount;
    [Header("CAMERA PARAMETERS")] private Camera _cam;
    private CameraViewModeGesture _cameraViewModeGesture;
    public CamPreSets actualCamPreset;
   
    public List<CamPreSets> camPreSets;
    [Header("CAMERA ROTATIONS")]
    public int count;
    [SerializeField] 
    private float smoothTransitionTime = 0.3f;
    
    [Serializable]
    public struct CamPreSets
    {
        public int presetNumber;
        [Space(2f)] public Vector3 camPos;
        public Vector3 camRot;
        public GameObject buttonNextTurn;
    }


    [Header("VICTORY CONDITIONS")] public bool isConditionVictory;
    public ConditionVictory conditionVictory;
    private bool _isEndZoneShowed;
    public List<GameObject> allBlocks;
    [HideInInspector] public int cycleCount;

    private void Awake()
    {
        _gameManager = this;
        _cam = Camera.main;
        Application.targetFrameRate = 30;
        
    }


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < allBlocks.Count; i++)
        {
            int randomLocation = Random.Range(minHeightBlocMovement, maxHeightBlocMovement);
            allBlocks[i].transform.position = new Vector3(allBlocks[i].transform.position.x, randomLocation, allBlocks[i].transform.position.z);
        }
        _cameraViewModeGesture = _cam.gameObject.GetComponent<CameraViewModeGesture>();
        SpawnPlayers();
        StartGame();
    }

    void SpawnPlayers()
    {
        for (int i = 0; i < playersSpawnPoints.Length; i++)
        {
            //Spawn player at specific location
            Vector3 spawnPos = playersSpawnPoints[i].gameObject.GetComponent<Node>().GetWalkPoint() + new Vector3(0, 0.5f, 0);

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

        CamConfig(count);
        NFCManager.Instance.PlayerChangeTurn();
    }

    void CamConfig(int countTurn)
    {
        if (currentPlayerTurn.canSwitch)
        {
            if (actualCamPreset.presetNumber > 0)
            {
                actualCamPreset.buttonNextTurn.SetActive(false);
            }

            Transform cameraTransform = _cam.transform;
            Quaternion target = Quaternion.Euler(camPreSets[countTurn].camRot);
        
            //Smooth Transition
            cameraTransform.DOMove(camPreSets[countTurn].camPos, smoothTransitionTime);
            cameraTransform.DORotateQuaternion(target, smoothTransitionTime);
        
            actualCamPreset = camPreSets[countTurn];
        
            //UI SWITCH
            UiManager.Instance.SwitchUiForPlayer(actualCamPreset.buttonNextTurn);
            CameraButtonManager.Instance.SetUpUiCamPreset();
            _cameraViewModeGesture.SetUpCameraBaseViewMode();
        
            count++;
            if (count >= camPreSets.Count)
            {
                count = 0;
            }

            currentPlayerTurn.canSwitch = false;
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
        if (currentPlayerTurn.currentCardEffect)
        {
            currentPlayerTurn.currentCardEffect.SetActive(false);
            currentPlayerTurn.currentCardEffect = null;
        }
       
        currentPlayerTurn = players[playerNumberTurn];
        currentPlayerTurn.StartState();

        NFCManager.Instance.PlayerChangeTurn();
        CamConfig(count);
    }

    public void DecreaseVariable()
    {
        turnCount--;

        if(turnCount <= 0)
            turnCount=0;
        
        currentPlayerTurn = players[count -1];
        currentPlayerTurn.StartState();
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
        StartCoroutine(NFCManager.Instance.ColorOneByOneAllTheAntennas());
        Time.timeScale = 0f;
        UiManager.Instance.WinSetUp(playerTeam);
    }

    private void OnApplicationQuit()
    {
        StopAllCoroutines();
    }
}