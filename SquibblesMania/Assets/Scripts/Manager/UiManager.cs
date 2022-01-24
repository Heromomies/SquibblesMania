using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    //Manager for simple button Ui
    [Header("MANAGER UI")]
    public TextMeshProUGUI currentActionPointsOfCurrentPlayerTurn;

    public TextMeshProUGUI turnCountText;
    private static UiManager _uiManager;
    public GameObject buttonNextTurn;
    public GameObject conditionEvent, conditionInventory;
    
    [Header("WIN PANEL")] public GameObject winPanel;
    public TextMeshProUGUI winText;
    public static UiManager Instance => _uiManager;
  
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
        TouchManager.Instance.uiInteractionParentObject.SetActive(false);
    }

    public void ButtonUpDownBlock()
    {
        TouchManager.Instance.uiInteractionParentObject.SetActive(false);
        MovementBlockManager.Instance.buttonMoveBlockParentObject.SetActive(true);
        MovementBlockManager.Instance.isMovingBlock = true;
    }

    public void SetUpCurrentActionPointOfCurrentPlayer(int actionPointText)
    {
        currentActionPointsOfCurrentPlayerTurn.text = $"Action point : {actionPointText}";
    }

    public void UpdateCurrentTurnCount(int turnCount)
    {
        turnCountText.text = $"Round number : {turnCount}";
    }
    public void ButtonNextTurn()
    {
        GameManager.Instance.currentPlayerTurn.CurrentState.ExitState(GameManager.Instance.currentPlayerTurn);
    }

    public void Restart(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        
    }

    public void MenuStart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
    public void SeeCondition()
    {
        if (!conditionEvent.activeSelf)
        {
            conditionEvent.SetActive(true);
            conditionInventory.SetActive(true);
        }
        else
        {
            conditionEvent.SetActive(false);
            conditionInventory.SetActive(false);
        }
           
    }
    public void WinSetUp(Player.PlayerTeam playerTeam)
    {
        winPanel.SetActive(true);
        winText.text = $"{playerTeam} WIN";
    }
}