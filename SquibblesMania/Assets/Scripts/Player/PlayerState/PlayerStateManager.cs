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
	public List<Transform> previewPath;
	public List<Transform> blocPreviewPath;
	public PlayerMovementManager playerMovementManager;
	
	public GameObject psStun;

	[HideInInspector] public bool isInJump;

	private float _timeLeft = 1.5f;

	private void Start()
	{
		DetectBlockBelowPlayer();
		Node currentNodePlayerOn = currentBlockPlayerOn.GetComponent<Node>();
		//Assign the player to a list for know on what block group is currently on
		GroupBlockDetection groupBlockDetection = currentNodePlayerOn.groupBlockParent;
		groupBlockDetection.playersOnGroupBlock.Add(gameObject.transform);
		currentNodePlayerOn.isActive = false;
		playerMovementManager = PlayerMovementManager.Instance;
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

	public void StartPreviewPathFinding()
	{
		if (CurrentState == PlayerActionPointCardState)
		{
			PlayerActionPointCardState.PathToGo(this);
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

		if (Physics.Raycast(ray, out hit, 1.1f))
		{
			if (hit.collider.gameObject.GetComponent<Node>() != null)
			{
				currentBlockPlayerOn = hit.transform;
				_timeLeft = 1.5f;
			}
		}
		else
		{
			_timeLeft -= Time.deltaTime;
			if (_timeLeft < 0)
			{
				var blockPlayerOn = GameManager.Instance.currentPlayerTurn.currentBlockPlayerOn.gameObject;
				var obj = blockPlayerOn.GetComponentInParent<GroupBlockDetection>();
				var children = obj.GetComponentsInChildren<Node>();

				var randomNumber = Random.Range(0, children.Length);

				GameManager.Instance.currentPlayerTurn.transform.position =
						children[randomNumber].transform.position + new Vector3(0, 1, 0);
				_timeLeft = 1.5f;
			}
			
		}
	}
	
	public void StunPlayer(PlayerStateManager player, int stunTurnCount)
	{
		player.isPlayerStun = true;
		player.stunCount = stunTurnCount;
	}
}