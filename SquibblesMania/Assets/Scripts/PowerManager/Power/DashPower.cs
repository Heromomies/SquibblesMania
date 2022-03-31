using System;
using System.Collections.Generic;
using DG.Tweening;
using DigitalRubyShared;
using UnityEngine;
using UnityEngine.EventSystems;

public class DashPower : MonoBehaviour, IManagePower
{
	[Header("POWER SETTINGS")] public int dashRange;
	public LayerMask layerPlayerInteractable;
	public LayerMask layerInteractable;
	public LayerMask layerShowPath;

	public List<Transform> hitTransforms;
	
	public Transform baseSpawnRaycastTransform;
	public Transform raycastPlayer;

	[HideInInspector] public List<GameObject> listObjectToSetActiveFalse;

	[Header("TOUCH SETTINGS")] [Range(1, 10)]
	public int swipeTouchCount = 1;

	[Range(0.0f, 10.0f)] public float swipeThresholdSeconds;
	[Range(0.0f, 1.0f)] public float minimumDistanceUnits;

	public SwipeGestureRecognizer swipe;
	private readonly List<Vector3> _vectorRaycast = new List<Vector3> {Vector3.back, Vector3.forward, Vector3.right, Vector3.left};
	private readonly List<RaycastResult> raycast = new List<RaycastResult>();

	private Vector2 _focus, _startFocus;
	private float _offset;
	private Camera _cam;
	private int _distanceDisplayPower = 10;
	private int _distanceDisplayDash = 3;
	private float _distV1, _distV2, _distV3, _distV4;
	
	public PanGestureRecognizer SwapTouchGesture { get; private set; }

	[Header("DISPLAY POWER TRANSFORM")] public Conditions[] displayPower;

	[Serializable]
	public struct Conditions
	{
		public List<Transform> raycastTransform;
	}


	#region Swipe Gesture Enabled

	private void OnEnable()
	{
		/*swipe = new SwipeGestureRecognizer();
		swipe.StateUpdated += SwipeUpdated;
		swipe.DirectionThreshold = 0;
		swipe.MinimumNumberOfTouchesToTrack = swipe.MaximumNumberOfTouchesToTrack = swipeTouchCount;
		swipe.MinimumDistanceUnits = minimumDistanceUnits;
		swipe.EndMode = SwipeGestureRecognizerEndMode.EndImmediately;
		swipe.ThresholdSeconds = swipeThresholdSeconds;
		swipe.AllowSimultaneousExecutionWithAllGestures();
		FingersScript.Instance.AddGesture(swipe);*/

		SwapTouchGesture = new PanGestureRecognizer();
		SwapTouchGesture.ThresholdUnits = 0.0f; // start right away
		//Add new gesture
		SwapTouchGesture.StateUpdated += PlayerTouchGestureUpdated;
		SwapTouchGesture.AllowSimultaneousExecutionWithAllGestures();

		FingersScript.Instance.AddGesture(SwapTouchGesture);

		_cam = Camera.main;
		_offset = PlayerMovementManager.Instance.offset;

		DisplayPower();
	}

	#endregion

	private void PlayerTouchGestureUpdated(GestureRecognizer gesture)
	{
		if (gesture.State == GestureRecognizerState.Began)
		{
			PointerEventData p = new PointerEventData(EventSystem.current);
			p.position = new Vector2(gesture.FocusX, gesture.FocusY);

			raycast.Clear();
			EventSystem.current.RaycastAll(p, raycast);

			Ray ray = _cam.ScreenPointToRay(p.position);

			if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, layerShowPath))
			{
				var playerPos = GameManager.Instance.currentPlayerTurn.transform.position;
				var hitInfoPos = hitInfo.collider.transform.position;

				if (playerPos.x < hitInfoPos.x && Math.Abs(playerPos.z - hitInfoPos.z) < 0.1f)
				{
					DashDirection(2); // Right
				}

				if (playerPos.x > hitInfoPos.x && Math.Abs(playerPos.z - hitInfoPos.z) < 0.1f)
				{
					DashDirection(3); // Left
				}

				if (playerPos.z > hitInfoPos.z && Math.Abs(playerPos.x - hitInfoPos.x) < 0.1f)
				{
					DashDirection(0); // Down
				}

				if (playerPos.z < hitInfoPos.z && Math.Abs(playerPos.x - hitInfoPos.x) < 0.1f)
				{
					DashDirection(1); // Up
				}
			}
			else
			{
				gesture.Reset();
			}
		}
	}


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

	#region Swipe To Dash

	private void SwipeUpdated(GestureRecognizer gesture) // When we swipe
	{
		SwipeGestureRecognizer swipeGestureRecognizer = gesture as SwipeGestureRecognizer;
		if (swipeGestureRecognizer.State == GestureRecognizerState.Ended)
		{
			var endDirection = Swipe(swipeGestureRecognizer);

			switch (GameManager.Instance.actualCamPreset.presetNumber)
			{
				case 1:
					switch (endDirection)
					{
						case SwipeGestureRecognizerDirection.Down:
							DashDirection(0);
							break;
						case SwipeGestureRecognizerDirection.Up:
							DashDirection(1);
							break;
						case SwipeGestureRecognizerDirection.Right:
							DashDirection(2);
							break;
						case SwipeGestureRecognizerDirection.Left:
							DashDirection(3);
							break;
					}

					break;
				case 2:
					switch (endDirection)
					{
						case SwipeGestureRecognizerDirection.Down:
							DashDirection(0);
							break;
						case SwipeGestureRecognizerDirection.Up:
							DashDirection(1);
							break;
						case SwipeGestureRecognizerDirection.Right:
							DashDirection(2);
							break;
						case SwipeGestureRecognizerDirection.Left:
							DashDirection(3);
							break;
					}

					break;
				case 3:
					switch (endDirection)
					{
						case SwipeGestureRecognizerDirection.Down:
							DashDirection(1);
							break;
						case SwipeGestureRecognizerDirection.Up:
							DashDirection(0);
							break;
						case SwipeGestureRecognizerDirection.Right:
							DashDirection(3);
							break;
						case SwipeGestureRecognizerDirection.Left:
							DashDirection(2);
							break;
					}

					break;
				case 4:
					switch (endDirection)
					{
						case SwipeGestureRecognizerDirection.Down:
							DashDirection(1);
							break;
						case SwipeGestureRecognizerDirection.Up:
							DashDirection(0);
							break;
						case SwipeGestureRecognizerDirection.Right:
							DashDirection(3);
							break;
						case SwipeGestureRecognizerDirection.Left:
							DashDirection(2);
							break;
					}

					break;
			}
		}
	}

	#endregion

	#region Swipe Dash Direction

	//Update method of the long press gesture
	public void DashDirection(int numberDirectionVector) // When we clicked on button
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
					layerPlayerInteractable)) // If the player repulsed touch a block behind him
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

		NFCManager.Instance.powerActivated = true;

		ClearPower();
	}

	#endregion

	#region Display Power

	public void DisplayPower() // Show the path 
	{
		var posPlayer = GameManager.Instance.currentPlayerTurn.transform.position;
		baseSpawnRaycastTransform.position = new Vector3(posPlayer.x, posPlayer.y + _distanceDisplayDash, posPlayer.z);
		raycastPlayer.position = baseSpawnRaycastTransform.position;

		for (int i = 0; i < displayPower.Length; i++)
		{
			var rot = 0f;
			
			if (_vectorRaycast[i] == Vector3.right)
			{
				rot = 180f;
			}
			else if (_vectorRaycast[i] == Vector3.back)
			{
				rot = 0f;
			}
			else if (_vectorRaycast[i] == Vector3.forward)
			{
				rot = 90f;
			}
			else if (_vectorRaycast[i] == Vector3.left)
			{
				rot = 270f;
			}
			
			if (Physics.Raycast(displayPower[i].raycastTransform[2].position, Vector3.down, out var hitOne, _distanceDisplayPower, layerPlayerInteractable)) // launch the raycast
			{
				var distV1 = Vector3.Distance(displayPower[i].raycastTransform[2].position, hitOne.transform.position);
				_distV1 = distV1;
				
				if (Physics.Raycast(displayPower[i].raycastTransform[1].position, Vector3.down, out var hitTwo, _distanceDisplayPower, layerPlayerInteractable)) // launch the raycast
				{
					var distV2 = Vector3.Distance(displayPower[i].raycastTransform[1].position, hitTwo.transform.position);
					_distV2 = distV2;
				}
				if (Physics.Raycast(displayPower[i].raycastTransform[0].position, Vector3.down, out var hitThird, _distanceDisplayPower, layerPlayerInteractable)) // launch the raycast
				{
					var distV3 = Vector3.Distance(displayPower[i].raycastTransform[0].position, hitThird.transform.position);
					_distV3 = distV3;
				}
				
				if (Physics.Raycast(raycastPlayer.position, Vector3.down, out var hitPlayer, _distanceDisplayPower, layerInteractable)) // launch the raycast
				{
					var distV4 = Vector3.Distance(raycastPlayer.position, hitPlayer.transform.position);
					_distV4 = distV4;
				}
				
				if (_distV4 <= _distV3 && _distV4 <= _distV2 && _distV4 <= _distV1)
				{
					SpawnObjectOnFinalPathDash(hitOne.transform);
					SpawnShaderOnPathDash(hitTwo.transform, rot);
					SpawnShaderOnPathDash(hitThird.transform, rot);
				}
				else if (_distV4 <= _distV3 && _distV3 <= _distV2 && _distV2 >= _distV1)
				{
					SpawnObjectOnFinalPathDash(hitTwo.transform);
					SpawnShaderOnPathDash(hitThird.transform, rot);
				}
				else if (_distV4 <= _distV3 && _distV2 <= _distV3)
				{
					SpawnObjectOnFinalPathDash(hitThird.transform);	
				}

				Debug.Log("DIST V3 : "+_distV3);
				Debug.Log("DIST V4 : "+_distV4);
			}
			else if(hitOne.collider == null)
			{
				if (Physics.Raycast(displayPower[i].raycastTransform[1].position, Vector3.down, out var hitTwo, _distanceDisplayPower, layerPlayerInteractable)) // launch the raycast
				{
					var distV2 = Vector3.Distance(displayPower[i].raycastTransform[1].position, hitTwo.transform.position);
					_distV2 = distV2;
				}
				else if(hitTwo.collider == null)
				{
					if (Physics.Raycast(displayPower[i].raycastTransform[0].position, Vector3.down, out var hitFourth, _distanceDisplayPower, layerPlayerInteractable)) // launch the raycast
					{
						var distV3 = Vector3.Distance(displayPower[i].raycastTransform[0].position, hitFourth.transform.position);
						_distV3 = distV3;
						
						if (Physics.Raycast(raycastPlayer.position, Vector3.down, out var hitPlayer, _distanceDisplayPower, layerInteractable)) // launch the raycast
						{
							var distV4 = Vector3.Distance(raycastPlayer.position, hitPlayer.transform.position);
							_distV4 = distV4;
						}

						if (_distV4 <= _distV2 && _distV2 <= _distV3)
						{ 
							SpawnObjectOnFinalPathDash(hitFourth.transform);
						}
					}
					else if(hitFourth.collider == null)
					{
						_distV3 = _distanceDisplayDash;
					}
				}
				if (Physics.Raycast(displayPower[i].raycastTransform[0].position, Vector3.down, out var hitThird, _distanceDisplayPower, layerPlayerInteractable)) // launch the raycast
				{
					var distV3 = Vector3.Distance(displayPower[i].raycastTransform[0].position, hitThird.transform.position);
					_distV3 = distV3;
				}
				if (Physics.Raycast(raycastPlayer.position, Vector3.down, out var hitPlayerTwo, _distanceDisplayPower, layerInteractable)) // launch the raycast
				{
					var distV4 = Vector3.Distance(raycastPlayer.position, hitPlayerTwo.transform.position);
					_distV4 = distV4;
				}
				if (_distV4 <= _distV3 && _distV2 <= _distV3)
				{
					SpawnObjectOnFinalPathDash(hitTwo.transform);
					SpawnShaderOnPathDash(hitThird.transform, rot);
				}
			}
		}

		#region OldCode

		/*var currentBlockUnderPlayer = GameManager.Instance.currentPlayerTurn.currentBlockPlayerOn;
		var parentCurrentBlock = currentBlockUnderPlayer.GetComponentInParent<GroupBlockDetection>().transform.position.y;

		for (int i = 0; i < _vectorRaycast.Count; i++)
		{
			var dist = 0;
			var rot = 0f;
			
			if (_vectorRaycast[i] == Vector3.right)
			{
				rot = 90f;
			}
			else if (_vectorRaycast[i] == Vector3.back)
			{
				rot = 180f;
			}
			else if (_vectorRaycast[i] == Vector3.forward)
			{
				rot = 0f;
			}
			else if (_vectorRaycast[i] == Vector3.left)
			{
				rot = 270f;
			}

			if (Physics.Raycast(GameManager.Instance.currentPlayerTurn.transform.position, _vectorRaycast[i], out var hitBloc,
				dashRange, layerInteractable))
			{
				var distBetweenPlayerAndBloc = Vector3.Distance(GameManager.Instance.currentPlayerTurn.transform.position, hitBloc.transform.position);

				dist = (int) distBetweenPlayerAndBloc;
				dist -= 1;
			}
			if (dist == 1)
			{
				if (Physics.Raycast(currentBlockUnderPlayer.position, _vectorRaycast[i], out var hitFirstBloc,
					_distanceDisplayPower, layerInteractable)) // launch the raycast
				{
					if (Math.Abs(parentCurrentBlock - hitFirstBloc.transform.GetComponentInParent<GroupBlockDetection>().transform.position.y) < 0.1f)
					{
						SpawnObjectOnFinalPathDash(hitFirstBloc.transform);
						hitTransforms.Add(hitFirstBloc.transform);
					}
				}
			}

			else if (dist == 2 || dist == 3)
			{
				if (Physics.Raycast(currentBlockUnderPlayer.position, _vectorRaycast[i], out var hitFirstBloc,
					_distanceDisplayPower, layerInteractable)) // launch the raycast
				{
					if (Math.Abs(parentCurrentBlock - hitFirstBloc.transform.GetComponentInParent<GroupBlockDetection>().transform.position.y) < 0.1f)
					{
						SpawnShaderOnPathDash(hitFirstBloc.transform, rot);
						hitTransforms.Add(hitFirstBloc.transform);
					}
				}

				if (Physics.Raycast(currentBlockUnderPlayer.position + _vectorRaycast[i], _vectorRaycast[i], out var hitSecondBloc,
					_distanceDisplayPower, layerInteractable)) // launch the raycast
				{
					if (Math.Abs(parentCurrentBlock - hitSecondBloc.transform.GetComponentInParent<GroupBlockDetection>().transform.position.y) < 0.1f)
					{
						SpawnObjectOnFinalPathDash(hitSecondBloc.transform);
						hitTransforms.Add(hitSecondBloc.transform);
					}
				}
			}

			// ReSharper disable once UselessComparisonToIntegralConstant
			else if (dist == 0 || dist > 3)
			{
				if (Physics.Raycast(currentBlockUnderPlayer.position, _vectorRaycast[i], out var hitFirstBloc,
					_distanceDisplayPower, layerInteractable)) // launch the raycast
				{
					if (Math.Abs(parentCurrentBlock - hitFirstBloc.transform.GetComponentInParent<GroupBlockDetection>().transform.position.y) < 0.1f)
					{
						SpawnShaderOnPathDash(hitFirstBloc.transform, rot);
						hitTransforms.Add(hitFirstBloc.transform);
					}
				}

				if (Physics.Raycast(currentBlockUnderPlayer.position + _vectorRaycast[i], _vectorRaycast[i], out var hitSecondBloc,
					_distanceDisplayPower, layerInteractable)) // launch the raycast
				{
					if (Math.Abs(parentCurrentBlock - hitSecondBloc.transform.GetComponentInParent<GroupBlockDetection>().transform.position.y) < 0.1f)
					{
						SpawnShaderOnPathDash(hitSecondBloc.transform, rot);
						hitTransforms.Add(hitSecondBloc.transform);
					}
				}

				if (Physics.Raycast(currentBlockUnderPlayer.position + (_vectorRaycast[i] * 2), _vectorRaycast[i], out var hitThirdBloc,
					_distanceDisplayPower, layerInteractable)) // launch the raycast
				{
					if (Math.Abs(parentCurrentBlock - hitThirdBloc.transform.GetComponentInParent<GroupBlockDetection>().transform.position.y) < 0.1f)
					{
						SpawnObjectOnFinalPathDash(hitThirdBloc.transform);
						hitTransforms.Add(hitThirdBloc.transform);
					}
				}
			}
		}*/

		#endregion
	}

	#endregion

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		for (int i = 0; i < displayPower.Length; i++)
		{
			Gizmos.DrawLine(displayPower[i].raycastTransform[0].position,
				new Vector3(displayPower[i].raycastTransform[0].position.x, displayPower[i].raycastTransform[0].position.y - _distanceDisplayDash,
					displayPower[i].raycastTransform[0].position.z));
			Gizmos.DrawLine(displayPower[i].raycastTransform[1].position,
				new Vector3(displayPower[i].raycastTransform[1].position.x, displayPower[i].raycastTransform[1].position.y - _distanceDisplayDash,
					displayPower[i].raycastTransform[1].position.z));
			Gizmos.DrawLine(displayPower[i].raycastTransform[2].position,
				new Vector3(displayPower[i].raycastTransform[2].position.x, displayPower[i].raycastTransform[2].position.y - _distanceDisplayDash,
					displayPower[i].raycastTransform[2].position.z));
		}
	}

	#region SpawnObjectOnDash

	void SpawnObjectOnFinalPathDash(Transform objectToChange)
	{
		var objPos = objectToChange.position;
		
		GameObject obj = PoolManager.Instance.SpawnObjectFromPool("PlanePowerPath",
			new Vector3(objPos.x, objPos.y + 1.02f, objPos.z), Quaternion.identity, null);

		listObjectToSetActiveFalse.Add(obj);
	}

	#endregion

	#region SpawnObjectOnDash

	void SpawnShaderOnPathDash(Transform objectToChange, float position)
	{
		var objPos = objectToChange.position;
		var objRot = objectToChange.localPosition;

		GameObject obj = PoolManager.Instance.SpawnObjectFromPool("ShaderPlanePower",
			new Vector3(objPos.x, objPos.y + 1.02f, objPos.z), Quaternion.Euler(objRot.x, position, objRot.z), null);

		listObjectToSetActiveFalse.Add(obj);
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
		hitTransforms.Clear();

		foreach (var g in listObjectToSetActiveFalse)
		{
			g.SetActive(false);
		}

		listObjectToSetActiveFalse.Clear();

		PowerManager.Instance.ActivateDeactivatePower(1, false);
		PowerManager.Instance.ChangeTurnPlayer();
	}

	public void OnDisable()
	{
		if (FingersScript.HasInstance)
		{
			FingersScript.Instance.RemoveGesture(swipe);
		}
	}
}