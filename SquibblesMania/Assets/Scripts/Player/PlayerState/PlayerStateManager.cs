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
	public float timeRotateSpeed;

	[Header("PLAYER UTILITIES")] public int playerNumber;
	public bool isPlayerInActionCardState;
	public List<Transform> nextBlockPath;
	
	public PlayerMovementManager playerMovementManager;
	
	public GameObject psStun;

	private void Start()
	{
		//Assign the player to a list for know on what block group is currently on
		GameManager.Instance.DetectParentBelowPlayers();
		
		playerMovementManager = PlayerMovementManager.Instance;
	}


	// Update is called once per frame
	void Update()
	{
		if (CurrentState != null)
		{
			CurrentState.UpdateState(this);
			//GameManager.Instance.DetectParentBelowPlayers();
		}
	}

	public void StartState()
	{
		//Start the player turn in the player card state
		CurrentState = PlayerCardState;
		CurrentState.EnterState(this);
	}


	public void StartPreviewPathFinding()
	{
		//If the current state of the player is when he use his action point
		if (CurrentState == PlayerActionPointCardState)
		{
			PlayerActionPointCardState.FindPath(this);
		}
	}

	public void StartPlayerMovement()
	{
		if (!walking)
		{
			walking = true;
			StartCoroutine(PlayerActionPointCardState.BeginFollowPath(this));
		}
	}

	public void ResetPreviewPathFinding()
	{
		if (CurrentState == PlayerActionPointCardState)
		{
			PlayerActionPointCardState.ResetPreviewPath(this);
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

	public void StunPlayer(PlayerStateManager player, int stunTurnCount)
	{
		player.isPlayerStun = true;
		player.stunCount = stunTurnCount;
	}
}