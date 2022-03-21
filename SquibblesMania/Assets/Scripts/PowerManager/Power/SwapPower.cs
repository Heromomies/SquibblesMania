using System;
using System.Collections.Generic;
using System.Linq;
using DigitalRubyShared;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwapPower : MonoBehaviour, IManagePower
{
	[Header("POWER SETTINGS")]
	public int range;
	public LayerMask layer;
	[Space] 
	[Header("MATERIALS")] 
	public Material firstMat; 
	public Material secondMat;

	private GameObject _playerToSwap;
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
				PowerManager.Instance.ActivateDeactivatePower(0, false);
				PowerManager.Instance.ChangeTurnPlayer();
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
		SwapPosition(transformPlayerOne, _playerTwo.transform);
	}

	private void SwapPosition(Transform playerOne, Transform playerTwo) // Swap the position between the two players
	{
		playerOne.position = playerTwo.position;
		playerTwo.position = _pos;

		ClearPower();
	}
	
	public void ClearPower()
	{	SwapTouchGesture.StateUpdated -= PlayerTouchGestureUpdated;

		for (int i = 0; i < players.Length; i++)
		{
			Transform child = players[i].transform.GetChild(1);

			child.GetComponentInChildren<Renderer>().material.color = firstMat.color;
		}

		PowerManager.Instance.ActivateDeactivatePower(0, false);
		PowerManager.Instance.ChangeTurnPlayer();
		
	}
}