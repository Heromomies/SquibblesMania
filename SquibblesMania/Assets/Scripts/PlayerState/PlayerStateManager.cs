using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    
    private PlayerBaseState currentState;

    public PlayerActionCardState PlayerActionCardState = new PlayerActionCardState();
    public PlayerCardState PlayerCardState = new PlayerCardState();
    public PlayerPowerCardState PlayerPowerCardState = new PlayerPowerCardState();
    
    // Start is called before the first frame update
    void Start()
    {
        currentState = PlayerCardState;
        
        currentState.EnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  public  void SwitchState(PlayerBaseState state)
    {
        currentState.ExitState(this);
        //Switch current state to the new "state"
        currentState = state;
        
        state.EnterState(this);
    }
}
