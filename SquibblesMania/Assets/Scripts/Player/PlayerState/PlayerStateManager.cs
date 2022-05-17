using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerStateManager : Player
{
	public PlayerBaseState CurrentState;

	public readonly PlayerActionPointCardState PlayerActionPointCardState = new PlayerActionPointCardState();
	private readonly PlayerCardState _playerCardState = new PlayerCardState();
	public readonly PlayerPowerCardState PlayerPowerCardState = new PlayerPowerCardState();

	[Header("PLAYER BLOCKS VIEW")] public Transform currentBlocPlayerOn;
	public Transform currentTouchBloc;
	public List<Transform> finalPathFinding = new List<Transform>();
	public bool walking;
	public float timeMoveSpeed;
	public float timeRotateSpeed;

	[Header("PLAYER UTILITIES")] public int playerNumber;
	public bool isPlayerInActionCardState;
	public List<Transform> nextBlockPath;
	
	public PlayerMovementManager playerMovementManager;


	private WaitForSeconds _waitForSecondsPlayerRespawn = new WaitForSeconds(0.5f);
	
	private void Start()
	{
		//Assign the player to a list for know on what block group is currently on
		playerMovementManager = PlayerMovementManager.Instance;
		
		GameManager.Instance.DetectParentBelowPlayers();
		PlayerStateEventManager.Instance.ONPlayerStunTriggerEnter += StunPlayer;
	}


	// Update is called once per frame
	void Update()
	{
		if (CurrentState != null)
		{
			CurrentState.UpdateState(this);
		}
		
	}

	public void StartState()
	{
		//Start the player turn in the player card state
		CurrentState = _playerCardState;
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

	public IEnumerator PlayerRespawnUpdateBlocBelow(PlayerStateManager player)
	{
		yield return _waitForSecondsPlayerRespawn;
		if (player.currentBlocPlayerOn.TryGetComponent(out Node currentPlayerNode))
		{
			currentPlayerNode.isActive = true;
			currentPlayerNode.GetComponentInParent<GroupBlockDetection>().playersOnGroupBlock.Remove(player.transform);
		}
           
		Ray ray = new Ray(player.transform.position, -transform.up);
		RaycastHit hit;
            
		if (Physics.Raycast(ray, out hit, 1.1f))
		{
			if (hit.collider.gameObject.TryGetComponent(out Node node))
			{
				player.currentBlocPlayerOn = hit.transform;
			}
		}
            
		player.currentBlocPlayerOn.GetComponent<Node>().isActive = false;
            
		GroupBlockDetection groupBlockDetection = player.currentBlocPlayerOn.transform.GetComponentInParent<GroupBlockDetection>();
		if (!groupBlockDetection.playersOnGroupBlock.Contains(player.transform))
		{
			groupBlockDetection.playersOnGroupBlock.Add(player.transform);
		}
		if (player.isPlayerInActionCardState)
		{
			player.CurrentState.EnterState(player);
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