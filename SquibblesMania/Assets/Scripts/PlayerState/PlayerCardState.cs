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
        player.playerActionPoint = Random.Range(1, 6);
        UiManager.Instance.SetUpCurrentActionPointOfCurrentPlayer(player.playerActionPoint);
        player.isPlayerInActionCardState = true;
        UiManager.Instance.buttonNextTurn.SetActive(false);
        player.SwitchState(player.PlayerActionPointCardState);
    
    }

    public override void UpdtateState(PlayerStateManager player)
    {
        // if player touch the power button
        if (Input.GetKeyDown(KeyCode.A))
        {
            player.SwitchState(player.PlayerPowerCardState);
        }
        //if player touch the action point button 
        else if (Input.GetKeyDown(KeyCode.E))
        {
        }
    }

    public override void ExitState(PlayerStateManager player)
    {
    }
}