using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    //Manager for simple button Ui
    [Header("MANAGER UI")]
    public TextMeshProUGUI currentActionPointsOfCurrentPlayerTurn;
    
    private static UiManager _uiManager;

    public static UiManager Instance => _uiManager;
    public GameObject buttonNextTurn;
    private void Awake()
    {
        _uiManager = this;
       
    }
    

    public void ButtonPathFindingBlock()
    {
        
        GameManager.Instance.currentPlayerTurn.currentTouchBlock = TouchManager.Instance.hit.transform;

        GameManager.Instance.currentPlayerTurn.StartPathFinding();
        TouchManager.Instance.uiInteractionParentObject.SetActive(false);
    }

    public void ButtonUpDownBlock()
    {
        TouchManager.Instance.uiInteractionParentObject.SetActive(false);
        TouchManager.Instance.uiScaleBlockParentObject.SetActive(true);
        TouchManager.Instance.isMovingBlock = true;
    }

    public void SetUpCurrentActionPointOfCurrentPlayer(int actionPointText)
    {
        currentActionPointsOfCurrentPlayerTurn.text = $"Action point : {actionPointText}";
    }

    public void ButtonNextTurn()
    {
        GameManager.Instance.currentPlayerTurn.CurrentState.ExitState(GameManager.Instance.currentPlayerTurn);
    }
}