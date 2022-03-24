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
    [Header("MANAGER UI")]
    private static UiManager _uiManager;
    public GameObject buttonNextTurn;

    [Header("WIN PANEL")] public GameObject winPanel;
    public TextMeshProUGUI winText;
    public static UiManager Instance => _uiManager;

    [Header("CARD UI VFX")] public Transform[] parentSpawnCardUiVFX;
    public Camera uiCam;
    private void Awake()
    {
        _uiManager = this;
    }

    public void SwitchUiForPlayer(GameObject buttonNextTurnPlayer)
    {
        buttonNextTurn = buttonNextTurnPlayer;
    }




    public void ButtonNextTurn()
    {
        AudioManager.Instance.Play("ButtonNextTurn");
        
        NFCManager.Instance.numberOfTheCard = 0;
        NFCManager.Instance.displacementActivated = false;
        NFCManager.Instance.newCardDetected = false;
        NFCManager.Instance.powerActivated = false;
        GameManager.Instance.currentPlayerTurn.canSwitch = true;
        GameManager.Instance.currentPlayerTurn.CurrentState.ExitState(GameManager.Instance.currentPlayerTurn);
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
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
          
        }
        else
        {
            CameraButtonManager.Instance.enabled = false;
            cameraTouchMovement.enabled = true;
            CameraButtonManager.Instance.BaseViewMode();
        }
    }
    
}