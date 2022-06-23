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
	
	[HideInInspector] public GameObject zombiePlayer;
	[Space(25)] public List<TextMeshProUGUI> textWhenNoZombieAreSelected;
	[Space(25)] public List<TextMeshProUGUI> textWhenThereAreNoZombieAround;
	[Space(25)] public List<GameObject> listObjectToSetActiveFalse;

	public Transform baseSpawnRaycastTransform;
	public Transform raycastPlayer;
	[Space] [Range(1, 10)] public int mirrorRange;
	[Range(0.0f, 1f)] public float mirrorPlayerSpeed;
	[Range(0.0f, 1f)] public float mirrorZombieSpeed;
	
	[Header("MATERIAL SETTINGS")] 
	[Space] 
	public Material selectableMat;
	[Space]
	public Material changeZombieMat;

	public Collider[] players;
	private WaitForSeconds _waitParticles = new WaitForSeconds(0.1f);
	private WaitForSeconds _waitDetectBlocUnderZombie = new WaitForSeconds(1f);
	private readonly WaitForSeconds _timeBetweenPlayerZombieMovement = new WaitForSeconds(0.3f);
	private readonly List<Vector3> _vectorRaycast = new List<Vector3> {Vector3.back, Vector3.forward, Vector3.right, Vector3.left};

	private readonly List<RaycastResult> _raycast = new List<RaycastResult>();
	private PanGestureRecognizer SwapTouchGesture { get; set; }

	private Camera _cam;
	private int _distanceDisplayPower = 10;
	private int _distanceDisplayDash = 3;
	private float _distV2, _distV3, _distV4;
	private GameObject _particleToDeactivate, _particleOnPutCard;
	private GameObject _particleToDeactivateZombie;
	private Vector3 _heightWhenDash = new Vector3(0, 0.5f, 0);
	private Vector3 _vectorCard = new Vector3(0,-0.45f,0);
	
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
		SwapTouchGesture = new PanGestureRecognizer();
		SwapTouchGesture.ThresholdUnits = 0.0f; // start right away
		//Add new gesture
		SwapTouchGesture.StateUpdated += PlayerTouchGestureUpdated;
		SwapTouchGesture.AllowSimultaneousExecutionWithAllGestures();

		FingersScript.Instance.AddGesture(SwapTouchGesture);
		
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

			_raycast.Clear();
			EventSystem.current.RaycastAll(p, _raycast);

			Ray ray = _cam.ScreenPointToRay(p.position);

			if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, layerPlayer))
			{
				if (players.ToList().Contains(hitInfo.collider) && hitInfo.transform.name != GameManager.Instance.currentPlayerTurn.name)
				{
					zombiePlayer = hitInfo.collider.gameObject;
					_particleToDeactivateZombie = PoolManager.Instance.SpawnObjectFromPool("ZombieParticle", zombiePlayer.transform.position, Quaternion.identity, zombiePlayer.transform);
					
					var posPlayer = GameManager.Instance.currentPlayerTurn.transform.position;
					baseSpawnRaycastTransform.position = new Vector3(posPlayer.x, posPlayer.y + _distanceDisplayDash, posPlayer.z);
					raycastPlayer.position = baseSpawnRaycastTransform.position;

					if(players.Length > 0)
					{
						foreach (var pl in players)
						{
							GameManager.Instance.SetUpPlayerMaterial(pl.GetComponent<PlayerStateManager>(), pl.GetComponent<PlayerStateManager>().playerNumber);
						}
					}

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
				PowerManager.Instance.ResetPollingAndLights();

				var playerPos = GameManager.Instance.currentPlayerTurn.transform.position;
				var hitInfoPos = hitShowPath.collider.transform.position;
				var quat = Quaternion.Euler(0,0,0);

				if (_particleOnPutCard != null)
				{
					_particleOnPutCard.SetActive(false);
					_particleOnPutCard = null;
				}
				
				if (playerPos.x < hitInfoPos.x && Math.Abs(playerPos.z - hitInfoPos.z) < 0.1f)
				{
					MirrorDirection(2, hitInfoPos); // Right
					quat = Quaternion.Euler(0,90f,0);
					GameManager.Instance.currentPlayerTurn.gameObject.transform.rotation = quat;
				}

				if (playerPos.x > hitInfoPos.x && Math.Abs(playerPos.z - hitInfoPos.z) < 0.1f)
				{
					MirrorDirection(3, hitInfoPos); // Left
					quat = Quaternion.Euler(0,-90f,0);
					GameManager.Instance.currentPlayerTurn.gameObject.transform.rotation = quat;
				}

				if (playerPos.z > hitInfoPos.z && Math.Abs(playerPos.x - hitInfoPos.x) < 0.1f)
				{
					MirrorDirection(0, hitInfoPos); // Down
					quat = Quaternion.Euler(0,180f,0);
					GameManager.Instance.currentPlayerTurn.gameObject.transform.rotation = quat;
				}

				if (playerPos.z < hitInfoPos.z && Math.Abs(playerPos.x - hitInfoPos.x) < 0.1f)
				{
					MirrorDirection(1, hitInfoPos); // Up
					quat = Quaternion.Euler(0,0f,0);
					GameManager.Instance.currentPlayerTurn.gameObject.transform.rotation = quat;
				}

				ActiveParticle();
			}
		}
	}

	#endregion

	#region MirorDirection

	private void MirrorDirection(int directionIndex, Vector3 posToGo) // When we clicked on button
	{
		var currentPlayer = GameManager.Instance.currentPlayerTurn;
		var currentPlayerTransform = currentPlayer.transform;
		var playerPos = currentPlayerTransform.position;
		transform.position = playerPos;

		if (Physics.Raycast(transform.position, _vectorRaycast[directionIndex], out var hit, mirrorRange)) // launch the raycast
		{
			if (hit.collider.gameObject.layer == 3 || hit.collider.gameObject.layer == 0 || hit.collider.gameObject.layer == 15)
			{
				var distance = Vector3.Distance(playerPos, hit.collider.transform.position);
				distance = (int) distance;

				if (distance <= 3.5f)
				{
					GameManager.Instance.currentPlayerTurn.transform.DOMove(
						posToGo +_heightWhenDash, mirrorPlayerSpeed);
				}
			}
			else if (hit.collider.gameObject.layer == 6) // When the raycast touch another player
			{
				var distanceBetweenTwoPlayers = Vector3.Distance(playerPos, hit.collider.transform.position);
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
					AudioManager.Instance.Play("PlayerHit");
					
					var distanceBetweenBlockAndPlayerTouched = Vector3.Distance(hit.transform.position,
						hitPlayerTouched.transform.position);
					distanceBetweenBlockAndPlayerTouched += 0.1f;
					distanceBetweenBlockAndPlayerTouched = (int) distanceBetweenBlockAndPlayerTouched; //Check distance between himself and the block behind him

					var distanceBetweenTwoPlayersWhenABlockIsBehind = Vector3.Distance(playerPos, hit.collider.transform.position);
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
									playerPos + _vectorRaycast[directionIndex] *
									(distanceBetweenTwoPlayers + distanceBetweenBlockAndPlayerTouched - 2), mirrorPlayerSpeed);
								break;
							case 3:
								GameManager.Instance.currentPlayerTurn.transform.DOMove(
									playerPos + _vectorRaycast[directionIndex] *
									(distanceBetweenTwoPlayers - 1), mirrorPlayerSpeed);
								break;
						}

						//In any case, the player repulsed will stop his course before the bloc who stop him
						hit.collider.transform.DOMove(hit.collider.transform.position
						                              + _vectorRaycast[directionIndex] * (distanceBetweenBlockAndPlayerTouched - 1), mirrorPlayerSpeed);
					}
				}
				else // If the player repulsed don't have any bloc behind him, the player who dash just dash and repulse from 1 the player
				{
					GameManager.Instance.currentPlayerTurn.transform.DOMove(
						playerPos + _vectorRaycast[directionIndex] * mirrorRange, mirrorPlayerSpeed);
					hit.collider.transform.DOMove(hit.collider.transform.position
					                              + _vectorRaycast[directionIndex] * distanceBetweenTwoPlayers, mirrorPlayerSpeed);
				}
			}
			else if (hit.collider.gameObject.layer == 0)
			{
				GameManager.Instance.currentPlayerTurn.transform.DOMove(
					posToGo +_heightWhenDash, mirrorPlayerSpeed);
			}
		}
		else // If they are no bloc or players on his path, dash from 3
		{
			GameManager.Instance.currentPlayerTurn.transform.DOMove(
				posToGo +_heightWhenDash, mirrorPlayerSpeed);
		}

		AudioManager.Instance.Play("PowerMirorEnd");
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

		if (Physics.Raycast(transform.position, -_vectorRaycast[directionZombieIndex], out var hitZombie, mirrorRange)) // launch the raycast
		{
			if (hitZombie.collider.gameObject.layer == 3 || hitZombie.collider.gameObject.layer == 0)
			{
				var distance = Vector3.Distance(positionZombiePlayer, hitZombie.collider.transform.position);
				distance = (int) distance;

				if (distance <= 3.5f)
				{
					zombiePlayer.transform.DOMove(
						positionZombiePlayer - _vectorRaycast[directionZombieIndex] * (distance - 1), mirrorZombieSpeed);
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
						distanceBetweenTwoPlayers = 2;
						break;
					case 2:
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
								distanceBetweenTwoPlayersWhenABlockIsBehind = 2;
								break;
							case 2:
								distanceBetweenTwoPlayersWhenABlockIsBehind = 1;
								break;
						}

						switch (distanceBetweenTwoPlayersWhenABlockIsBehind) // according to the distance between the two players, the dash is not the same
						{
							case 2:
								zombiePlayer.transform.DOMove(
									positionZombiePlayer - _vectorRaycast[directionZombieIndex] *
									(distanceBetweenTwoPlayers + distanceBetweenBlockAndPlayerTouched - 2), mirrorZombieSpeed);
								break;
							case 3:
								zombiePlayer.transform.DOMove(
									positionZombiePlayer - _vectorRaycast[directionZombieIndex] *
									(distanceBetweenTwoPlayers - 1), mirrorZombieSpeed);
								break;
						}

						//In any case, the player repulsed will stop his course before the bloc who stop him
						hitZombie.collider.transform.DOMove(hitZombie.collider.transform.position
						                                    - _vectorRaycast[directionZombieIndex] * (distanceBetweenBlockAndPlayerTouched - 1), mirrorZombieSpeed);
					}
				}
				else // If the player repulsed don't have any bloc behind him, the player who dash just dash and repulse from 1 the player
				{
					zombiePlayer.transform.DOMove(
						positionZombiePlayer - _vectorRaycast[directionZombieIndex] * mirrorRange, mirrorZombieSpeed);
					hitZombie.collider.transform.DOMove(hitZombie.collider.transform.position
					                                    - _vectorRaycast[directionZombieIndex] * distanceBetweenTwoPlayers, mirrorZombieSpeed);
				}
			}
			else if (hitZombie.collider.gameObject.layer == 0)
			{
				zombiePlayer.transform.DOMove(
					positionZombiePlayer - _vectorRaycast[directionZombieIndex] * mirrorRange, mirrorZombieSpeed);
			}
		}
		else // If they are no bloc or players on his path, dash from 3
		{
			zombiePlayer.transform.DOMove(
				positionZombiePlayer - _vectorRaycast[directionZombieIndex] * mirrorRange, mirrorZombieSpeed);
		}

		AudioManager.Instance.Play("PowerMirorEnd");
		
		ClearPower();
	}

	#endregion

	#region DisplayPower

	public void DisplayPower() // Display the zone who the players can swipe
	{
		players = null;

		var t = transform;
		var posPlayer = GameManager.Instance.currentPlayerTurn.transform.position;
		t.position = posPlayer;

		_particleOnPutCard = PoolManager.Instance.SpawnObjectFromPool("ParticleDisplayPowerMirror", posPlayer + _vectorCard, Quaternion.Euler(-90f,0,0), null);

		
		// ReSharper disable once Unity.PreferNonAllocApi
		players = Physics.OverlapSphere(t.position, rangeDetectionPlayer, layerPlayer);

		for (int i = 0; i < players.Length; i++)
		{
			if (players[i].name != GameManager.Instance.currentPlayerTurn.name && players.Length > 1)
			{
				players[i].GetComponent<PlayerStateManager>().playerMesh.GetComponent<Renderer>().material = selectableMat;
			}
		}

		if (players.Length > 1)
		{
			switch (GameManager.Instance.actualCamPreset.presetNumber)
			{
				case 1: textWhenNoZombieAreSelected[0].gameObject.SetActive(true); break;
				case 2: textWhenNoZombieAreSelected[0].gameObject.SetActive(true); break;
				case 3: textWhenNoZombieAreSelected[1].gameObject.SetActive(true); break;
				case 4: textWhenNoZombieAreSelected[1].gameObject.SetActive(true); break;
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
		if (objectToChange != null)
		{
			var objPos = objectToChange.position;
			var objRot = objectToChange.localPosition;

			GameObject obj = PoolManager.Instance.SpawnObjectFromPool("ShaderPlanePower",
				new Vector3(objPos.x, objPos.y + 1.02f, objPos.z), Quaternion.Euler(objRot.x, position, objRot.z), null);

			listObjectToSetActiveFalse.Add(obj);
		}
	}

	#endregion

	private void ActiveParticle()
	{
		var playerTransform = GameManager.Instance.currentPlayerTurn.transform;
		_particleToDeactivate = PoolManager.Instance.SpawnObjectFromPool("ParticleMirror", playerTransform.position, playerTransform.rotation, playerTransform);
	}
	
	IEnumerator CoroutineDeactivateParticle()
	{
		yield return _waitParticles;

		if (_particleToDeactivate != null)
		{
			_particleToDeactivate.SetActive(false);
		}

		if (_particleToDeactivateZombie != null)
		{
			_particleToDeactivateZombie.SetActive(false);
		}
		

		for (int i = 0; i < textWhenNoZombieAreSelected.Count; i++)
		{
			textWhenNoZombieAreSelected[i].gameObject.SetActive(false);
		}

		foreach (var g in textWhenThereAreNoZombieAround)
		{
			g.gameObject.SetActive(false);
		}

		StartCoroutine(WaitBeforeDetectUnderZombie());
		
		foreach (var g in listObjectToSetActiveFalse)
		{
			g.SetActive(false);
		}

		listObjectToSetActiveFalse.Clear();
	}

	IEnumerator WaitBeforeDetectUnderZombie()
	{
		yield return _waitDetectBlocUnderZombie;
		
		zombiePlayer = null;

		GameManager.Instance.DetectParentBelowPlayers();
		GameManager.Instance.DetectParentBelowPlayers();
		
		PowerManager.Instance.ActivateDeactivatePower(3, false);
	}
	
	public void ClearPower() // Clear the power
	{
		if(players.Length > 0)
		{
			foreach (var p in players)
			{
				GameManager.Instance.SetUpPlayerMaterial(p.GetComponent<PlayerStateManager>(), p.GetComponent<PlayerStateManager>().playerNumber);
			}
		}
		
		if (_particleOnPutCard != null)
		{
			_particleOnPutCard.SetActive(false);
			_particleOnPutCard = null;
		}
		
		if (NFCManager.Instance.powerActivated)
		{
			GameManager.Instance.PlayerMoving();
			StartCoroutine(CoroutineDeactivateParticle());
		}
		else
		{
			for (int i = 0; i < textWhenNoZombieAreSelected.Count; i++)
			{
				textWhenNoZombieAreSelected[i].gameObject.SetActive(false);
			}

			if (_particleToDeactivateZombie != null)
			{
				_particleToDeactivateZombie.SetActive(false);
			}
			
			foreach (var g in textWhenThereAreNoZombieAround)
			{
				g.gameObject.SetActive(false);
			}
			
			foreach (var g in listObjectToSetActiveFalse)
			{
				g.SetActive(false);
			}
			
			listObjectToSetActiveFalse.Clear();

			PowerManager.Instance.ActivateDeactivatePower(3, false);
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