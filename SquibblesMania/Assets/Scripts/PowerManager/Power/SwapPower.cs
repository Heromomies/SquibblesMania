using System;
using System.Collections.Generic;
using System.Linq;
using DigitalRubyShared;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwapPower : MonoBehaviour
{
	public int range;
	public LayerMask layer;

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
		ShowPower();
	}

	private void ShowPower() // Show the sphere and admit the player to chose the other player to swap
	{
		 players = Physics.OverlapSphere(transform.position, range, layer);

		_playerOne = GameManager.Instance.currentPlayerTurn.gameObject.GetComponent<Collider>();
		
		switch (players.Length)
		{
			case 1:
				PowerManager.Instance.ActivateDeactivatePower(0, false);
				PowerManager.Instance.ChangeTurnPlayer();
				break;
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
				if (players.ToList().Contains(hitInfo.collider))
				{
					_playerTwo = hitInfo.collider;
					LaunchPower();
				}
			}
			else
			{
				gesture.Reset();
			}
		}
	}
	
	private void LaunchPower()
	{
		var transformPlayerOne = _playerOne.transform;
		_pos = transformPlayerOne.position;
		SwapPosition(transformPlayerOne, _playerTwo.transform);
	}

	private void SwapPosition(Transform playerOne, Transform playerTwo) // Swap the position between the two players
	{
		playerOne.position = playerTwo.position;
		playerTwo.position = _pos;
		
		SwapTouchGesture.StateUpdated -= PlayerTouchGestureUpdated;
		
		PowerManager.Instance.ActivateDeactivatePower(0, false);
		PowerManager.Instance.ChangeTurnPlayer();
	}
}