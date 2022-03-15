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
	public LayerMask layerPlayer;
	public float rangeDetectionPlayer;
	public GameObject zombiePlayer; //TODO make it private

	public int dashRange;

	[Range(1, 10)] public int swipeTouchCount = 1;
	[Range(0.0f, 10.0f)] public float swipeThresholdSeconds;
	[Range(0.0f, 1.0f)] public float minimumDistanceUnits;
	
	public LayerMask layerMaskInteractableAndPlayer;

	public List<Transform> hitTransforms;
	
	[Space] public Material firstMat;
	public Material secondMat;

	public TextMeshProUGUI textWhenNoZombieAreSelected;
	
	public SwipeGestureRecognizer swipe;
	private readonly WaitForSeconds _timeBetweenPlayerZombieMovement = new WaitForSeconds(0.3f);
	private readonly List<Vector3> _vectorRaycast = new List<Vector3> {Vector3.back, Vector3.forward, Vector3.right, Vector3.left};

	private readonly List<RaycastResult> raycast = new List<RaycastResult>();
	private PanGestureRecognizer SwapTouchGesture { get; set; }

	private Camera _cam;

	private void Awake()
	{
		_cam = Camera.main;
	}
	
	private void OnEnable()
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
		
		DisplayPower();
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

			if (Physics.Raycast(ray, out var hitInfo, rangeDetectionPlayer, layerPlayer))
			{
				if (hitInfo.collider.name != GameManager.Instance.currentPlayerTurn.name)
				{
					zombiePlayer = hitInfo.collider.gameObject;
					textWhenNoZombieAreSelected.gameObject.SetActive(false);
				}
			}
			else
			{
				gesture.Reset();
			}
		}
	}

	public void SwipeUpdated(GestureRecognizer gestureRecognizer)
	{
		SwipeGestureRecognizer swipeGestureRecognizer = gestureRecognizer as SwipeGestureRecognizer;
		if (swipeGestureRecognizer.State == GestureRecognizerState.Ended  && zombiePlayer != null)
		{
			switch (GameManager.Instance.actualCamPreset.presetNumber)
			{
				case 1: switch (swipeGestureRecognizer.EndDirection)
					{
						case SwipeGestureRecognizerDirection.Down: SwipeMirorDirection(0); break;
						case SwipeGestureRecognizerDirection.Up: SwipeMirorDirection(1); break;
						case SwipeGestureRecognizerDirection.Right: SwipeMirorDirection(2); break;
						case SwipeGestureRecognizerDirection.Left: SwipeMirorDirection(3); break;
					}
					break;
				case 3: switch (swipeGestureRecognizer.EndDirection)
				{
					case SwipeGestureRecognizerDirection.Down: SwipeMirorDirection(1); break;
					case SwipeGestureRecognizerDirection.Up: SwipeMirorDirection(0); break;
					case SwipeGestureRecognizerDirection.Right: SwipeMirorDirection(3); break;
					case SwipeGestureRecognizerDirection.Left: SwipeMirorDirection(2); break;
				} break;
				case 2: switch (swipeGestureRecognizer.EndDirection)
				{
					case SwipeGestureRecognizerDirection.Down: SwipeMirorDirection(1); break;
					case SwipeGestureRecognizerDirection.Up: SwipeMirorDirection(0); break;
					case SwipeGestureRecognizerDirection.Right: SwipeMirorDirection(3); break;
					case SwipeGestureRecognizerDirection.Left: SwipeMirorDirection(2); break;
				} break;
				case 4: switch (swipeGestureRecognizer.EndDirection)
				{
					case SwipeGestureRecognizerDirection.Down: SwipeMirorDirection(0); break;
					case SwipeGestureRecognizerDirection.Up: SwipeMirorDirection(1); break;
					case SwipeGestureRecognizerDirection.Right: SwipeMirorDirection(2); break;
					case SwipeGestureRecognizerDirection.Left: SwipeMirorDirection(3); break;
				} break;
			}
		}
	}
	
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

	IEnumerator DisplaceZombiePlayer(int directionZombieIndex)
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

		ClearPower();
	}

	public void DisplayPower()
	{
		var currentBlockUnderPlayer = GameManager.Instance.currentPlayerTurn.currentBlockPlayerOn;
		var parentCurrentBlock = currentBlockUnderPlayer.GetComponentInParent<GroupBlockDetection>().transform.position.y;

		for (int i = 0; i < _vectorRaycast.Count; i++)
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

			if (Physics.Raycast(currentBlockUnderPlayer.position + _vectorRaycast[i] * 2, _vectorRaycast[i], out var hitThirdBloc,
				dashRange)) // launch the raycast
			{
				if (Math.Abs(parentCurrentBlock - hitThirdBloc.transform.GetComponentInParent<GroupBlockDetection>().transform.position.y) < 0.1f)
				{
					ChangeMaterial(hitThirdBloc.transform);
					hitTransforms.Add(hitThirdBloc.transform);
				}
			}
		}
		
		textWhenNoZombieAreSelected.gameObject.SetActive(true);
	}

	private void ChangeMaterial(Transform objectToChange)
	{
		var color = objectToChange.GetComponent<Renderer>().materials[2].GetColor("_EmissionColor");
		color = secondMat.color;
		objectToChange.GetComponent<Renderer>().materials[2].SetColor("_EmissionColor", color);
	}
	
	public void CancelPower()
	{
	}

	public void DoPower()
	{
	}

	public void ClearPower()
	{
		for (int i = 0; i < hitTransforms.Count; i++)
		{
			hitTransforms[i].GetComponent<Renderer>().materials[2].SetColor("_EmissionColor", firstMat.color);
		}

		zombiePlayer = null;
		hitTransforms.Clear();
		
		PowerManager.Instance.ActivateDeactivatePower(1, false);
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