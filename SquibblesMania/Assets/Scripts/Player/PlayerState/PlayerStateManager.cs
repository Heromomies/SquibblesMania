using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerStateManager : Player
{
    public PlayerBaseState CurrentState;

    public PlayerActionPointCardState PlayerActionPointCardState = new PlayerActionPointCardState();
    public PlayerCardState PlayerCardState = new PlayerCardState();
    public PlayerPowerCardState PlayerPowerCardState = new PlayerPowerCardState();

    [Header("PLAYER BLOCKS VIEW")] public Transform currentBlockPlayerOn;
    public Transform currentTouchBlock;
    public List<Transform> finalPathFinding = new List<Transform>();
    public bool walking;
    public float timeMoveSpeed;
    public GameObject particle;

    [Header("PLAYER UTILITIES")] public int playerNumber;
    public bool isPlayerInActionCardState;
    public List<Transform> nextBlockPath;
    
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
        if (CurrentState != null)
        {
            CurrentState.UpdateState(this);
            DetectBlockBelowPlayer();
        }
    }

    public void StartState()
    {
        //Start the player turn in the player card state
        CurrentState = PlayerCardState;
        CurrentState.EnterState(this);
    }

    
    
    public void StartPathFinding()
    {
        //If the current state of the player is when he use his action point
        if (CurrentState == PlayerActionPointCardState)
        {
            PlayerActionPointCardState.FindPath(this);
        }
    }


    public void SwitchState(PlayerBaseState state)
    {
        if (CurrentState != null)
        {
            CurrentState.ExitState(this);
            //Switch current state to the new "state"
            CurrentState = state;

            state.EnterState(this);
        }
        
    }

    private void DetectBlockBelowPlayer()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        Debug.DrawRay(transform.position, -transform.up, Color.red);

        if (Physics.Raycast(ray, out hit, 1.1f))
        {
            if (hit.collider.gameObject.GetComponent<Node>() != null)
            {
                currentBlockPlayerOn = hit.transform;
            }
        }
        else
        {
           //  StartCoroutine(WaitUntilRespawn());
        }
    }

    private IEnumerator WaitUntilRespawn()
    {
        yield return new WaitForSeconds(1f);
        
        var blockPlayerOn = GameManager.Instance.currentPlayerTurn.currentBlockPlayerOn.gameObject;
        var obj =  blockPlayerOn.GetComponentInParent<GroupBlockDetection>();
        var children = obj.GetComponentsInChildren<Node>();

        var randomNumber =Random.Range(0,children.Length);
        
        GameManager.Instance.currentPlayerTurn.transform.position = children[randomNumber].transform.position + new Vector3(0,1,0);
        StopAllCoroutines();
    }

    public void StunPlayer(PlayerStateManager player, int stunTurnCount)
    {
        player.isPlayerStun = true;
        player.stunCount = stunTurnCount;

        if (GameManager.Instance.currentPlayerTurn == player)
        {
            //Exit current state of current player
            player.CurrentState.ExitState(player);
        }
    }
}