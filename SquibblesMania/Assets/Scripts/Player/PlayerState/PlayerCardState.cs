using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardState : PlayerBaseState
{
    //The state when player put card on Square one
    public override void EnterState(PlayerStateManager player)
    {
        //Turn of player x
        //Message player turn x "Put a card on the corresponding surface"

        if (player.isPlayerStun)
        {
            player.stunCount--;
            PlayerIsStun(player);
        }
        
        
        
        
    
    }

    void PlayerIsStun(PlayerStateManager player)
    {
        //If the stunCount is less than zero player is now not stun
        if (player.stunCount <= 0)
        {
            player.isPlayerStun = false;
        }
        
        switch (player.playerNumber)
        {
            case 0:
                GameManager.Instance.ChangePlayerTurn(1);

                break;
            case 1:
                GameManager.Instance.ChangePlayerTurn(2);

                break;
            case 2:
                GameManager.Instance.ChangePlayerTurn(3);

                break;
            case 3:
                GameManager.Instance.ChangePlayerTurn(0);
                break;
        }
    }

    public override void UpdateState(PlayerStateManager player)
    {
        // if player touch the power button
        if (Input.GetKeyDown(KeyCode.A))
        {
            player.SwitchState(player.PlayerPowerCardState);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            player.playerActionPoint = Random.Range(1, 6);
            UiManager.Instance.SetUpCurrentActionPointOfCurrentPlayer(player.playerActionPoint);
            player.isPlayerInActionCardState = true;
            UiManager.Instance.buttonNextTurn.SetActive(false);
            player.SwitchState(player.PlayerActionPointCardState);
        }
   
    }

    public override void ExitState(PlayerStateManager player)
    {
        
    }
}