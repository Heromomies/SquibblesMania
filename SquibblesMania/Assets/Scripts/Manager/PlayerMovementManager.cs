using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DigitalRubyShared;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovementManager : MonoBehaviour
{
	public LongPressGestureRecognizer LongPressBlocMovementGesture { get; private set; }

	[Header("TOUCH PARAMETERS")] public LayerMask touchLayerMask;
	public LayerMask blocLayerMask;
	[Range(1, 10)] public int swipeTouchCount = 1;
	[Range(0.0f, 10.0f)] public float swipeThresholdSeconds;
	public GameObject blocMovementManager;
	[Range(0.0f, 1.0f)] public float minimumDistanceUnits;
	
	[Header("Player PARAMETERS")] public List<Transform> previewPath = new List<Transform>();
	public List<GameObject> sphereList = new List<GameObject>();
	public GameObject playerCurrentlySelected;
	public GameObject ghostPlayer;
	public float raycastDistance;

	private readonly List<RaycastResult> _raycast = new List<RaycastResult>();
	public SwipeGestureRecognizer swipe;
	private Camera _cam;
	private RaycastHit _hit;
	private readonly List<Vector3> _directionPlayer = new List<Vector3> {Vector3.back, Vector3.forward, Vector3.right, Vector3.left};

	private readonly List<Vector3> _directionRaycast = new List<Vector3>
		{new Vector3(0, -0.5f, -1), new Vector3(0, -0.5f, 1), new Vector3(1, -0.5f, 0), new Vector3(-1, -0.5f, 0)};

	private readonly WaitForSeconds _timeBetweenPlayerMovement = new WaitForSeconds(0.8f);
	private readonly WaitForSeconds _timeBetweenDeactivateSphere = new WaitForSeconds(0.01f);
	private readonly WaitForSeconds _timeBetweenReloadPath = new WaitForSeconds(0.2f);

	#region Singleton

	private static PlayerMovementManager playerMovementManager;

	public static PlayerMovementManager Instance => playerMovementManager;

	// Start is called before the first frame update

	private void Awake()
	{
		playerMovementManager = this;
		_cam = Camera.main;
	}

	#endregion

	private void OnEnable()
	{
		//Set up the new gesture 
		swipe = new SwipeGestureRecognizer();
		swipe.StateUpdated += SwipeUpdated;
		swipe.DirectionThreshold = 0;
		swipe.MinimumNumberOfTouchesToTrack = swipe.MaximumNumberOfTouchesToTrack = swipeTouchCount;
		swipe.ThresholdSeconds = swipeThresholdSeconds;
		swipe.MinimumDistanceUnits = minimumDistanceUnits;
		swipe.EndMode = SwipeGestureRecognizerEndMode.EndContinusously;
		FingersScript.Instance.AddGesture(swipe);

		//Set up the new gesture 
		LongPressBlocMovementGesture = new LongPressGestureRecognizer();
		LongPressBlocMovementGesture.StateUpdated += LongPressBlocMovementGestureOnStateUpdated;
		LongPressBlocMovementGesture.ThresholdUnits = 0.0f;
		LongPressBlocMovementGesture.MinimumDurationSeconds = 0.1f;
		LongPressBlocMovementGesture.AllowSimultaneousExecution(swipe);
		FingersScript.Instance.AddGesture(LongPressBlocMovementGesture);
		
		GameObject gPlayer = Instantiate(ghostPlayer, transform.position, Quaternion.identity);
		ghostPlayer = gPlayer;
		ghostPlayer.SetActive(false);
	}

	private void SwipeUpdated(GestureRecognizer gesture) // When we swipe
	{
		SwipeGestureRecognizer swipe = gesture as SwipeGestureRecognizer;
		if (swipe.State == GestureRecognizerState.Ended && playerCurrentlySelected != null)
		{
			switch (GameManager.Instance.actualCamPreset.presetNumber)
			{
				case 1:
					switch (swipe.EndDirection)
					{
						case SwipeGestureRecognizerDirection.Down: StartCoroutine(StartPlayerMovement(0)); break;
						case SwipeGestureRecognizerDirection.Up: StartCoroutine(StartPlayerMovement(1)); break;
						case SwipeGestureRecognizerDirection.Right: StartCoroutine(StartPlayerMovement(2)); break;
						case SwipeGestureRecognizerDirection.Left: StartCoroutine(StartPlayerMovement(3)); break;
					} break;
				case 3 :
					switch (swipe.EndDirection)
					{
						case SwipeGestureRecognizerDirection.Down: StartCoroutine(StartPlayerMovement(0)); break;
						case SwipeGestureRecognizerDirection.Up: StartCoroutine(StartPlayerMovement(1)); break;
						case SwipeGestureRecognizerDirection.Right: StartCoroutine(StartPlayerMovement(2)); break;
						case SwipeGestureRecognizerDirection.Left: StartCoroutine(StartPlayerMovement(3)); break;
					} break;
				case 2:
					switch (swipe.EndDirection)
					{
						case SwipeGestureRecognizerDirection.Down: StartCoroutine(StartPlayerMovement(1)); break;
						case SwipeGestureRecognizerDirection.Up: StartCoroutine(StartPlayerMovement(0)); break; 
						case SwipeGestureRecognizerDirection.Right: StartCoroutine(StartPlayerMovement(3)); break; 
						case SwipeGestureRecognizerDirection.Left: StartCoroutine(StartPlayerMovement(2)); break;
					} break;
				case 4:
					switch (swipe.EndDirection)
					{
						case SwipeGestureRecognizerDirection.Down: StartCoroutine(StartPlayerMovement(1)); break;
						case SwipeGestureRecognizerDirection.Up: StartCoroutine(StartPlayerMovement(0)); break;
						case SwipeGestureRecognizerDirection.Right: StartCoroutine(StartPlayerMovement(3)); break;
						case SwipeGestureRecognizerDirection.Left: StartCoroutine(StartPlayerMovement(2)); break;
					} break;
			}
		}
	}

	private void OnDisable()
	{
		if (FingersScript.HasInstance)
		{
			//FingersScript.Instance.RemoveGesture(longPress);
		}
	}

	//Update method of the long press gesture
	private void LongPressBlocMovementGestureOnStateUpdated(GestureRecognizer gesture)
	{
		if (GameManager.Instance.currentPlayerTurn.playerActionPoint > 0)
		{
			if (gesture.State == GestureRecognizerState.Began)
			{
				PointerEventData p = new PointerEventData(EventSystem.current);
				p.position = new Vector2(gesture.FocusX, gesture.FocusY);

				_raycast.Clear();
				EventSystem.current.RaycastAll(p, _raycast);
				// Cast a ray from the camera
				Ray ray = _cam.ScreenPointToRay(p.position);

				if (Physics.Raycast(ray, out _hit, Mathf.Infinity, touchLayerMask))
				{
					if (_hit.collider.name == GameManager.Instance.currentPlayerTurn.name)
					{
						ghostPlayer.SetActive(true);
						playerCurrentlySelected = _hit.transform.gameObject;
						var hitObj = _hit.transform.position;
						ghostPlayer.transform.position = new Vector3(hitObj.x, hitObj.y - 0.5f, hitObj.z);
						
						playerCurrentlySelected = ghostPlayer;
						
						blocMovementManager.SetActive(false);

						GameManager.Instance.currentPlayerTurn.playerActionPoint--;

						var cBlockPlayerOn = GameManager.Instance.currentPlayerTurn.currentBlockPlayerOn;

						if (!previewPath.Contains(cBlockPlayerOn))
						{
							previewPath.Add(cBlockPlayerOn);

							GameManager.Instance.currentPlayerTurn.playerActionPoint++;
						}
					}
				}
			}
			else if (gesture.State == GestureRecognizerState.Ended && playerCurrentlySelected != null)
			{
				ClearListAfterRelease();
			}
		}
		else if (gesture.State == GestureRecognizerState.Ended && playerCurrentlySelected != null)
		{
			ClearListAfterRelease();

			if (GameManager.Instance.currentPlayerTurn.playerActionPoint <= 0)
			{
				UiManager.Instance.buttonNextTurn.SetActive(true);
			}
		}
	}

	private void ClearListAfterRelease() // Clear the list after the player released the Squeeples
	{
		GameManager.Instance.currentPlayerTurn.nextBlockPath = previewPath;
		
		for (int i = 0; i < sphereList.Count; i++)
		{
			sphereList[i].SetActive(false);
		}
		
		GameManager.Instance.currentPlayerTurn.currentTouchBlock = ghostPlayer.GetComponent<CheckUnderGhost>().currentBlockGhostOn;
		GameManager.Instance.currentPlayerTurn.StartPathFinding();

		StartCoroutine(ResetPreviewPlatform());
	}

	private IEnumerator StartPlayerMovement(int direction) // Depends on the position the player wants to go, he moves in the wished direction
	{
		switch (direction)
		{
			case 0: PreviewPath(0); break;
			case 1: PreviewPath(1); break;
			case 2: PreviewPath(2); break;
			case 3: PreviewPath(3); break;
		}

		yield return _timeBetweenPlayerMovement;
	}

	private void PreviewPath(int value) // Move the player 
	{
		if (Physics.Raycast(ghostPlayer.transform.position, _directionRaycast[value], out var hit, raycastDistance, blocLayerMask))
		{
			if (Math.Abs(hit.transform.position.y - GameManager.Instance.currentPlayerTurn.currentBlockPlayerOn.position.y) < 0.1f)
			{
				var positionList = previewPath.IndexOf(hit.transform);
				if (GameManager.Instance.currentPlayerTurn.playerActionPoint > 0)
				{
					if (!previewPath.Contains(hit.transform) || previewPath.Count - 1 == positionList + 1)
					{
						GameManager.Instance.currentPlayerTurn.playerActionPoint--;
						ghostPlayer.transform.position += _directionPlayer[value];

						StartCoroutine(WaitBeforeCheckUnderPlayer());
					}
				}
				else if (previewPath.Count - 1 == positionList + 1)
				{
					GameManager.Instance.currentPlayerTurn.playerActionPoint--;
					ghostPlayer.transform.position += _directionPlayer[value];

					StartCoroutine(WaitBeforeCheckUnderPlayer());
				}
			}
		}

		UiManager.Instance.SetUpCurrentActionPointOfCurrentPlayer(GameManager.Instance.currentPlayerTurn.playerActionPoint);
	}

	private IEnumerator WaitBeforeCheckUnderPlayer() // Check the block under the ghost
	{
		yield return _timeBetweenDeactivateSphere;

		ghostPlayer.GetComponent<CheckUnderGhost>().GhostMoved();

		yield return _timeBetweenDeactivateSphere;

		var cBlockGhostOn = ghostPlayer.GetComponent<CheckUnderGhost>().currentBlockGhostOn;
		var pCount = previewPath.Count - 1;
		var pCountPos = previewPath[pCount].position;
		var positionList = previewPath.IndexOf(cBlockGhostOn);

		if (!previewPath.Contains(cBlockGhostOn) && cBlockGhostOn != previewPath[pCount])
		{
			previewPath.Add(cBlockGhostOn);

			LaunchBullet(pCountPos);
		}
		else
		{
			previewPath.Remove(previewPath[positionList + 1]);
		}
	}

	private void LaunchBullet(Vector3 positionToInstantiate) // Launch bullet 
	{
		GameObject sphere = PoolManager.Instance.SpawnObjectFromPool(
			"SphereShowPath", new Vector3(positionToInstantiate.x, positionToInstantiate.y + 1.2f, positionToInstantiate.z), Quaternion.identity,
			null);

		sphereList.Add(sphere);
	}
	
	IEnumerator ResetPreviewPlatform()
	{
		yield return _timeBetweenReloadPath;
		
		if (!GameManager.Instance.currentPlayerTurn.walking)
		{
			var player = GameManager.Instance.currentPlayerTurn;
			player.PlayerActionPointCardState.SetFalsePathObjects();
			player.PlayerActionPointCardState.PreviewPath(player.playerActionPoint, player);
		}
	}
}