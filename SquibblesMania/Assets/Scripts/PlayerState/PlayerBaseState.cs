using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState
{
    public abstract void EnterState(PlayerStateManager player);

    public abstract void UpdtateState(PlayerStateManager player);

    public abstract void ExitState(PlayerStateManager player);

  
}