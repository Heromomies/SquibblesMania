using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPowerCardState : PlayerBaseState
{
    //The state when player use his card power
    public override void EnterState(PlayerStateManager player)
    {
        
    }

    public override void UpdtateState(PlayerStateManager player)
    {
        throw new System.NotImplementedException();
    }

    public override void ExitState(PlayerStateManager player)
    {
        //Switch to next player of another team to play
        switch (player.playerNumber)
        {
            case 0:
                GameManager.Instance.players[1].StartState();
                break;
            case 1:
                GameManager.Instance.players[2].StartState();
                break;
            case 2:
                GameManager.Instance.players[3].StartState();
                break;
            case 3:
                GameManager.Instance.players[0].StartState();
                break;
        }
    }
}