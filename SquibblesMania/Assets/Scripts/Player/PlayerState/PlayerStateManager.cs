using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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
    
    private void Awake()
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
            finalPathFinding.Clear();

            PlayerActionPointCardState.FindPath(this);
        }
    }

    
    public void SwitchState(PlayerBaseState state)
    {
        CurrentState.ExitState(this);
        //Switch current state to the new "state"
        CurrentState = state;

        state.EnterState(this);
    }

    private void DetectBlockBelowPlayer()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        Debug.DrawRay(transform.position, -transform.up, Color.red);
          
        if (Physics.Raycast(ray, out hit, 1f))
        {
            if (hit.collider.gameObject.GetComponent<Node>() != null)
            {
                currentBlockPlayerOn = hit.transform;
            }
            
        }
        else
        {
           // StartCoroutine(WaitUntilRespawn());
        }
    }

    IEnumerator WaitUntilRespawn()
    {
        yield return new WaitForSeconds(1f);
        Vector3 p = GameManager.Instance.playersSpawnPoints[playerNumber].position;
        transform.position = new Vector3(p.x, p.y +1, p.z);
        GameObject a = Instantiate(particle, transform.position, Quaternion.identity);
        Destroy(a, 1f);
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