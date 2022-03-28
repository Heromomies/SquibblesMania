using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatCodeManager : MonoBehaviour
{

    [SerializeField] private int actionPointToAdd;

    [SerializeField] private bool isActivated = true;
   

    // Update is called once per frame
    void Update()
    {
        if (isActivated)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                //Add action point and switch to action point state the current player
                GameManager.Instance.currentPlayerTurn.playerActionPoint = actionPointToAdd;
               // UiManager.Instance.SetUpCurrentActionPointOfCurrentPlayer(GameManager.Instance.currentPlayerTurn.playerActionPoint);
                GameManager.Instance.currentPlayerTurn.SwitchState(GameManager.Instance.currentPlayerTurn.PlayerActionPointCardState);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            GameManager.Instance.turnCount++;
        }
    }
}
