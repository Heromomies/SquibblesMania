using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    //Manager for simple button Ui

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
}