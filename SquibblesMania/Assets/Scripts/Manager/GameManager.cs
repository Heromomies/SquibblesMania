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
    [Header("CAMERA PARAMETERS")] [SerializeField] private Camera _cam;
    [SerializeField] private CameraViewModeGesture cameraViewModeGesture;
    public CamPreSets actualCamPreset;
   
    public List<CamPreSets> camPreSets;
    [Header("CAMERA ROTATIONS")]
    public int count;
    [SerializeField] 
    private float smoothTransitionTime = 0.3f;

    [SerializeField] private List<CamPreSets> previousCamPreSetsList;
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
    public GameObject winT1;
    public GameObject winT2;

    [Space] [Header("MAP ZONE")] 
    public List<GameObject> cleanList;
    
    [Header("PLAYER CUSTOMIZATION")]
    public PlayerData playerData;
    public List<GameObject> hats = new List<GameObject>();
    public List<Material> colors = new List<Material>();

    private void Awake()
    {
        Application.targetFrameRate = 30;
        _gameManager = this;
    }


    // Start is called before the first frame update
  private void Start()
    {
        for (int i = 0; i < allBlocks.Count; i++)
        {
            int randomLocation = Random.Range(minHeightBlocMovement, maxHeightBlocMovement);
            allBlocks[i].transform.position = new Vector3(allBlocks[i].transform.position.x, randomLocation, allBlocks[i].transform.position.z);
        }
        
        SpawnPlayers();
        StartGame();
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        Setup();
    }

    private void Reset()
    {
        Setup();
    }

    private void Setup()
    {
        _cam = Camera.main;
        if (_cam != null) cameraViewModeGesture = _cam.GetComponent<CameraViewModeGesture>();
    }
#endif

  private void SpawnPlayers()
    {
        for (int i = 0; i < playersSpawnPoints.Length; i++)
        {
            //Spawn player at specific location
            if (playersSpawnPoints[i].gameObject.TryGetComponent(out Node playerNodeSpawnPoint))
            {
                Vector3 spawnPos = playerNodeSpawnPoint.GetWalkPoint() + new Vector3(0, 0.5f, 0);
                PlayerStateManager player = Instantiate(playerPref, spawnPos, Quaternion.identity);
                player.playerRespawnPoint = spawnPos;
                player.currentBlocPlayerOn = playersSpawnPoints[i].transform;
                player.gameObject.name = "Player " + (i + 1);
                player.playerNumber = i;
                players.Add(player);
            }
        }
        SetUpPlayers();
      
    }

  private void SetPlayerTeam(PlayerStateManager player, Player.PlayerTeam playerTeam, Color color, Material playerCustomMat)
  {
      player.playerTeam = playerTeam;
      player.gameObject.GetComponentInChildren<Renderer>().material.color = color;
      player.indicatorPlayer.SetActive(false);
      player.playerMesh.material = playerCustomMat;
  }
  
    void SetUpPlayers()
    {
        SetPlayerTeam(players[0], Player.PlayerTeam.TeamOne, Color.red, colors[playerData.P1colorID] );
        Instantiate(hats[playerData.P1hatID], players[0].playerHat.transform.position, players[0].playerHat.transform.rotation).transform.parent = players[0].playerHat.transform;
        
        SetPlayerTeam(players[1], Player.PlayerTeam.TeamTwo, Color.blue, colors[playerData.P2colorID]);
        Instantiate(hats[playerData.P2hatID], players[1].playerHat.transform.position, players[1].playerHat.transform.rotation).transform.parent = players[1].playerHat.transform; ;

        SetPlayerTeam(players[2], Player.PlayerTeam.TeamOne, Color.red,colors[playerData.P3colorID] );
        Instantiate(hats[playerData.P3hatID], players[2].playerHat.transform.position, players[2].playerHat.transform.rotation).transform.parent = players[2].playerHat.transform; ;
        
        SetPlayerTeam(players[3], Player.PlayerTeam.TeamTwo, Color.blue, colors[playerData.P4colorID]);
        Instantiate(hats[playerData.P4hatID], players[3].playerHat.transform.position, players[3].playerHat.transform.rotation).transform.parent = players[3].playerHat.transform;
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

    public void SetUpPlayerMaterial(PlayerStateManager player, int playerNumber)
    {
        switch (playerNumber)
        {
            case 0 : player.playerMesh.material = colors[playerData.P1colorID]; break;
            case 1 : player.playerMesh.material = colors[playerData.P2colorID]; break;
            case 2 : player.playerMesh.material = colors[playerData.P3colorID]; break;
            case 3 : player.playerMesh.material = colors[playerData.P4colorID]; break; 
        }
    }
    
    void CamConfig(int countTurn)
    {
        if (currentPlayerTurn.canSwitch)
        {
            if (actualCamPreset.presetNumber > 0)
            {
                actualCamPreset.buttonNextTurn.SetActive(false);
            }
            
            actualCamPreset = camPreSets[countTurn];
            
            Transform cameraTransform = _cam.transform;
            Quaternion target = Quaternion.Euler(actualCamPreset.camRot);
            
            //Smooth Transition
            cameraTransform.DOMove(actualCamPreset.camPos, smoothTransitionTime);
            cameraTransform.DORotateQuaternion(target, smoothTransitionTime);
            
            //UI SWITCH
            UiManager.Instance.SwitchUiForPlayer(actualCamPreset.buttonNextTurn);
            CameraButtonManager.Instance.SetUpUiCamPreset();
            
            //Register Previous Cam View Mode
            if (turnCount <= 4)
            {
                cameraViewModeGesture.SetUpCameraViewMode(true, 1);
            }
            else
            {
                cameraViewModeGesture.SetUpCameraViewMode(false, count);
            }
            
            currentPlayerTurn.canSwitch = false;
        }
    }

    private void SavePreviousCamRotY(int indexCam)
    {
        Vector3 camEulerAngles = _cam.transform.eulerAngles;
        Vector3 camPos = _cam.transform.position;
        
        CamPreSets previousCamPreSets = previousCamPreSetsList[indexCam];

        previousCamPreSets.camRot = camEulerAngles;
        previousCamPreSets.camPos = new Vector3(Mathf.Round(camPos.x), Mathf.Round(camPos.y), Mathf.Round(camPos.z));
        previousCamPreSets.presetNumber = camPreSets[indexCam].presetNumber;
        previousCamPreSets.buttonNextTurn = camPreSets[indexCam].buttonNextTurn;

        previousCamPreSetsList[indexCam] = previousCamPreSets;
        
        camPreSets[indexCam] = previousCamPreSetsList[indexCam];
    }

    private void IncreaseDemiCycle()
    {
        if (conditionVictory.mapTheme == ConditionVictory.Theme.Volcano)
        {
            VolcanoManager.Instance.CyclePassed();
        }

        PowerManager.Instance.CyclePassed();
    }

    public void ChangePlayerTurn(int playerNumberTurn)
    {
        if (playerNumberTurn == players[0].playerNumber || playerNumberTurn == players[2].playerNumber)
        {
            IncreaseDemiCycle();
            cycleCount++;
        }

        if (conditionVictory.mapTheme == ConditionVictory.Theme.Mountain)
        {
            if (playerNumberTurn == players[3].playerNumber)
            {
                MountainManager.Instance.ChangeCycle();
            }
            MountainManager.Instance.ChangeTurn();
        }

        turnCount++;
        if (currentPlayerTurn.currentCardEffect)
        {
            currentPlayerTurn.currentCardEffect.SetActive(false);
            currentPlayerTurn.currentCardEffect = null;
        }
        
        
        SavePreviousCamRotY(count);
        cameraViewModeGesture.SavePreviousViewModeGesture(count);
        count = (count + 1) % camPreSets.Count; 
        CamConfig(count);
        
        currentPlayerTurn = players[playerNumberTurn];
        currentPlayerTurn.StartState();

        NFCManager.Instance.PlayerChangeTurn();

        if (UiManager.Instance.textActionPointPopUp)
        {
            UiManager.Instance.textActionPointPopUp.SetActive(false);
            UiManager.Instance.textActionPointPopUp = null;
        }

        if (playerNumberTurn == players[0].playerNumber || playerNumberTurn == players[2].playerNumber)
        {
            winT2.SetActive(false);
            winT1.SetActive(true);
        }
        else
        {
            winT1.SetActive(false);
            winT2.SetActive(true);
        }
    }

    public void DecreaseVariable()
    {
        turnCount--;

        if(turnCount <= 0)
            turnCount=0;

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

    public void DetectParentBelowPlayers()
    {
        foreach (var player in players)
        {
            if (player.currentBlocPlayerOn.TryGetComponent(out Node currentPlayerNode))
            {
                currentPlayerNode.isActive = true;
                currentPlayerNode.GetComponentInParent<GroupBlockDetection>().playersOnGroupBlock.Remove(player.transform);
            }
           
            Ray ray = new Ray(player.transform.position, -transform.up);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, 1.1f))
            {
                if (hit.collider.gameObject.TryGetComponent(out Node node))
                {
                    player.currentBlocPlayerOn = hit.transform;
                }
            }
            
            player.currentBlocPlayerOn.GetComponent<Node>().isActive = false;
            
            GroupBlockDetection groupBlockDetection = player.currentBlocPlayerOn.transform.GetComponentInParent<GroupBlockDetection>();
            if (!groupBlockDetection.playersOnGroupBlock.Contains(player.transform))
            {
                groupBlockDetection.playersOnGroupBlock.Add(player.transform);
            }
        }
        
        if (conditionVictory.mapTheme == ConditionVictory.Theme.Mountain)
        {
            MountainManager.Instance.wind.CheckIfPlayersAreHide();
        }
    }

    public void PlayerTeamWin(Player.PlayerTeam playerTeam)
    {
        StartCoroutine(NFCManager.Instance.ColorOneByOneAllTheAntennas());
     
        UiManager.Instance.WinSetUp(playerTeam);
    }

    private void OnApplicationQuit()
    {
        StopAllCoroutines();
    }
}