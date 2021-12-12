using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionCardState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Friend !");
    }

    public override void UpdtateState(PlayerStateManager player)
    {
        throw new System.NotImplementedException();
    }

    public override void ExitState(PlayerStateManager player)
    {
        throw new System.NotImplementedException();
    }


}
