using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Hello");
        player.SwitchState(player.PlayerActionCardState);
    }

    public override void UpdtateState(PlayerStateManager player)
    {
        
    }

    public override void ExitState(PlayerStateManager player)
    {
       Debug.Log("my");
    }


}
