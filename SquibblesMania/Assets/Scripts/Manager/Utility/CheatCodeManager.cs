using System.Collections;
using System.Collections.Generic;
using UnityEngine;




#if UNITY_EDITOR
public class CheatCodeManager : MonoBehaviour
{
    [SerializeField] private int actionPointToAdd;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            //Add action point and switch to action point state the current player
            GameManager.Instance.currentPlayerTurn.playerActionPoint = actionPointToAdd;
            GameManager.Instance.currentPlayerTurn.CurrentState = GameManager.Instance.currentPlayerTurn.PlayerActionPointCardState;
            GameManager.Instance.currentPlayerTurn.CurrentState.EnterState(GameManager.Instance.currentPlayerTurn);
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            GameManager.Instance.turnCount++;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            TeamInventoryManager.Instance.AddResourcesToInventory(1, GameManager.Instance.currentPlayerTurn.playerTeam);
        }
    }
}
#endif
