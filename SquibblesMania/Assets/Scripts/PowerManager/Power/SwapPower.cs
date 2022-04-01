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
	[Header("MATERIALS")] 
	public Material firstMat; 
	public Material secondMat;

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
		DisplayPower();
	}

	public void DisplayPower() // Show the sphere and admit the player to chose the other player to swap
	{
		// ReSharper disable once Unity.PreferNonAllocApi
		players = Physics.OverlapSphere(transform.position, range, layer);

		_playerOne = GameManager.Instance.currentPlayerTurn.gameObject.GetComponent<Collider>();

		for (int i = 0; i < players.Length; i++)
		{
			if (players[i].name != _playerOne.name)
			{
				Transform child = players[i].transform.GetChild(1);

				var color = child.GetComponentInChildren<Renderer>().material.color;
				color = secondMat.color;
				child.GetComponentInChildren<Renderer>().material.color = color;
			}
		}

		switch (players.Length)
		{
			case 1:
				switch (GameManager.Instance.actualCamPreset.presetNumber)
				{
					case 1: textWhenThereAreNoZombieAround[0].gameObject.SetActive(true); break;
					case 2: textWhenThereAreNoZombieAround[0].gameObject.SetActive(true); break;
					case 3: textWhenThereAreNoZombieAround[1].gameObject.SetActive(true); break;
					case 4: textWhenThereAreNoZombieAround[1].gameObject.SetActive(true); break;
				}
				break;
		}
	}

	public void CancelPower()
	{
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
				if (players.ToList().Contains(hitInfo.collider))
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
		playerOne.position = playerTwo.position;
		playerTwo.position = _pos;

		ClearPower();
	}
	
	public void ClearPower()
	{
		StartCoroutine(StartBeforeClearCoroutine());
		
		SwapTouchGesture.StateUpdated -= PlayerTouchGestureUpdated;

		foreach (var g in textWhenThereAreNoZombieAround)
		{
			g.gameObject.SetActive(false);
		}
		
		for (int i = 0; i < players.Length; i++)
		{
			GameManager.Instance.SetUpMaterial(players[i].GetComponent<PlayerStateManager>(), players[i].GetComponent<PlayerStateManager>().playerNumber);
		}

		PowerManager.Instance.ActivateDeactivatePower(0, false);
		PowerManager.Instance.ChangeTurnPlayer();
	}

	IEnumerator StartBeforeClearCoroutine()
	{
		yield return new WaitForSeconds(3f);
		
		_particleToDeactivatePlayerOne.SetActive(false);
		_particleToDeactivatePlayerTwo.SetActive(false);
	}
}