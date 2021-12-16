using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerStateManager : Player
{
    private PlayerBaseState currentState;

    public PlayerActionPointCardState PlayerActionPointCardState = new PlayerActionPointCardState();
    public PlayerCardState PlayerCardState = new PlayerCardState();
    public PlayerPowerCardState PlayerPowerCardState = new PlayerPowerCardState();

    public Transform currentBlockPlayerOn, currentTouchBlock;
    public List<Transform> finalPathFinding = new List<Transform>();
    public bool walking;
    public float timeMoveSpeed;

    public int playerNumber;
    public bool isPlayerInActionCardState;

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
        
        if (currentState != null)
        {
            currentState.UpdtateState(this);
            DetectBlockBelowPlayer();
        }
    }

    public void StartState()
    {
        //Start the player turn in the player card state
        currentState = PlayerCardState;
        currentState.EnterState(this);
    }
    public void StartPathFinding()
    {
        //If the current state of the player is when he use his action point
        if (currentState == PlayerActionPointCardState)
        {
            finalPathFinding.Clear();
            PlayerActionPointCardState.FindPath(this);
        }
        
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