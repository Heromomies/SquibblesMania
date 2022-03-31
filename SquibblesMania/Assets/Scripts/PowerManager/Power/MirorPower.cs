using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
	public LayerMask layerShowPath;

	public float rangeDetectionPlayer;
	public List<Transform> hitTransforms;
	[HideInInspector] public GameObject zombiePlayer;
	[Space(25)] public List<TextMeshProUGUI> textWhenNoZombieAreSelected;
	[Space(25)] public List<TextMeshProUGUI> textWhenThereAreNoZombieAround;
	[Space(25)] public List<GameObject> listObjectToSetActiveFalse;

	public Transform baseSpawnRaycastTransform;
	public Transform raycastPlayer;

	[Header("TOUCH SETTINGS")] [Space] [Range(1, 10)]
	public int dashRange;

	[Range(1, 10)] public int swipeTouchCount = 1;
	[Range(0.0f, 10.0f)] public float swipeThresholdSeconds;
	[Range(0.0f, 1.0f)] public float minimumDistanceUnits;

	[Header("MATERIAL SETTINGS")] [Space] public Material firstMat;
	public Material secondMat;
	public Material thirdMat;

	[Space] public Material zombieMat;
	public Material changeZombieMat;

	[HideInInspector] public Collider[] players;
	public SwipeGestureRecognizer swipe;
	private readonly WaitForSeconds _timeBetweenPlayerZombieMovement = new WaitForSeconds(0.3f);
	private readonly List<Vector3> _vectorRaycast = new List<Vector3> {Vector3.back, Vector3.forward, Vector3.right, Vector3.left};

	private readonly List<RaycastResult> raycast = new List<RaycastResult>();
	private PanGestureRecognizer SwapTouchGesture { get; set; }

	private Camera _cam;
	private Vector2 _focus, _startFocus;
	private float _offset;
	private int _distanceDisplayPower = 10;
	private int _distanceDisplayDash = 3;
	private float _distV2, _distV3, _distV4;
	private GameObject _particleToDeactivate;
	
	[Header("DISPLAY POWER TRANSFORM")] public Conditions[] displayPower;

	[Serializable]
	public struct Conditions
	{
		public List<Transform> raycastTransform;
	}

	private void Awake()
	{
		_cam = Camera.main;
	}

	#region Swipe Gesture

	private void OnEnable() // Add swipe gesture and pan gesture to select a player and move it
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

			if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, layerPlayer))
			{
				if (players.ToList().Contains(hitInfo.collider) && hitInfo.transform.name != GameManager.Instance.currentPlayerTurn.name)
				{
					zombiePlayer = hitInfo.collider.gameObject;

					Transform child = zombiePlayer.transform.GetChild(1);
					child.GetComponentInChildren<Renderer>().material.color = changeZombieMat.color;

					var posPlayer = GameManager.Instance.currentPlayerTurn.transform.position;
					baseSpawnRaycastTransform.position = new Vector3(posPlayer.x, posPlayer.y + _distanceDisplayDash, posPlayer.z);
					raycastPlayer.position = baseSpawnRaycastTransform.position;
					
					for (int i = 0; i < _vectorRaycast.Count; i++)
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

						if (Physics.Raycast(displayPower[i].raycastTransform[1].position, Vector3.down, out var hitTwo, _distanceDisplayPower,
							layerInteractable)) // launch the raycast
						{
							var distV2 = Vector3.Distance(displayPower[i].raycastTransform[1].position, hitTwo.transform.position);
							_distV2 = distV2;
						}
						else if (hitTwo.collider == null)
						{
							if (Physics.Raycast(displayPower[i].raycastTransform[0].position, Vector3.down, out var hitFourth, _distanceDisplayPower,
								layerInteractable)) // launch the raycast
							{
								var distV3 = Vector3.Distance(displayPower[i].raycastTransform[0].position, hitFourth.transform.position);
								_distV3 = distV3;

								if (Physics.Raycast(raycastPlayer.position, Vector3.down, out var hitPlayer, _distanceDisplayPower,
									layerInteractable)) // launch the raycast
								{
									var distV4 = Vector3.Distance(raycastPlayer.position, hitPlayer.transform.position);
									_distV4 = distV4;
								}

								if (_distV4 <= _distV3)
								{
									SpawnObjectOnFinalPathDash(hitFourth.transform);
								}
							}
							else if (hitFourth.collider == null)
							{
								_distV3 = _distanceDisplayDash;
							}
						}

						if (Physics.Raycast(displayPower[i].raycastTransform[0].position, Vector3.down, out var hitThird, _distanceDisplayPower,
							layerInteractable)) // launch the raycast
						{
							var distV3 = Vector3.Distance(displayPower[i].raycastTransform[0].position, hitThird.transform.position);
							_distV3 = distV3;
						}

						if (Physics.Raycast(raycastPlayer.position, Vector3.down, out var hitPlayerTwo, _distanceDisplayPower,
							layerInteractable)) // launch the raycast
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

			if (Physics.Raycast(ray, out var hitShowPath, Mathf.Infinity, layerShowPath))
			{
				var playerPos = GameManager.Instance.currentPlayerTurn.transform.position;
				var hitInfoPos = hitShowPath.collider.transform.position;

				if (playerPos.x < hitInfoPos.x && Math.Abs(playerPos.z - hitInfoPos.z) < 0.1f)
				{
					MirrorDirection(2); // Right
				}

				if (playerPos.x > hitInfoPos.x && Math.Abs(playerPos.z - hitInfoPos.z) < 0.1f)
				{
					MirrorDirection(3); // Left
				}

				if (playerPos.z > hitInfoPos.z && Math.Abs(playerPos.x - hitInfoPos.x) < 0.1f)
				{
					MirrorDirection(0); // Down
				}

				if (playerPos.z < hitInfoPos.z && Math.Abs(playerPos.x - hitInfoPos.x) < 0.1f)
				{
					MirrorDirection(1); // Up
				}

				ActiveParticle();
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
				case 1:
					switch (endDirection)
					{
						case SwipeGestureRecognizerDirection.Down:
							MirrorDirection(0);
							break;
						case SwipeGestureRecognizerDirection.Up:
							MirrorDirection(1);
							break;
						case SwipeGestureRecognizerDirection.Right:
							MirrorDirection(2);
							break;
						case SwipeGestureRecognizerDirection.Left:
							MirrorDirection(3);
							break;
					}

					break;
				case 2:
					switch (endDirection)
					{
						case SwipeGestureRecognizerDirection.Down:
							MirrorDirection(0);
							break;
						case SwipeGestureRecognizerDirection.Up:
							MirrorDirection(1);
							break;
						case SwipeGestureRecognizerDirection.Right:
							MirrorDirection(2);
							break;
						case SwipeGestureRecognizerDirection.Left:
							MirrorDirection(3);
							break;
					}

					break;
				case 3:
					switch (endDirection)
					{
						case SwipeGestureRecognizerDirection.Down:
							MirrorDirection(1);
							break;
						case SwipeGestureRecognizerDirection.Up:
							MirrorDirection(0);
							break;
						case SwipeGestureRecognizerDirection.Right:
							MirrorDirection(3);
							break;
						case SwipeGestureRecognizerDirection.Left:
							MirrorDirection(2);
							break;
					}

					break;
				case 4:
					switch (endDirection)
					{
						case SwipeGestureRecognizerDirection.Down:
							MirrorDirection(1);
							break;
						case SwipeGestureRecognizerDirection.Up:
							MirrorDirection(0);
							break;
						case SwipeGestureRecognizerDirection.Right:
							MirrorDirection(3);
							break;
						case SwipeGestureRecognizerDirection.Left:
							MirrorDirection(2);
							break;
					}

					break;
			}
		}
	}

	#endregion

	#region SwipeMirorDirection

	private void MirrorDirection(int directionIndex) // When we clicked on button
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
		NFCManager.Instance.powerActivated = true;
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

	#endregion

	#region DisplayPower

	public void DisplayPower() // Display the zone who the players can swipe
	{
		transform.position = GameManager.Instance.currentPlayerTurn.transform.position;

		players = Physics.OverlapSphere(transform.position, rangeDetectionPlayer, layerPlayer);

		for (int i = 0; i < players.Length; i++)
		{
			if (players[i].name != GameManager.Instance.currentPlayerTurn.name)
			{
				Transform child = players[i].transform.GetChild(1);

				var color = child.GetComponentInChildren<Renderer>().material.color;
				color = thirdMat.color;
				child.GetComponentInChildren<Renderer>().material.color = color;
			}
		}

		if (players.Length > 1)
		{
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
		else
		{
			switch (GameManager.Instance.actualCamPreset.presetNumber)
			{
				case 1:
					textWhenThereAreNoZombieAround[0].gameObject.SetActive(true);
					break;
				case 2:
					textWhenThereAreNoZombieAround[0].gameObject.SetActive(true);
					break;
				case 3:
					textWhenThereAreNoZombieAround[1].gameObject.SetActive(true);
					break;
				case 4:
					textWhenThereAreNoZombieAround[1].gameObject.SetActive(true);
					break;
			}
		}
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

	#region CHANGE PLAYER MATERIAL

	private void ChangePlayerMaterial(Transform objectToChange, Material mat) // Change the material of the object
	{
		var color = objectToChange.GetComponent<Renderer>().materials[2].GetColor("_EmissionColor");
		color = mat.color;
		objectToChange.GetComponent<Renderer>().materials[2].SetColor("_EmissionColor", color);
	}

	#endregion

	private void ActiveParticle()
	{
		var playerTransform = GameManager.Instance.currentPlayerTurn.transform;
		_particleToDeactivate = PoolManager.Instance.SpawnObjectFromPool("ParticleMirror", playerTransform.position, playerTransform.rotation, playerTransform);
	}
	
	IEnumerator CoroutineDeactivateParticle()
	{
		yield return new WaitForSeconds(0.1f);
		
		_particleToDeactivate.SetActive(false);
		
		for (int i = 0; i < hitTransforms.Count; i++)
		{
			hitTransforms[i].GetComponent<Renderer>().materials[2].SetColor("_EmissionColor", firstMat.color);
		}

		foreach (var p in players)
		{
			Transform child = p.transform.GetChild(1);
			child.GetComponentInChildren<Renderer>().material.color = zombieMat.color;
		}

		foreach (var g in textWhenThereAreNoZombieAround)
		{
			g.gameObject.SetActive(false);
		}

		zombiePlayer = null;
		hitTransforms.Clear();
		foreach (var g in listObjectToSetActiveFalse)
		{
			g.SetActive(false);
		}

		listObjectToSetActiveFalse.Clear();

		PowerManager.Instance.ActivateDeactivatePower(3, false);
		PowerManager.Instance.ChangeTurnPlayer();
	}
	
	public void CancelPower()
	{
	}

	public void DoPower()
	{
	}

	public void ClearPower() // Clear the power
	{
		StartCoroutine(CoroutineDeactivateParticle());
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