using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DigitalRubyShared;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private static GameManager _gameManager;

    public static GameManager Instance => _gameManager;
    public int maxHeightBlocMovement, minHeightBlocMovement;

    [Header("PLAYERS MANAGER PARAMETERS")] public List<PlayerStateManager> players;
    public List<Transform> playersSpawnPoints = new List<Transform>();
    public PlayerStateManager playerPref;

    public PlayerStateManager currentPlayerTurn;
  
    public int turnCount;
    [Header("CAMERA PARAMETERS")] [SerializeField] private Camera _cam;
    public CameraViewModeGesture cameraViewModeGesture;
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
        public Slider sliderNextTurn;
    }
    
    [Header("VICTORY CONDITIONS")] public bool isConditionVictory;
    public ConditionVictory conditionVictory;
    public Volume volume;
    private bool _isEndZoneShowed;
    public List<GameObject> allBlocParents;
    public List<GameObject> winConditonsList = new List<GameObject>();
    [Space] [Header("MAP ZONE")] 
    public List<GameObject> cleanList;
    
    [Header("PLAYER CUSTOMIZATION")]
    public PlayerData playerData;
    public List<GameObject> hats = new List<GameObject>();
    public List<Material> colors = new List<Material>();
    public GameObject spawnPointSpriteParent;
    public List<Color> playerColors = new List<Color>();
    
    private void Awake()
    {
        Application.targetFrameRate = 30;
        _gameManager = this;
        volume.profile.Reset();
    }
    
    // Start is called before the first frame update
    private void Start()
    {
        if (MapGeneratorManager.Instance != null)
        {
            MapGeneratorManager.Instance.SetupMap();
        }
        
        for (int i = 0; i < allBlocParents.Count; i++)
        {
            var randomLocation = Random.Range(minHeightBlocMovement, maxHeightBlocMovement);
            allBlocParents[i].transform.position = new Vector3(allBlocParents[i].transform.position.x, randomLocation, allBlocParents[i].transform.position.z);
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
        for (int i = 0; i < playersSpawnPoints.Count; i++)
        {
            //Spawn player at specific location
            if (playersSpawnPoints[i].gameObject.TryGetComponent(out Node playerNodeSpawnPoint))
            {
                Vector3 spawnPos = playerNodeSpawnPoint.GetWalkPoint() + new Vector3(0, 0.5f, 0);
                PlayerStateManager player = Instantiate(playerPref, spawnPos, Quaternion.identity);
                player.playerRespawnPoint = playerNodeSpawnPoint.gameObject.transform;
                player.currentBlocPlayerOn = playersSpawnPoints[i].transform;
                player.gameObject.name = "Player " + (i + 1);
                player.playerNumber = i;
                players.Add(player);
            }
        }
        SetUpPlayers();
      
    }

  private void SetPlayerTeam(PlayerStateManager player, Player.PlayerTeam playerTeam, Material playerCustomMat)
  {
      if (player.playerRespawnPoint.TryGetComponent(out Node playerNodeSpawnPoint))
      {
          player.playerTeam = playerTeam;
          SetSpriteSpawnPlayerPoint(player, playerNodeSpawnPoint, playerCustomMat);
          player.indicatorPlayerRenderer.gameObject.SetActive(false);
          player.playerMesh.material = playerCustomMat;
      }
  }

  private void SetSpriteSpawnPlayerPoint(PlayerStateManager player,Node playerNodeSpawnPoint, Material playerCustomMat)
  {
      //TODO Couleurs list a update directement dans le playerData quand on selectionne sa team
      Vector3 spawnPos = playerNodeSpawnPoint.GetWalkPoint();
          
      GameObject spriteSpawnPoint = Instantiate(spawnPointSpriteParent, spawnPos + spawnPointSpriteParent.transform.position, spawnPointSpriteParent.transform.rotation, playerNodeSpawnPoint.gameObject.transform);
      SpriteRenderer playerSprite = null;
      
      if (spriteSpawnPoint.transform.GetChild(0).TryGetComponent(out SpriteRenderer sprite))
      {
          playerSprite = sprite;
      }

      if (playerSprite != null)
      {
          switch (playerCustomMat.name)
          {
              case "M_blue_player": player.indicatorPlayerRenderer.material.color = playerColors[0]; 
                  playerSprite.color = playerColors[0];
                  player.playerColor = playerColors[0];
                  break;
              case "M_red_player": player.indicatorPlayerRenderer.material.color = playerColors[2]; 
                  playerSprite.color = playerColors[2]; 
                  player.playerColor = playerColors[2];
                  break;
              case "M_yellow_player": player.indicatorPlayerRenderer.material.color = playerColors[3]; 
                  playerSprite.color = playerColors[3]; 
                  player.playerColor = playerColors[3];break;
              case "M_green_player": player.indicatorPlayerRenderer.material.color = playerColors[1];  
                  playerSprite.color = playerColors[1]; 
                  player.playerColor = playerColors[1];break;
          }
      }
  }
  
    void SetUpPlayers()
    {
        SetPlayerTeam(players[0], Player.PlayerTeam.TeamOne, colors[playerData.P1colorID] );
        Instantiate(hats[playerData.P1hatID], players[0].playerHat.transform.position, players[0].playerHat.transform.rotation).transform.parent = players[0].playerHat.transform;
        
        SetPlayerTeam(players[1], Player.PlayerTeam.TeamTwo, colors[playerData.P2colorID]);
        Instantiate(hats[playerData.P2hatID], players[1].playerHat.transform.position, players[1].playerHat.transform.rotation).transform.parent = players[1].playerHat.transform; ;

        SetPlayerTeam(players[2], Player.PlayerTeam.TeamOne, colors[playerData.P3colorID] );
        Instantiate(hats[playerData.P3hatID], players[2].playerHat.transform.position, players[2].playerHat.transform.rotation).transform.parent = players[2].playerHat.transform; ;
        
        SetPlayerTeam(players[3], Player.PlayerTeam.TeamTwo, colors[playerData.P4colorID]);
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
            
            actualCamPreset = camPreSets[countTurn];

            var cameraTransform = _cam.transform;
            var target = Quaternion.Euler(actualCamPreset.camRot);

            //Smooth Transition
            cameraTransform.DOMove(actualCamPreset.camPos, smoothTransitionTime);
            cameraTransform.DORotateQuaternion(target, smoothTransitionTime);

            //UI SWITCH
            UiManager.Instance.SwitchUiForPlayer(actualCamPreset.sliderNextTurn);
            CameraButtonManager.Instance.SetUpUiCamPreset();
            
            //Register Previous Cam View Mode
            if (turnCount <= 4)
            {
                cameraViewModeGesture.SetUpCameraViewMode(true, 0);
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
        var camEulerAngles = _cam.transform.eulerAngles;
        var camPos = _cam.transform.position;
        
        var previousCamPreSets = previousCamPreSetsList[indexCam];

        previousCamPreSets.camRot = camEulerAngles;
        previousCamPreSets.camPos = new Vector3(Mathf.Round(camPos.x), Mathf.Round(camPos.y), Mathf.Round(camPos.z));
        previousCamPreSets.presetNumber = camPreSets[indexCam].presetNumber;
        previousCamPreSets.sliderNextTurn = camPreSets[indexCam].sliderNextTurn;

        previousCamPreSetsList[indexCam] = previousCamPreSets;
        
        camPreSets[indexCam] = previousCamPreSetsList[indexCam];
    }

    private void IncreaseDemiCycle()
    {
        if (conditionVictory.mapTheme == ConditionVictory.Theme.Volcano)
        {
            VolcanoManager.Instance.CyclePassed();
        }
        
    }

    public void ChangePlayerTurn(int playerNumberTurn)
    {
        if (playerNumberTurn == players[0].playerNumber || playerNumberTurn == players[2].playerNumber)
        {
            IncreaseDemiCycle();
        }

        if (conditionVictory.mapTheme == ConditionVictory.Theme.Mountain)
        {
            if (playerNumberTurn == players[3].playerNumber)
            {
                MountainManager.Instance.ChangeCycle();
            }
            MountainManager.Instance.ChangeTurn();
        } else if (conditionVictory.mapTheme == ConditionVictory.Theme.Volcano)
        {
            VolcanoManager.Instance.ChangeTurnVolcano();
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
        Debug.Log(currentPlayerTurn);
        NFCManager.Instance.PlayerChangeTurn();

        if (UiManager.Instance.textActionPointPopUp)
        {
            UiManager.Instance.textActionPointPopUp.SetActive(false);
            UiManager.Instance.textActionPointPopUp = null;
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
            int randomNumberEndSpawnPoint = Random.Range(0, conditionVictory.endZoneSpawnPoints.Count);
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
        
        
    }

    public void PlayerMoving()
    {
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