using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DigitalRubyShared;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MirorPower : MonoBehaviour, IManagePower
{
	[Header("POWER SETTINGS")] [Space] public LayerMask layerPlayer;
	public LayerMask layerMaskInteractableAndPlayer;
	public LayerMask layerInteractable;

	public float rangeDetectionPlayer;
	public List<Transform> hitTransforms;
	[HideInInspector] public GameObject zombiePlayer;
	public List<TextMeshProUGUI> textWhenNoZombieAreSelected;

	[Header("TOUCH SETTINGS")] [Space] [Range(1, 10)]
	public int dashRange;

	[Range(1, 10)] public int swipeTouchCount = 1;
	[Range(0.0f, 10.0f)] public float swipeThresholdSeconds;
	[Range(0.0f, 1.0f)] public float minimumDistanceUnits;

	[Header("MATERIAL SETTINGS")] [Space] public Material firstMat;
	public Material secondMat;

	[Space] public Material zombieMat;
	public Material changeZombieMat;

	public SwipeGestureRecognizer swipe;
	private readonly WaitForSeconds _timeBetweenPlayerZombieMovement = new WaitForSeconds(0.3f);
	private readonly List<Vector3> _vectorRaycast = new List<Vector3> {Vector3.back, Vector3.forward, Vector3.right, Vector3.left};

	private readonly List<RaycastResult> raycast = new List<RaycastResult>();
	private PanGestureRecognizer SwapTouchGesture { get; set; }

	private Camera _cam;
	private Vector2 _focus, _startFocus;
	private float _offset;

	private void Awake()
	{
		_cam = Camera.main;
	}

	#region Swipe Gesture

	private void OnEnable() // Add swipe gesture and pan gesture to select a player and move it
	{
		swipe = new SwipeGestureRecognizer();
		swipe.StateUpdated += SwipeUpdated;
		swipe.DirectionThreshold = 0;
		swipe.MinimumNumberOfTouchesToTrack = swipe.MaximumNumberOfTouchesToTrack = swipeTouchCount;
		swipe.MinimumDistanceUnits = minimumDistanceUnits;
		swipe.EndMode = SwipeGestureRecognizerEndMode.EndImmediately;
		swipe.ThresholdSeconds = swipeThresholdSeconds;
		swipe.AllowSimultaneousExecutionWithAllGestures();
		FingersScript.Instance.AddGesture(swipe);

		SwapTouchGesture = new PanGestureRecognizer();
		SwapTouchGesture.ThresholdUnits = 0.0f; // start right away
		//Add new gesture
		SwapTouchGesture.StateUpdated += PlayerTouchGestureUpdated;
		SwapTouchGesture.AllowSimultaneousExecutionWithAllGestures();

		FingersScript.Instance.AddGesture(SwapTouchGesture);

		_offset = PlayerMovementManager.Instance.offset;
		
		DisplayPower();
	}

	#endregion

	#region PlayerTouchGestureUpdated

	private void PlayerTouchGestureUpdated(GestureRecognizer gesture) // To touch a player and select him
	{
		if (gesture.State == GestureRecognizerState.Began)
		{
			PointerEventData p = new PointerEventData(EventSystem.current);
			p.position = new Vector2(gesture.FocusX, gesture.FocusY);

			raycast.Clear();
			EventSystem.current.RaycastAll(p, raycast);

			Ray ray = _cam.ScreenPointToRay(p.position);

			if (Physics.Raycast(ray, out var hitInfo, rangeDetectionPlayer, layerPlayer))
			{
				if (hitInfo.collider.name != GameManager.Instance.currentPlayerTurn.name)
				{
					zombiePlayer = hitInfo.collider.gameObject;

					Transform child = zombiePlayer.transform.GetChild(1);
					child.GetComponentInChildren<Renderer>().material.color = changeZombieMat.color;

					foreach (var actionPlayerPreset in textWhenNoZombieAreSelected)
					{
						actionPlayerPreset.gameObject.SetActive(false);
					}
				}
			}
			else
			{
				gesture.Reset();
			}
		}
	}

	#endregion

	/// <summary>
	/// Swipe adaptation for the isometric view, check the Y rotation of the camera to adapt the swipe
	/// </summary>
	#region SwipeAdaptation

	public SwipeGestureRecognizerDirection Swipe(GestureRecognizer swipeDirection)
	{
		_focus = new Vector2(swipeDirection.FocusX, swipeDirection.FocusY);
		_startFocus = new Vector2(swipeDirection.StartFocusX, swipeDirection.StartFocusY);

		var dir = _focus - _startFocus;

		float angle = Vector3.SignedAngle(dir, Vector3.up, Vector3.forward);
		angle = angle < 0 ? angle + 360 : angle;
		
		var offsetCamera = _cam.transform.eulerAngles.y - _offset;
		angle = Mathf.Repeat(angle + offsetCamera, 360);

		if (270 < angle || angle < 0)
		{
			
			return SwipeGestureRecognizerDirection.Up;
		}
		else if (0 < angle && angle < 90)
		{
			
			return SwipeGestureRecognizerDirection.Right;
		}
		else if (90 < angle && angle < 180)
		{
			
			return SwipeGestureRecognizerDirection.Down;
		}
		else if (180 < angle && angle < 270)
		{
			return SwipeGestureRecognizerDirection.Left;
		}
		else
		{
			return SwipeGestureRecognizerDirection.Any;
		}
	}

	#endregion
	
	#region SwipeUpdated

	private void SwipeUpdated(GestureRecognizer gestureRecognizer) // Swipe update, when we select a player, we can swipe after that
	{
		SwipeGestureRecognizer swipeGestureRecognizer = gestureRecognizer as SwipeGestureRecognizer;
		if (swipeGestureRecognizer.State == GestureRecognizerState.Ended && zombiePlayer != null)
		{
			var endDirection = Swipe(swipeGestureRecognizer);
			
			switch (GameManager.Instance.actualCamPreset.presetNumber)
			{
				case 1 : switch (endDirection) {
						case SwipeGestureRecognizerDirection.Down: SwipeMirorDirection(0); break;
						case SwipeGestureRecognizerDirection.Up: SwipeMirorDirection(1); break;
						case SwipeGestureRecognizerDirection.Right: SwipeMirorDirection(2); break;
						case SwipeGestureRecognizerDirection.Left: SwipeMirorDirection(3); break; }
					break;
				case 2 : switch (endDirection) {
						case SwipeGestureRecognizerDirection.Down: SwipeMirorDirection(0); break;
						case SwipeGestureRecognizerDirection.Up: SwipeMirorDirection(1); break;
						case SwipeGestureRecognizerDirection.Right: SwipeMirorDirection(2); break;
						case SwipeGestureRecognizerDirection.Left: SwipeMirorDirection(3); break; }
					break;
				case 3 : switch (endDirection) {
						case SwipeGestureRecognizerDirection.Down: SwipeMirorDirection(1); break;
						case SwipeGestureRecognizerDirection.Up: SwipeMirorDirection(0); break;
						case SwipeGestureRecognizerDirection.Right: SwipeMirorDirection(3); break;
						case SwipeGestureRecognizerDirection.Left: SwipeMirorDirection(2); break; }
					break;
				case 4 : switch (endDirection) {
						case SwipeGestureRecognizerDirection.Down: SwipeMirorDirection(1); break;
						case SwipeGestureRecognizerDirection.Up: SwipeMirorDirection(0); break;
						case SwipeGestureRecognizerDirection.Right: SwipeMirorDirection(3); break;
						case SwipeGestureRecognizerDirection.Left: SwipeMirorDirection(2); break; }
					break;
			}
		}
	}

	#endregion

	#region SwipeMirorDirection

	private void SwipeMirorDirection(int directionIndex) // When we clicked on button
	{
		var position = GameManager.Instance.currentPlayerTurn.transform.position;
		transform.position = position;

		if (Physics.Raycast(transform.position, _vectorRaycast[directionIndex], out var hit, dashRange)) // launch the raycast
		{
			if (hit.collider.gameObject.layer == 3 || hit.collider.gameObject.layer == 0)
			{
				var distance = Vector3.Distance(position, hit.collider.transform.position);
				distance = (int) distance;

				if (distance <= 3.5f)
				{
					GameManager.Instance.currentPlayerTurn.transform.DOMove(
						position + _vectorRaycast[directionIndex] * (distance - 1), 0.05f);
				}
			}
			else if (hit.collider.gameObject.layer == 6) // When the raycast touch another player
			{
				var distanceBetweenTwoPlayers = Vector3.Distance(position, hit.collider.transform.position);
				distanceBetweenTwoPlayers += 0.1f;
				distanceBetweenTwoPlayers = (int) distanceBetweenTwoPlayers; // check distance between two players

				switch (distanceBetweenTwoPlayers) // inverse distance for the dash, else the player repulsed don't follow the range  
				{
					case 1:
						distanceBetweenTwoPlayers = 3;
						break;
					case 3:
						distanceBetweenTwoPlayers = 1;
						break;
				}

				if (Physics.Raycast(hit.transform.position, _vectorRaycast[directionIndex], out var hitPlayerTouched, distanceBetweenTwoPlayers,
					layerMaskInteractableAndPlayer)) // If the player repulsed touch a block behind him
				{
					var distanceBetweenBlockAndPlayerTouched = Vector3.Distance(hit.transform.position,
						hitPlayerTouched.transform.position);
					distanceBetweenBlockAndPlayerTouched += 0.1f;
					distanceBetweenBlockAndPlayerTouched = (int) distanceBetweenBlockAndPlayerTouched; //Check distance between himself and the block behind him

					var distanceBetweenTwoPlayersWhenABlockIsBehind = Vector3.Distance(position, hit.collider.transform.position);
					distanceBetweenTwoPlayersWhenABlockIsBehind += 0.1f;
					distanceBetweenTwoPlayersWhenABlockIsBehind =
						(int) distanceBetweenTwoPlayersWhenABlockIsBehind; // Check the distance between the two players

					if (distanceBetweenBlockAndPlayerTouched > 1)
					{
						switch (distanceBetweenTwoPlayersWhenABlockIsBehind) // inverse distance for the dash, else the player repulsed don't follow the range  
						{
							case 1:
								distanceBetweenTwoPlayersWhenABlockIsBehind = 3;
								break;
							case 3:
								distanceBetweenTwoPlayersWhenABlockIsBehind = 1;
								break;
						}

						switch (distanceBetweenTwoPlayersWhenABlockIsBehind) // according to the distance between the two players, the dash is not the same
						{
							case 2:
								GameManager.Instance.currentPlayerTurn.transform.DOMove(
									position + _vectorRaycast[directionIndex] *
									(distanceBetweenTwoPlayers + distanceBetweenBlockAndPlayerTouched - 2), 0.05f);
								break;
							case 3:
								GameManager.Instance.currentPlayerTurn.transform.DOMove(
									position + _vectorRaycast[directionIndex] *
									(distanceBetweenTwoPlayers - 1), 0.05f);
								break;
						}

						//In any case, the player repulsed will stop his course before the bloc who stop him
						hit.collider.transform.DOMove(hit.collider.transform.position
						                              + _vectorRaycast[directionIndex] * (distanceBetweenBlockAndPlayerTouched - 1), 1f);
					}
				}
				else // If the player repulsed don't have any bloc behind him, the player who dash just dash and repulse from 1 the player
				{
					GameManager.Instance.currentPlayerTurn.transform.DOMove(
						position + _vectorRaycast[directionIndex] * dashRange, 0.05f);
					hit.collider.transform.DOMove(hit.collider.transform.position
					                              + _vectorRaycast[directionIndex] * distanceBetweenTwoPlayers, 1f);
				}
			}
			else if (hit.collider.gameObject.layer == 0)
			{
				GameManager.Instance.currentPlayerTurn.transform.DOMove(
					position + _vectorRaycast[directionIndex] * dashRange, 0.1f);
			}
		}
		else // If they are no bloc or players on his path, dash from 3
		{
			GameManager.Instance.currentPlayerTurn.transform.DOMove(
				position + _vectorRaycast[directionIndex] * dashRange, 0.05f);
		}

		StartCoroutine(DisplaceZombiePlayer(directionIndex));
	}

	#endregion

	#region DisplaceZombiePlayer

	IEnumerator DisplaceZombiePlayer(int directionZombieIndex) // Same function than before but inversed for the "zombie"
	{
		yield return _timeBetweenPlayerZombieMovement;

		var positionZombiePlayer = zombiePlayer.transform.position;
		transform.position = positionZombiePlayer;

		if (Physics.Raycast(transform.position, -_vectorRaycast[directionZombieIndex], out var hitZombie, dashRange)) // launch the raycast
		{
			if (hitZombie.collider.gameObject.layer == 3 || hitZombie.collider.gameObject.layer == 0)
			{
				var distance = Vector3.Distance(positionZombiePlayer, hitZombie.collider.transform.position);
				distance = (int) distance;

				if (distance <= 3.5f)
				{
					zombiePlayer.transform.DOMove(
						positionZombiePlayer - _vectorRaycast[directionZombieIndex] * (distance - 1), 0.05f);
				}
			}
			else if (hitZombie.collider.gameObject.layer == 6) // When the raycast touch another player
			{
				var distanceBetweenTwoPlayers = Vector3.Distance(positionZombiePlayer, hitZombie.collider.transform.position);
				distanceBetweenTwoPlayers += 0.1f;
				distanceBetweenTwoPlayers = (int) distanceBetweenTwoPlayers; // check distance between two players

				switch (distanceBetweenTwoPlayers) // inverse distance for the dash, else the player repulsed don't follow the range  
				{
					case 1:
						distanceBetweenTwoPlayers = 3;
						break;
					case 3:
						distanceBetweenTwoPlayers = 1;
						break;
				}

				if (Physics.Raycast(hitZombie.transform.position, _vectorRaycast[directionZombieIndex], out var hitPlayerTouched, distanceBetweenTwoPlayers,
					layerMaskInteractableAndPlayer)) // If the player repulsed touch a block behind him
				{
					var distanceBetweenBlockAndPlayerTouched = Vector3.Distance(hitZombie.transform.position,
						hitPlayerTouched.transform.position);
					distanceBetweenBlockAndPlayerTouched += 0.1f;
					distanceBetweenBlockAndPlayerTouched = (int) distanceBetweenBlockAndPlayerTouched; //Check distance between himself and the block behind him

					var distanceBetweenTwoPlayersWhenABlockIsBehind = Vector3.Distance(positionZombiePlayer, hitZombie.collider.transform.position);
					distanceBetweenTwoPlayersWhenABlockIsBehind += 0.1f;
					distanceBetweenTwoPlayersWhenABlockIsBehind =
						(int) distanceBetweenTwoPlayersWhenABlockIsBehind; // Check the distance between the two players

					if (distanceBetweenBlockAndPlayerTouched > 1)
					{
						switch (distanceBetweenTwoPlayersWhenABlockIsBehind) // inverse distance for the dash, else the player repulsed don't follow the range  
						{
							case 1:
								distanceBetweenTwoPlayersWhenABlockIsBehind = 3;
								break;
							case 3:
								distanceBetweenTwoPlayersWhenABlockIsBehind = 1;
								break;
						}

						switch (distanceBetweenTwoPlayersWhenABlockIsBehind) // according to the distance between the two players, the dash is not the same
						{
							case 2:
								zombiePlayer.transform.DOMove(
									positionZombiePlayer - _vectorRaycast[directionZombieIndex] *
									(distanceBetweenTwoPlayers + distanceBetweenBlockAndPlayerTouched - 2), 0.05f);
								break;
							case 3:
								zombiePlayer.transform.DOMove(
									positionZombiePlayer - _vectorRaycast[directionZombieIndex] *
									(distanceBetweenTwoPlayers - 1), 0.05f);
								break;
						}

						//In any case, the player repulsed will stop his course before the bloc who stop him
						hitZombie.collider.transform.DOMove(hitZombie.collider.transform.position
						                                    - _vectorRaycast[directionZombieIndex] * (distanceBetweenBlockAndPlayerTouched - 1), 1f);
					}
				}
				else // If the player repulsed don't have any bloc behind him, the player who dash just dash and repulse from 1 the player
				{
					zombiePlayer.transform.DOMove(
						positionZombiePlayer - _vectorRaycast[directionZombieIndex] * dashRange, 0.05f);
					hitZombie.collider.transform.DOMove(hitZombie.collider.transform.position
					                                    - _vectorRaycast[directionZombieIndex] * distanceBetweenTwoPlayers, 1f);
				}
			}
			else if (hitZombie.collider.gameObject.layer == 0)
			{
				zombiePlayer.transform.DOMove(
					positionZombiePlayer - _vectorRaycast[directionZombieIndex] * dashRange, 0.1f);
			}
		}
		else // If they are no bloc or players on his path, dash from 3
		{
			zombiePlayer.transform.DOMove(
				positionZombiePlayer - _vectorRaycast[directionZombieIndex] * dashRange, 0.05f);
		}

		NFCManager.Instance.powerActivated = true;
		
		ClearPower();
	}

	#endregion

	#region DisplayPower

	public void DisplayPower() // Display the zone who the players can swipe
	{
		var currentBlockUnderPlayer = GameManager.Instance.currentPlayerTurn.currentBlockPlayerOn;
		var parentCurrentBlock = currentBlockUnderPlayer.GetComponentInParent<GroupBlockDetection>().transform.position.y;

		for (int i = 0; i < _vectorRaycast.Count; i++)
		{
			var dist = 0;
			
			if (Physics.Raycast(GameManager.Instance.currentPlayerTurn.transform.position, _vectorRaycast[i], out var hitBloc,
				dashRange, layerInteractable))
			{
				var distBetweenPlayerAndBloc = Vector3.Distance(GameManager.Instance.currentPlayerTurn.transform.position, hitBloc.transform.position);

				dist = (int) distBetweenPlayerAndBloc;
			}

			if (dist == 1 || dist == 2)
			{
				if (Physics.Raycast(currentBlockUnderPlayer.position, _vectorRaycast[i], out var hitFirstBloc, dashRange)) // launch the raycast
				{
					if (Math.Abs(parentCurrentBlock - hitFirstBloc.transform.GetComponentInParent<GroupBlockDetection>().transform.position.y) < 0.1f)
					{
						ChangeMaterial(hitFirstBloc.transform);
						hitTransforms.Add(hitFirstBloc.transform);
					}
				}
			}
			else if(dist == 0 || dist > 2)
			{
				if (Physics.Raycast(currentBlockUnderPlayer.position, _vectorRaycast[i], out var hitFirstBloc, dashRange)) // launch the raycast
				{
					if (Math.Abs(parentCurrentBlock - hitFirstBloc.transform.GetComponentInParent<GroupBlockDetection>().transform.position.y) < 0.1f)
					{
						ChangeMaterial(hitFirstBloc.transform);
						hitTransforms.Add(hitFirstBloc.transform);
					}
				}
				if (Physics.Raycast(currentBlockUnderPlayer.position + _vectorRaycast[i], _vectorRaycast[i], out var hitSecondBloc,
					dashRange)) // launch the raycast
				{
					if (Math.Abs(parentCurrentBlock - hitSecondBloc.transform.GetComponentInParent<GroupBlockDetection>().transform.position.y) < 0.1f)
					{
						ChangeMaterial(hitSecondBloc.transform);
						hitTransforms.Add(hitSecondBloc.transform);
					}
				}
			}
		}

		switch (GameManager.Instance.actualCamPreset.presetNumber)
		{
			case 1:
				textWhenNoZombieAreSelected[0].gameObject.SetActive(true);
				break;
			case 2:
				textWhenNoZombieAreSelected[0].gameObject.SetActive(true);
				break;
			case 3:
				textWhenNoZombieAreSelected[1].gameObject.SetActive(true);
				break;
			case 4:
				textWhenNoZombieAreSelected[1].gameObject.SetActive(true);
				break;
		}
	}

	#endregion

	#region CHANGE MATERIAL

	private void ChangeMaterial(Transform objectToChange) // Change the material of the object
	{
		var color = objectToChange.GetComponent<Renderer>().materials[2].GetColor("_EmissionColor");
		color = secondMat.color;
		objectToChange.GetComponent<Renderer>().materials[2].SetColor("_EmissionColor", color);
	}

	#endregion

	public void CancelPower()
	{
	}

	public void DoPower()
	{
	}

	public void ClearPower() // Clear the power
	{
		for (int i = 0; i < hitTransforms.Count; i++)
		{
			hitTransforms[i].GetComponent<Renderer>().materials[2].SetColor("_EmissionColor", firstMat.color);
		}

		Transform child = zombiePlayer.transform.GetChild(1);
		child.GetComponentInChildren<Renderer>().material.color = zombieMat.color;

		zombiePlayer = null;
		hitTransforms.Clear();

		PowerManager.Instance.ActivateDeactivatePower(3, false);
		PowerManager.Instance.ChangeTurnPlayer();
	}

	public void OnDisable()
	{
		if (FingersScript.HasInstance)
		{
			FingersScript.Instance.RemoveGesture(SwapTouchGesture);
			FingersScript.Instance.RemoveGesture(swipe);
		}
	}
}