using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DigitalRubyShared;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwapPower : MonoBehaviour, IManagePower
{
	[Header("POWER SETTINGS")]
	public int range;
	public LayerMask layer;
	[Space (10)] 
	public List<TextMeshProUGUI> textWhenThereAreNoZombieAround;
	[Space (10)]
	public List<TextMeshProUGUI> textSelectPlayer;
	[Space (10)] 
	[Header("MATERIALS")]
	public Material matToChange;

	private GameObject _playerToSwap;
	private GameObject _particleToDeactivatePlayerOne, _particleToDeactivatePlayerTwo;
	private Vector3 _pos;
	private Collider _playerOne, _playerTwo;
	private Camera _cam;
	[HideInInspector] public Collider[] players;
	private readonly List<RaycastResult> raycast = new List<RaycastResult>();
	public PanGestureRecognizer SwapTouchGesture { get; private set; }

	private void Awake()
	{
		_cam = Camera.main;
	}

	private void OnEnable()
	{
		SwapTouchGesture = new PanGestureRecognizer();
		SwapTouchGesture.ThresholdUnits = 0.0f; // start right away
		//Add new gesture
		SwapTouchGesture.StateUpdated += PlayerTouchGestureUpdated;
		SwapTouchGesture.AllowSimultaneousExecutionWithAllGestures();

		FingersScript.Instance.AddGesture(SwapTouchGesture);

		_playerToSwap = GameManager.Instance.currentPlayerTurn.gameObject;
		transform.position = _playerToSwap.transform.position;

		if (_particleToDeactivatePlayerOne != null)
		{
			_particleToDeactivatePlayerOne.SetActive(false);
			_particleToDeactivatePlayerOne = null;
		}

		if (_particleToDeactivatePlayerOne != null)
		{
			_particleToDeactivatePlayerTwo.SetActive(false);
			_particleToDeactivatePlayerTwo = null;
		}
		
		
		DisplayPower();
	}

	public void DisplayPower() // Show the sphere and admit the player to chose the other player to swap
	{
		// ReSharper disable once Unity.PreferNonAllocApi
		players = Physics.OverlapSphere(transform.position, range, layer);

		_playerOne = GameManager.Instance.currentPlayerTurn.gameObject.GetComponent<Collider>();

		for (int i = 0; i < players.Length; i++)
		{
			if (players[i].name != _playerOne.name && players.Length > 1)
			{
				players[i].GetComponent<PlayerStateManager>().meshRenderer.GetComponent<Renderer>().material = matToChange;
			}
		}

		if (players.Length > 1)
		{
			switch (GameManager.Instance.actualCamPreset.presetNumber)
			{
				case 1: textSelectPlayer[0].gameObject.SetActive(true); break;
				case 2: textSelectPlayer[0].gameObject.SetActive(true); break;
				case 3: textSelectPlayer[1].gameObject.SetActive(true); break;
				case 4: textSelectPlayer[1].gameObject.SetActive(true); break;
			}
		}
		else
		{
			switch (GameManager.Instance.actualCamPreset.presetNumber)
			{
				case 1: textWhenThereAreNoZombieAround[0].gameObject.SetActive(true); break;
				case 2: textWhenThereAreNoZombieAround[0].gameObject.SetActive(true); break;
				case 3: textWhenThereAreNoZombieAround[1].gameObject.SetActive(true); break;
				case 4: textWhenThereAreNoZombieAround[1].gameObject.SetActive(true); break;
			}
		}
	}

	private void PlayerTouchGestureUpdated(GestureRecognizer gesture)
	{
		if (gesture.State == GestureRecognizerState.Began)
		{
			PointerEventData p = new PointerEventData(EventSystem.current);
			p.position = new Vector2(gesture.FocusX, gesture.FocusY);

			raycast.Clear();
			EventSystem.current.RaycastAll(p, raycast);

			Ray ray = _cam.ScreenPointToRay(p.position);

			if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, layer))
			{
				if (players.ToList().Contains(hitInfo.collider) && hitInfo.collider.name != GameManager.Instance.name)
				{
					NFCManager.Instance.powerActivated = true;
					
					_playerTwo = hitInfo.collider;
					DoPower();
				}
			}
			else
			{
				gesture.Reset();
			}
		}
	}

	public void DoPower()
	{
		var transformPlayerOne = _playerOne.transform;
		_pos = transformPlayerOne.position;

		_particleToDeactivatePlayerOne = PoolManager.Instance.SpawnObjectFromPool("ParticleSwap", _pos, Quaternion.identity, null);
		_particleToDeactivatePlayerTwo = PoolManager.Instance.SpawnObjectFromPool("ParticleSwap", _playerTwo.transform.position, Quaternion.identity, null);
		
		SwapPosition(transformPlayerOne, _playerTwo.transform);
	}

	private void SwapPosition(Transform playerOne, Transform playerTwo) // Swap the position between the two players
	{
		foreach (var playerCol in players)
		{
			GameObject player = playerCol.gameObject;
			player.GetComponent<PlayerStateManager>().currentBlockPlayerOn.GetComponent<Node>().groupBlockParent.AddOrRemovePlayerFromList(true, player.transform);
		}
		
		playerOne.position = playerTwo.position;
		playerTwo.position = _pos;
		
		ClearPower();
	}
	
	public void ClearPower()
	{
		
		SwapTouchGesture.StateUpdated -= PlayerTouchGestureUpdated;

		foreach (var g in textWhenThereAreNoZombieAround)
		{
			g.gameObject.SetActive(false);
		}
		foreach (var g in textSelectPlayer)
		{
			g.gameObject.SetActive(false);
		}
		
		for (int i = 0; i < players.Length; i++)
		{
			GameManager.Instance.SetUpMaterial(players[i].GetComponent<PlayerStateManager>(), players[i].GetComponent<PlayerStateManager>().playerNumber);
		}

		PowerManager.Instance.ActivateDeactivatePower(0, false);
		PowerManager.Instance.ChangeTurnPlayer();
			
		//Set up player groupblockParent list 
	
		if (_playerTwo != null && _playerOne != null)
		{
			PlayerStateManager playerOneSwap =_playerOne.gameObject.GetComponent<PlayerStateManager>();
			PlayerStateManager playerTwoSwap = _playerTwo.gameObject.GetComponent<PlayerStateManager>();
			
			playerOneSwap.currentBlockPlayerOn.GetComponent<Node>().groupBlockParent.AddOrRemovePlayerFromList(false, playerTwoSwap.transform);
			playerTwoSwap.currentBlockPlayerOn.GetComponent<Node>().groupBlockParent.AddOrRemovePlayerFromList(false, playerOneSwap.transform);
		}
	}
	
	private void OnDisable()
	{
		if (FingersScript.HasInstance)
		{
			FingersScript.Instance.RemoveGesture(SwapTouchGesture);
		}
	}
}