using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventStateManager : MonoBehaviour
{
    private EventBaseState _currentState;

    private EruptionState _eruptionState = new EruptionState();
    private SmokeAsheState _smokeAsheState= new SmokeAsheState();
    private RiverState _riverState= new RiverState();

    // Start is called before the first frame update
    void Start()
    {
        _currentState = _eruptionState;
        
        _currentState.EnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        _currentState.UpdateState(this);
    }
}
