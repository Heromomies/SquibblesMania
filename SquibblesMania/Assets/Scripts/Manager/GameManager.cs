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

    [Header("PLAYER CUSTOMIZATION")]
    public PlayerData playerData;
    public List<GameObject> hats = new List<GameObject>();
    public List<Material> colors = new List<Material>();

    private void Awake()
    {
        Application.targetFrameRate = 30;
        _gameManager = this;
        _cam = Camera.main;
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
        Instantiate(hats[playerData.P1hatID], players[0].hat.transform.position, players[0].hat.transform.rotation).transform.parent = players[0].hat.transform;
        players[0].meshRenderer.GetComponent<Renderer>().material = colors[playerData.P1colorID];
        

        players[1].playerTeam = Player.PlayerTeam.TeamTwo;
        players[1].gameObject.GetComponentInChildren<Renderer>().material.color = Color.blue;
        players[1].indicatorPlayer.SetActive(false);
        Instantiate(hats[playerData.P2hatID], players[1].hat.transform.position, players[1].hat.transform.rotation).transform.parent = players[1].hat.transform; ;
        players[1].meshRenderer.GetComponent<Renderer>().material = colors[playerData.P2colorID];

        players[2].playerTeam = Player.PlayerTeam.TeamOne;
        players[2].gameObject.GetComponentInChildren<Renderer>().material.color = Color.red;
        players[2].indicatorPlayer.SetActive(false);
        Instantiate(hats[playerData.P3hatID], players[2].hat.transform.position, players[2].hat.transform.rotation).transform.parent = players[2].hat.transform; ;
        players[2].meshRenderer.GetComponent<Renderer>().material = colors[playerData.P3colorID];

        players[3].playerTeam = Player.PlayerTeam.TeamTwo;
        players[3].gameObject.GetComponentInChildren<Renderer>().material.color = Color.blue;
        players[3].indicatorPlayer.SetActive(false);
        Instantiate(hats[playerData.P4hatID], players[3].hat.transform.position, players[3].hat.transform.rotation).transform.parent = players[3].hat.transform;
        players[3].meshRenderer.GetComponent<Renderer>().material = colors[playerData.P4colorID];
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

    public void SetUpMaterial(PlayerStateManager player, int playerNumber)
    {
        switch (playerNumber)
        {
            case 0 : player.meshRenderer.GetComponent<Renderer>().material = colors[playerData.P1colorID];
                break;
            case 1 : player.meshRenderer.GetComponent<Renderer>().material = colors[playerData.P2colorID];
                break;
            case 2 : player.meshRenderer.GetComponent<Renderer>().material = colors[playerData.P3colorID];
                break;
            case 3 : player.meshRenderer.GetComponent<Renderer>().material = colors[playerData.P4colorID];
                break; 
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
            
          
            Transform cameraTransform = _cam.transform;
            Quaternion target = Quaternion.Euler(camPreSets[countTurn].camRot);
            
            //Smooth Transition
            cameraTransform.DOMove(camPreSets[countTurn].camPos, smoothTransitionTime);
            cameraTransform.DORotateQuaternion(target, smoothTransitionTime);
        
            actualCamPreset = camPreSets[countTurn];
            
            //UI SWITCH
            UiManager.Instance.SwitchUiForPlayer(actualCamPreset.buttonNextTurn);
            CameraButtonManager.Instance.SetUpUiCamPreset();
            
            //Register Previous Cam View Mode
            if (turnCount <= 4)
            {
                _cameraViewModeGesture.SetUpCameraViewMode(true, 1);
            }
            else
            {
                _cameraViewModeGesture.SetUpCameraViewMode(false, count);
            }
            
            //count++;
           /* if (count >= camPreSets.Count)
            {
                count = 0;
            }*/

            count = (count + 1) % camPreSets.Count; 
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
        VolcanoManager.Instance.CyclePassed();
        PowerManager.Instance.CyclePassed();
    }

    public void ChangePlayerTurn(int playerNumberTurn)
    {
        if (playerNumberTurn == players[0].playerNumber || playerNumberTurn == players[2].playerNumber)
        {
            IncreaseDemiCycle();
            cycleCount++;
        }

        if (playerNumberTurn == players[3].playerNumber)
        {
            MoutainManager.Instance.ChangeCycle();
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
       
        SavePreviousCamRotY((int)Mathf.Repeat(count-1, previousCamPreSetsList.Count-1));
        _cameraViewModeGesture.SavePreviousViewModeGesture((int)Mathf.Repeat(count-1, previousCamPreSetsList.Count));
        CamConfig(count);
        
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

        if (count != 0)
        {
            currentPlayerTurn = players[count -1];
        }
        else if (count == 0)
        {
            currentPlayerTurn = players[3];
        }
        
        
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
     
        UiManager.Instance.WinSetUp(playerTeam);
    }

    private void OnApplicationQuit()
    {
        StopAllCoroutines();
    }
}