using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerStateManager : Player
{
    private PlayerBaseState currentState;

    public PlayerActionCardState PlayerActionCardState = new PlayerActionCardState();
    public PlayerCardState PlayerCardState = new PlayerCardState();
    public PlayerPowerCardState PlayerPowerCardState = new PlayerPowerCardState();

    public Transform currentBlockPlayerOn, currentTouchBlock;
    public List<Transform> finalPathFinding = new List<Transform>();
    public bool walking;
    public float timeMoveSpeed;

    public int playerNumber;


    private void Start()
    {
        DetectBlockBelowPlayer();
        //Assign the player to a list for know on what block group is currently on
        GroupBlockDetection groupBlockDetection = currentBlockPlayerOn.GetComponent<Node>().groupBlockParent;
        groupBlockDetection.playersOnGroupBlock.Add(gameObject.transform);
    }

    // Update is called once per frame
    void Update()
    {
        DetectBlockBelowPlayer();
        Debug.Log(currentState);
        if (currentState != null)
        {
            currentState.UpdtateState(this);
        }
    }

    public void StartState()
    {
        currentState = PlayerCardState;
        currentState.EnterState(this);
    }

    public void SwitchState(PlayerBaseState state)
    {
        currentState.ExitState(this);
        //Switch current state to the new "state"
        currentState = state;

        state.EnterState(this);
    }

    private void DetectBlockBelowPlayer()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1f))
        {
            if (hit.collider.gameObject.GetComponent<Node>() != null)
            {
                currentBlockPlayerOn = hit.transform;
            }
        }
    }
}