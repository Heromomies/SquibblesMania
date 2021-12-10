using UnityEngine;

public abstract class EventBaseState
{
    public abstract void EnterState(EventStateManager eventState);

    public abstract void UpdateState(EventStateManager eventState);

    public abstract void OnCollisionEnter(EventStateManager eventState);
}
