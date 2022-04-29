using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DigitalRubyShared;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    //Manager for simple button Ui
    [Header("MANAGER UI")]
    private static UiManager _uiManager;
    public GameObject buttonNextTurn;
   
    
    [Header("WIN PANEL")] public GameObject winPanel;
    public GameObject textTeamOne, textTeamTwo;
    public static UiManager Instance => _uiManager;

    [Header("CARD UI VFX")]
    public Transform[] parentSpawnCardUiVFX;
    
    [Header("POP UP TEXT PARAMETERS")]
    public GameObject textActionPointPopUp;
    public int totalCurrentActionPoint;
    [SerializeField] private Vector3 offsetText;
    public Camera uiCam;

    [Header("STUN TEXT PARAMETERS")] [SerializeField]
    private GameObject[] playerStunTextParents;
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
        if (GameManager.Instance.currentPlayerTurn.isPlayerStun)
        {
            StunTextPopUp(GameManager.Instance.actualCamPreset.presetNumber, false);
        }
        
        GameManager.Instance.currentPlayerTurn.canSwitch = true;
        GameManager.Instance.currentPlayerTurn.CurrentState.ExitState(GameManager.Instance.currentPlayerTurn);

    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        textTeamOne.SetActive(false);
        textTeamTwo.SetActive(false);
    }


    public void StunTextPopUp(int actualCamPresetNumber, bool setActiveGameObject)
    {
        Debug.Log("Hello world");
       
        if (actualCamPresetNumber <= 2)
        {
            //TODO Set active du Stun text Team 1
            GameObject playerStunTextParent = playerStunTextParents[1];
            playerStunTextParent.SetActive(setActiveGameObject);
            Transform spriteArrow;
            switch (GameManager.Instance.currentPlayerTurn.playerNumber)
            {
                case 0: spriteArrow = playerStunTextParent.transform.GetChild(0);
                        spriteArrow.gameObject.SetActive(setActiveGameObject);
                        break;
                case 2: spriteArrow = playerStunTextParent.transform.GetChild(1); 
                        spriteArrow.gameObject.SetActive(setActiveGameObject);
                        break;
            }
        }
        else
        {
            //TODO Set active du Stun text Team 2
            GameObject playerStunTextParent = playerStunTextParents[0];
            playerStunTextParent.SetActive(setActiveGameObject);
            Transform spriteArrow;
            switch (GameManager.Instance.currentPlayerTurn.playerNumber)
            {
                case 1: spriteArrow = playerStunTextParent.transform.GetChild(0);
                    spriteArrow.gameObject.SetActive(setActiveGameObject);
                    break;
                case 3: spriteArrow = playerStunTextParent.transform.GetChild(1); 
                    spriteArrow.gameObject.SetActive(setActiveGameObject);
                    break;
            }
        }
    }
    
    public void WinSetUp(Player.PlayerTeam playerTeam)
    {
        winPanel.SetActive(true);
        if (playerTeam == Player.PlayerTeam.TeamOne)
        {
            textTeamOne.SetActive(true);
        }
        else
        {
            textTeamTwo.SetActive(true);
        }
    }
    
    #region SpawnTextActionPoint

    public void SpawnTextActionPointPopUp(Transform currentPlayer)
    {
        totalCurrentActionPoint = GameManager.Instance.currentPlayerTurn.playerActionPoint;
        if (!textActionPointPopUp)
        {
            textActionPointPopUp = PoolManager.Instance.SpawnObjectFromPool("PopUpTextActionPoint", currentPlayer.position + offsetText, Quaternion.identity, currentPlayer);
        }
        else
        {
            textActionPointPopUp.SetActive(true);
        }
        
        textActionPointPopUp.GetComponent<PopUpTextActionPoint>().SetUpText(GameManager.Instance.currentPlayerTurn.playerActionPoint);
    }

    #endregion
    
}