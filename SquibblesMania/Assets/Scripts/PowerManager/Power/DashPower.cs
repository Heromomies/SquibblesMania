using System;
using System.Collections;
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
	
	private readonly List<Vector3> _vectorRaycast = new List<Vector3> {Vector3.back, Vector3.forward, Vector3.right, Vector3.left};
	private readonly List<RaycastResult> raycast = new List<RaycastResult>();
	
	private Camera _cam;
	private int _distanceDisplayPower = 10;
	private int _distanceDisplayDash = 3;
	private float _distV1, _distV2, _distV3, _distV4;
	private GameObject _particleToDeactivate;
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
		SwapTouchGesture = new PanGestureRecognizer();
		SwapTouchGesture.ThresholdUnits = 0.0f; // start right away
		//Add new gesture
		SwapTouchGesture.StateUpdated += PlayerTouchGestureUpdated;
		SwapTouchGesture.AllowSimultaneousExecutionWithAllGestures();

		FingersScript.Instance.AddGesture(SwapTouchGesture);

		_cam = Camera.main;

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
				var player = GameManager.Instance.currentPlayerTurn;
				var playerPos = player.transform.position;
				var hitInfoPos = hitInfo.collider.transform.position;
				Quaternion quat = Quaternion.Euler(0,0,0);
				
				if (playerPos.x < hitInfoPos.x && Math.Abs(playerPos.z - hitInfoPos.z) < 0.1f)
				{
					DashDirection(2); // Right
					quat = Quaternion.Euler(0,90f,0);
					GameManager.Instance.currentPlayerTurn.gameObject.transform.rotation = quat;
				}

				if (playerPos.x > hitInfoPos.x && Math.Abs(playerPos.z - hitInfoPos.z) < 0.1f)
				{
					DashDirection(3); // Left
					quat = Quaternion.Euler(0,-90f,0);
					GameManager.Instance.currentPlayerTurn.gameObject.transform.rotation = quat;
				}

				if (playerPos.z > hitInfoPos.z && Math.Abs(playerPos.x - hitInfoPos.x) < 0.1f)
				{
					DashDirection(0); // Down
					quat = Quaternion.Euler(0,180f,0);
					GameManager.Instance.currentPlayerTurn.gameObject.transform.rotation = quat;
				}

				if (playerPos.z < hitInfoPos.z && Math.Abs(playerPos.x - hitInfoPos.x) < 0.1f)
				{
					DashDirection(1); // Up
					quat = Quaternion.Euler(0,0f,0);
					GameManager.Instance.currentPlayerTurn.gameObject.transform.rotation = quat;
				}
			}
			else
			{
				gesture.Reset();
			}
		}
	}

	#region Dash Direction

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
					ActiveParticle();
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
								ActiveParticle();
								break;
							case 3:
								GameManager.Instance.currentPlayerTurn.transform.DOMove(
									position + _vectorRaycast[numberDirectionVector] *
									(distanceBetweenTwoPlayers - 1), 0.05f);
								ActiveParticle();
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
					
					ActiveParticle();
				}
			}
			else if (hit.collider.gameObject.layer == 0)
			{
				GameManager.Instance.currentPlayerTurn.transform.DOMove(
					position + _vectorRaycast[numberDirectionVector] * dashRange, 0.1f);
				
				ActiveParticle();
			}
		}
		else // If they are no bloc or players on his path, dash from 3
		{
			GameManager.Instance.currentPlayerTurn.transform.DOMove(
				position + _vectorRaycast[numberDirectionVector] * dashRange, 0.05f);

			ActiveParticle();
		}

		NFCManager.Instance.powerActivated = true;

		ClearPower();
	}

	#endregion
	
	private void ActiveParticle()
	{
		var playerTransform = GameManager.Instance.currentPlayerTurn.transform;
		_particleToDeactivate = PoolManager.Instance.SpawnObjectFromPool("ParticleDash", playerTransform.position, playerTransform.rotation, playerTransform);
	}
	
	#region Display Power

	public void DisplayPower() // Show the path 
	{
		Debug.Log("IDecihfih");
		
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

						if (_distV4 <= _distV3)
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
				
				if (_distV4 <= _distV3 && _distV3 <= _distV2 && _distV4 <= _distV2)
				{
					SpawnObjectOnFinalPathDash(hitTwo.transform);
					SpawnShaderOnPathDash(hitThird.transform, rot);
				}
				else if (_distV4 <= _distV3)
				{ 
					SpawnObjectOnFinalPathDash(hitThird.transform);
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

	#region SpawnObjectOnDash

	void SpawnObjectOnFinalPathDash(Transform objectToChange)
	{
		if (objectToChange != null)
		{
			var objPos = objectToChange.position;
		
			GameObject obj = PoolManager.Instance.SpawnObjectFromPool("PlanePowerPath",
				new Vector3(objPos.x, objPos.y + 1.02f, objPos.z), Quaternion.identity, null);

			listObjectToSetActiveFalse.Add(obj);
		}
	}

	#endregion

	#region SpawnShaderOnDash

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

	IEnumerator CoroutineDeactivateParticle()
	{
		yield return new WaitForSeconds(0.1f);
		
		if(_particleToDeactivate != null)
			_particleToDeactivate.SetActive(false);
		
		hitTransforms.Clear();

		foreach (var g in listObjectToSetActiveFalse)
		{
			g.SetActive(false);
		}
		listObjectToSetActiveFalse.Clear();

		PowerManager.Instance.ActivateDeactivatePower(1, false);
		PowerManager.Instance.ChangeTurnPlayer();
	}
	
	public void ClearPower() // Clear the power
	{
		StartCoroutine(CoroutineDeactivateParticle());
	}

	private void OnDisable()
	{
		if (FingersScript.HasInstance)
		{
			FingersScript.Instance.RemoveGesture(SwapTouchGesture);
		}
	}
}