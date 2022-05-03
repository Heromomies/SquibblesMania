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
    [HideInInspector]
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

    [Header("STUN TEXT PARAMETERS")]
    [SerializeField] private UiPlayerStun[] uiPlayerStuns;
    
    [Serializable]
    public struct UiPlayerStun
    {
        public GameObject playerStunTextParent;
        public Transform[] arrowSprite;
    }
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
            GameManager.Instance.currentPlayerTurn.vfxStun.SetActive(false);
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
        if (actualCamPresetNumber <= 2)
        {
            uiPlayerStuns[0].playerStunTextParent.SetActive(setActiveGameObject);
            Transform spriteArrow;
            switch (GameManager.Instance.currentPlayerTurn.playerNumber)
            {
                case 0: spriteArrow = uiPlayerStuns[0].arrowSprite[GameManager.Instance.currentPlayerTurn.playerNumber];
                        spriteArrow.gameObject.SetActive(setActiveGameObject); break;
                case 2: spriteArrow = uiPlayerStuns[0].arrowSprite[GameManager.Instance.currentPlayerTurn.playerNumber]; 
                        spriteArrow.gameObject.SetActive(setActiveGameObject); break;
            }
        }
        else
        {
            uiPlayerStuns[1].playerStunTextParent.SetActive(setActiveGameObject);
            Transform spriteArrow;
            switch (GameManager.Instance.currentPlayerTurn.playerNumber)
            {
                case 1: spriteArrow = uiPlayerStuns[1].arrowSprite[GameManager.Instance.currentPlayerTurn.playerNumber]; 
                    spriteArrow.gameObject.SetActive(setActiveGameObject); break;
                case 3: spriteArrow = uiPlayerStuns[1].arrowSprite[GameManager.Instance.currentPlayerTurn.playerNumber];
                    spriteArrow.gameObject.SetActive(setActiveGameObject); break;
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