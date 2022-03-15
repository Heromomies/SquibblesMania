using System;
using System.Collections.Generic;
using DG.Tweening;
using DigitalRubyShared;
using UnityEngine;
using UnityEngine.EventSystems;

public class DashPower : MonoBehaviour, IManagePower
{
	[Header("POWER SETTINGS")]
	public int dashRange;

	public LayerMask layerMaskInteractableAndPlayer;
	public LayerMask layerMaskPlayer;

	public List<Transform> hitTransforms;

	[Header("TOUCH SETTINGS")]
	[Range(1, 10)] public int swipeTouchCount = 1;
	[Range(0.0f, 10.0f)] public float swipeThresholdSeconds;
	[Range(0.0f, 1.0f)] public float minimumDistanceUnits;
	[Range(0.0f, 1.0f)] public float minimumDurationSeconds;

	[Header("MATERIAL SETTINGS")]
	[Space] public Material firstMat;
	public Material secondMat;
	
	[HideInInspector] public GameObject playerCurrentlySelected;
	private RaycastHit _hit;
	private Camera _cam;
	public SwipeGestureRecognizer swipe;
	private readonly List<Vector3> _vectorRaycast = new List<Vector3> {Vector3.back, Vector3.forward, Vector3.right, Vector3.left};
	private readonly List<RaycastResult> _raycast = new List<RaycastResult>();
	public LongPressGestureRecognizer LongPressBlocMovementGesture { get; private set; }

	#region Swipe Gesture Enabled

	private void OnEnable()
	{
		_cam = Camera.main;
		
		swipe = new SwipeGestureRecognizer();
		swipe.StateUpdated += SwipeUpdated;
		swipe.DirectionThreshold = 0;
		swipe.MinimumNumberOfTouchesToTrack = swipe.MaximumNumberOfTouchesToTrack = swipeTouchCount;
		swipe.MinimumDistanceUnits = minimumDistanceUnits;
		swipe.EndMode = SwipeGestureRecognizerEndMode.EndImmediately;
		swipe.ThresholdSeconds = swipeThresholdSeconds;
		swipe.AllowSimultaneousExecutionWithAllGestures();
		FingersScript.Instance.AddGesture(swipe);
		
		//Set up the new gesture 
		LongPressBlocMovementGesture = new LongPressGestureRecognizer();
		LongPressBlocMovementGesture.StateUpdated += LongPressBlocMovementGestureOnStateUpdated;
		//LongPressBlocMovementGesture.ThresholdUnits = 0.0f;
		LongPressBlocMovementGesture.MinimumDurationSeconds = minimumDurationSeconds;
		LongPressBlocMovementGesture.AllowSimultaneousExecution(swipe);
		FingersScript.Instance.AddGesture(LongPressBlocMovementGesture);

		DisplayPower();
	}


	#endregion

	#region Long Press Function
	
	private void LongPressBlocMovementGestureOnStateUpdated(GestureRecognizer gesture)
	{
		if (gesture.State == GestureRecognizerState.Began)
		{
			PointerEventData p = new PointerEventData(EventSystem.current);
			p.position = new Vector2(gesture.FocusX, gesture.FocusY);

			_raycast.Clear();
			EventSystem.current.RaycastAll(p, _raycast);
			// Cast a ray from the camera
			Ray ray = _cam.ScreenPointToRay(p.position);

			if (Physics.Raycast(ray, out _hit, Mathf.Infinity, layerMaskPlayer))
			{
				if (_hit.collider.name == GameManager.Instance.currentPlayerTurn.name)
				{
					playerCurrentlySelected = _hit.transform.gameObject;
				}
			}
		}
	}
	#endregion

	#region Swipe To Dash

	private void SwipeUpdated(GestureRecognizer gesture) // When we swipe
	{
		SwipeGestureRecognizer swipeGestureRecognizer = gesture as SwipeGestureRecognizer;
		if (swipeGestureRecognizer.State == GestureRecognizerState.Ended  && playerCurrentlySelected != null)
		{
			Debug.Log(swipeGestureRecognizer.EndDirection);

			switch (GameManager.Instance.actualCamPreset.presetNumber)
			{
				case 1: switch (swipeGestureRecognizer.EndDirection)
					{
						case SwipeGestureRecognizerDirection.Down: SwipeDashDirection(0); break;
						case SwipeGestureRecognizerDirection.Up: SwipeDashDirection(1); break;
						case SwipeGestureRecognizerDirection.Right: SwipeDashDirection(2); break;
						case SwipeGestureRecognizerDirection.Left: SwipeDashDirection(3); break;
					}
					break;
				case 3: switch (swipeGestureRecognizer.EndDirection)
				{
					case SwipeGestureRecognizerDirection.Down: SwipeDashDirection(1); break;
					case SwipeGestureRecognizerDirection.Up: SwipeDashDirection(0); break;
					case SwipeGestureRecognizerDirection.Right: SwipeDashDirection(3); break;
					case SwipeGestureRecognizerDirection.Left: SwipeDashDirection(2); break;
				} break;
				case 2: switch (swipeGestureRecognizer.EndDirection)
				{
					case SwipeGestureRecognizerDirection.Down: SwipeDashDirection(1); break;
					case SwipeGestureRecognizerDirection.Up: SwipeDashDirection(0); break;
					case SwipeGestureRecognizerDirection.Right: SwipeDashDirection(3); break;
					case SwipeGestureRecognizerDirection.Left: SwipeDashDirection(2); break;
				} break;
				case 4: switch (swipeGestureRecognizer.EndDirection)
				{
					case SwipeGestureRecognizerDirection.Down: SwipeDashDirection(0); break;
					case SwipeGestureRecognizerDirection.Up: SwipeDashDirection(1); break;
					case SwipeGestureRecognizerDirection.Right: SwipeDashDirection(2); break;
					case SwipeGestureRecognizerDirection.Left: SwipeDashDirection(3); break;
				} break;
			}
		}
	}

	#endregion

	#region Swipe Dash Direction

	
	//Update method of the long press gesture
	public void SwipeDashDirection(int numberDirectionVector) // When we clicked on button
	{
		var position = GameManager.Instance.currentPlayerTurn.transform.position;
		transform.position = position;

		if (Physics.Raycast(transform.position, _vectorRaycast[numberDirectionVector], out var hit, dashRange)) // launch the raycast
		{
			if (hit.collider.gameObject.layer == 3 || hit.collider.gameObject.layer == 0)
			{
				var distance = Vector3.Distance(position, hit.collider.transform.position);
				distance = (int) distance;

				if (distance <= 3.5f)
				{
					GameManager.Instance.currentPlayerTurn.transform.DOMove(
						position + _vectorRaycast[numberDirectionVector] * (distance - 1), 0.05f);
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

				if (Physics.Raycast(hit.transform.position, _vectorRaycast[numberDirectionVector], out var hitPlayerTouched, distanceBetweenTwoPlayers,
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
									position + _vectorRaycast[numberDirectionVector] *
									(distanceBetweenTwoPlayers + distanceBetweenBlockAndPlayerTouched - 2), 0.05f);
								break;
							case 3:
								GameManager.Instance.currentPlayerTurn.transform.DOMove(
									position + _vectorRaycast[numberDirectionVector] *
									(distanceBetweenTwoPlayers - 1), 0.05f);
								break;
						}

						//In any case, the player repulsed will stop his course before the bloc who stop him
						hit.collider.transform.DOMove(hit.collider.transform.position
						                              + _vectorRaycast[numberDirectionVector] * (distanceBetweenBlockAndPlayerTouched - 1), 1f);
					}
				}
				else // If the player repulsed don't have any bloc behind him, the player who dash just dash and repulse from 1 the player
				{
					GameManager.Instance.currentPlayerTurn.transform.DOMove(
						position + _vectorRaycast[numberDirectionVector] * dashRange, 0.05f);
					hit.collider.transform.DOMove(hit.collider.transform.position
					                              + _vectorRaycast[numberDirectionVector] * distanceBetweenTwoPlayers, 1f);
				}
			}
			else if (hit.collider.gameObject.layer == 0)
			{
				GameManager.Instance.currentPlayerTurn.transform.DOMove(
					position + _vectorRaycast[numberDirectionVector] * dashRange, 0.1f);
			}
		}
		else // If they are no bloc or players on his path, dash from 3
		{
			GameManager.Instance.currentPlayerTurn.transform.DOMove(
				position + _vectorRaycast[numberDirectionVector] * dashRange, 0.05f);
		}

		PowerManager.Instance.ActivateDeactivatePower(1, false);
		PowerManager.Instance.ChangeTurnPlayer();

		ClearPower();
	}


	#endregion

	#region Display Power

	public void DisplayPower() // Show the path 
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
	}


	#endregion

	#region ChangeMaterial

	void ChangeMaterial(Transform objectToChange)
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

		playerCurrentlySelected = null;
		hitTransforms.Clear();
	}
	
	public void OnDisable()
	{
		if (FingersScript.HasInstance)
		{
			FingersScript.Instance.RemoveGesture(LongPressBlocMovementGesture);
			FingersScript.Instance.RemoveGesture(swipe);
		}
	}
}