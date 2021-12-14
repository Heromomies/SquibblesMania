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
        Debug.Log("Player put a card on the Square one");
        //Open panel with 2 button for the player
        
    }

    public override void UpdtateState(PlayerStateManager player)
    {
        //if player touch the action point button 
        player.SwitchState(player.PlayerActionCardState);
        //else if player touch the power button
        player.SwitchState(player.PlayerPowerCardState);
    }

    public override void ExitState(PlayerStateManager player)
    {
      
    }


}
