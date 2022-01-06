using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    //Manager for simple button Ui
    public TextMeshProUGUI currentActionPointsOfCurrentPlayerTurn;

    private static UiManager _uiManager;

    public static UiManager Instance => _uiManager;

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
        
    }

    public void SetUpCurrentActionPointOfCurrentPlayer(int actionPointText)
    {
        currentActionPointsOfCurrentPlayerTurn.text = $"Action point : {actionPointText}";
    }
}