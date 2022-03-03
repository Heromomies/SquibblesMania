using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DigitalRubyShared;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    //Manager for simple button Ui
    [Header("MANAGER UI")] public TextMeshProUGUI currentActionPointsOfCurrentPlayerTurn;
    
    private static UiManager _uiManager;
    public GameObject buttonNextTurn;

    [Header("WIN PANEL")] public GameObject winPanel;
    public TextMeshProUGUI winText;
    public static UiManager Instance => _uiManager;

    [SerializeField] [Header("CAM SWITCH PARAMETERS")]
    private bool isSwitchChanged;
    

    private void Awake()
    {
        _uiManager = this;
    }

    public void SwitchUiForPlayer(GameObject buttonNextTurnPlayer, TextMeshProUGUI currentActionPoint)
    {
        buttonNextTurn = buttonNextTurnPlayer;
        currentActionPointsOfCurrentPlayerTurn = currentActionPoint;
        SetUpCurrentActionPointOfCurrentPlayer(GameManager.Instance.currentPlayerTurn.playerActionPoint);
    }

    public void ButtonPathFindingBlock()
    {
        GameManager.Instance.currentPlayerTurn.currentTouchBlock = TouchManager.Instance.Hit.transform;
        GameManager.Instance.currentPlayerTurn.StartPathFinding();
        switch (GameManager.Instance.actualCamPreset.presetNumber)
        {
            case 1: TouchManager.Instance.uiInteraction[0].uiInteractionParentObject.SetActive(false); break;
            case 2: TouchManager.Instance.uiInteraction[0].uiInteractionParentObject.SetActive(false); break;
            case 3: TouchManager.Instance.uiInteraction[1].uiInteractionParentObject.SetActive(false); break;
            case 4: TouchManager.Instance.uiInteraction[1].uiInteractionParentObject.SetActive(false); break;
        }
    }

    public void ButtonUpDownBlock()
    {
        switch (GameManager.Instance.actualCamPreset.presetNumber)
        {
            case 1: 
                TouchManager.Instance.uiInteraction[0].uiInteractionParentObject.SetActive(false);
                MovementBlockManager.Instance.isMovingBlock = true; break;
            case 2: 
                TouchManager.Instance.uiInteraction[0].uiInteractionParentObject.SetActive(false);
                MovementBlockManager.Instance.isMovingBlock = true; break;
            case 3:  
                TouchManager.Instance.uiInteraction[1].uiInteractionParentObject.SetActive(false);
                MovementBlockManager.Instance.isMovingBlock = true; break;
            case 4: 
                TouchManager.Instance.uiInteraction[1].uiInteractionParentObject.SetActive(false);
                MovementBlockManager.Instance.isMovingBlock = true; break;
        }

    }

    public void SetUpCurrentActionPointOfCurrentPlayer(int actionPointText)
    {
        var localManager = currentActionPointsOfCurrentPlayerTurn.GetComponent<LocalizationParamsManager>();
        localManager.SetParameterValue("ACTIONPOINT", actionPointText.ToString());
    }


    public void ButtonNextTurn()
    {
        NFCManager.Instance.numberOfTheCard = 0;
        GameManager.Instance.currentPlayerTurn.CurrentState.ExitState(GameManager.Instance.currentPlayerTurn);
    }

    public void LoadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
    
    public void WinSetUp(Player.PlayerTeam playerTeam)
    {
        winPanel.SetActive(true);
        winText.text = $"{playerTeam} WIN";
    }

    public void ButtonChangeCamMoveUi()
    {
        FingersPanOrbitComponentScript cameraTouchMovement = Camera.main.gameObject.GetComponent<FingersPanOrbitComponentScript>();
        
        if (!CameraButtonManager.Instance.enabled)
        {
            CameraButtonManager.Instance.enabled = true;
            cameraTouchMovement.enabled = false;
            CameraButtonManager.Instance.TopViewMode();
            GameManager.Instance.ResetCamVars();
        }
        else
        {
            CameraButtonManager.Instance.enabled = false;
            cameraTouchMovement.enabled = true;
            CameraButtonManager.Instance.BaseViewMode();
            GameManager.Instance.ResetCamVars();
        }
    }
    
}