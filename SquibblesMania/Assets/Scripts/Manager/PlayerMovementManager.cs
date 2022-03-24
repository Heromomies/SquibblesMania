using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DigitalRubyShared;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovementManager : MonoBehaviour
{
	public LongPressGestureRecognizer LongPressBlocMovementGesture { get; private set; }

	[Header("TOUCH SETTINGS")] public LayerMask touchLayerMask;
	public LayerMask blocLayerMask;
	[Range(1, 10)] public int swipeTouchCount = 1;
	[Range(0.0f, 10.0f)] public float swipeThresholdSeconds;
	[Range(0.0f, 1.0f)] public float minimumDistanceUnits;
	[Range(0.0f, 1.0f)] public float longPressureDurationSeconds;
	[Range(0.0f, 0.1f)] public float timeLeftBetweenSwipe;

	[HideInInspector] public List<Transform> previewPath = new List<Transform>();
	[HideInInspector] public List<GameObject> sphereList = new List<GameObject>();
	[HideInInspector] public GameObject playerCurrentlySelected;

	[Header("PLAYER SETTINGS")] public GameObject ghostPlayer;
	public float raycastDistance;

	private Vector3 _touchPos;
	private Camera _cam;
	private RaycastHit _hit;
	[HideInInspector] public bool hasStopMovingBloc;

	[SerializeField] private Transform _blockParentCurrentlySelected;

	private GameObject _blockCurrentlySelected;

	[Header("BLOC SETTINGS")] [SerializeField]
	private float movementBlocAmount = 1f;

	private Vector3 _blocParentCurrentlySelectedPos; 
	private bool _isBlocSelected;
	private Vector3 _lastDirectionBloc;
	private float _timeInSecondsForBlocMove = 0.4f;
	private readonly WaitForSeconds _timeInSecondsBetweenBlocMovement = new WaitForSeconds(0.4f);

	[HideInInspector] public int totalCurrentActionPoint;
	private GameObject _textActionPointPopUp;

	[Header("POP UP TEXT PARAMETERS")] [SerializeField]
	private Vector3 offsetText;

	private readonly List<Transform> _nextBlocUpMeshPos = new List<Transform>();
	private readonly List<Transform> _nextBlocDownMeshPos = new List<Transform>();

	private bool _canTouchBloc = true;
	private float _timeLeftMax;
	private Vector3 _ghostVector;
	private readonly List<RaycastResult> _raycast = new List<RaycastResult>();
	public SwipeGestureRecognizer swipe;
	private readonly List<Vector3> _directionPlayer = new List<Vector3> {Vector3.back, Vector3.forward, Vector3.right, Vector3.left};

	private readonly List<Vector3> _directionRaycast = new List<Vector3>
		{new Vector3(0, -0.5f, -1), new Vector3(0, -0.5f, 1), new Vector3(1, -0.5f, 0), new Vector3(-1, -0.5f, 0)};

	private readonly WaitForSeconds _timeBetweenPlayerMovement = new WaitForSeconds(0.3f);
	private readonly WaitForSeconds _timeBetweenDeactivateSphere = new WaitForSeconds(0.001f);
	private readonly WaitForSeconds _timeBetweenReloadPath = new WaitForSeconds(0.1f);

	public bool hasMoved;
	private Vector2 _focus, _startFocus;
	public float offset;

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

	/// <summary>
	/// // Start and enable gesture
	/// </summary>

	#region Start

	private void Start()
	{
		_timeLeftMax = timeLeftBetweenSwipe;

		//Set up the new gesture 
		swipe = new SwipeGestureRecognizer();
		swipe.StateUpdated += SwipeUpdated;
		swipe.DirectionThreshold = 0;
		swipe.MinimumNumberOfTouchesToTrack = swipe.MaximumNumberOfTouchesToTrack = swipeTouchCount;
		swipe.ThresholdSeconds = swipeThresholdSeconds;
		swipe.MinimumDistanceUnits = minimumDistanceUnits;
		swipe.EndMode = SwipeGestureRecognizerEndMode.EndContinusously;
		swipe.AllowSimultaneousExecution(LongPressBlocMovementGesture);
		FingersScript.Instance.AddGesture(swipe);

		//Set up the new gesture 
		LongPressBlocMovementGesture = new LongPressGestureRecognizer();
		LongPressBlocMovementGesture.StateUpdated += LongPressBlocMovementGestureOnStateUpdated;
		//LongPressBlocMovementGesture.ThresholdUnits = 0.0f;
		LongPressBlocMovementGesture.MinimumDurationSeconds = longPressureDurationSeconds;
		LongPressBlocMovementGesture.AllowSimultaneousExecutionWithAllGestures();
		FingersScript.Instance.AddGesture(LongPressBlocMovementGesture);

		GameObject gPlayer = Instantiate(ghostPlayer, transform.position, Quaternion.identity);
		ghostPlayer = gPlayer;
		ghostPlayer.SetActive(false);
	}

	#endregion

	/// <summary>
	/// OnDisable remove gesture
	/// </summary>

	#region OnDisable

	private void OnDisable()
	{
		if (FingersScript.HasInstance)
		{
			FingersScript.Instance.RemoveGesture(LongPressBlocMovementGesture);
			FingersScript.Instance.RemoveGesture(swipe);
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

		var offsetCamera = _cam.transform.eulerAngles.y - offset;
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

	/// <summary>
	/// Function for the swipe gesture
	/// </summary>

	#region SwipeUpdated

	private void SwipeUpdated(GestureRecognizer gesture) // When we swipe
	{
		SwipeGestureRecognizer swipeGestureRecognizer = gesture as SwipeGestureRecognizer;
		if (swipeGestureRecognizer.State == GestureRecognizerState.Ended && playerCurrentlySelected != null)
		{
			var endDirection = Swipe(swipeGestureRecognizer);

			timeLeftBetweenSwipe -= Time.deltaTime;
			if (timeLeftBetweenSwipe < 0)
			{
				hasMoved = true;
				switch (GameManager.Instance.actualCamPreset.presetNumber)
				{
					case 1:
						switch (endDirection)
						{
							case SwipeGestureRecognizerDirection.Down: StartCoroutine(StartPlayerMovementCoroutine(0)); break;
							case SwipeGestureRecognizerDirection.Up: StartCoroutine(StartPlayerMovementCoroutine(1)); break;
							case SwipeGestureRecognizerDirection.Right: StartCoroutine(StartPlayerMovementCoroutine(2)); break;
							case SwipeGestureRecognizerDirection.Left: StartCoroutine(StartPlayerMovementCoroutine(3)); break;
						} break;
					case 2:
						switch (endDirection)
						{
							case SwipeGestureRecognizerDirection.Down: StartCoroutine(StartPlayerMovementCoroutine(0)); break;
							case SwipeGestureRecognizerDirection.Up: StartCoroutine(StartPlayerMovementCoroutine(1)); break;
							case SwipeGestureRecognizerDirection.Right: StartCoroutine(StartPlayerMovementCoroutine(2)); break;
							case SwipeGestureRecognizerDirection.Left: StartCoroutine(StartPlayerMovementCoroutine(3)); break;
						} break;
					case 3:
						switch (endDirection)
						{
							case SwipeGestureRecognizerDirection.Down: StartCoroutine(StartPlayerMovementCoroutine(1)); break;
							case SwipeGestureRecognizerDirection.Up: StartCoroutine(StartPlayerMovementCoroutine(0)); break;
							case SwipeGestureRecognizerDirection.Right: StartCoroutine(StartPlayerMovementCoroutine(3)); break;
							case SwipeGestureRecognizerDirection.Left: StartCoroutine(StartPlayerMovementCoroutine(2)); break;
						} break;
					case 4:
						switch (endDirection)
						{
							case SwipeGestureRecognizerDirection.Down: StartCoroutine(StartPlayerMovementCoroutine(1)); break;
							case SwipeGestureRecognizerDirection.Up: StartCoroutine(StartPlayerMovementCoroutine(0)); break;
							case SwipeGestureRecognizerDirection.Right: StartCoroutine(StartPlayerMovementCoroutine(3)); break;
							case SwipeGestureRecognizerDirection.Left: StartCoroutine(StartPlayerMovementCoroutine(2)); break;
						} break;
				}

				timeLeftBetweenSwipe = _timeLeftMax;
			}
		}
	}

	#endregion

	/// <summary>
	/// Function for the long press gesture
	/// </summary>

	#region LongPressBlocMovementGesture

	//Update method of the long press gesture
	private void LongPressBlocMovementGestureOnStateUpdated(GestureRecognizer gesture)
	{
		if (GameManager.Instance.currentPlayerTurn.playerActionPoint > 0)
		{
			if (gesture.State == GestureRecognizerState.Began)
			{
				PlayerStateManager currentPlayerTurn = GameManager.Instance.currentPlayerTurn;

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
						_canTouchBloc = false;
						ResetBlocPreviewMesh();

						ghostPlayer.SetActive(true);
						playerCurrentlySelected = _hit.transform.gameObject;
						var hitObj = _hit.transform.position;
						ghostPlayer.transform.position = new Vector3(hitObj.x, hitObj.y - 0.5f, hitObj.z);

						playerCurrentlySelected = ghostPlayer;

						GameManager.Instance.currentPlayerTurn.playerActionPoint--;

						var cBlockPlayerOn = GameManager.Instance.currentPlayerTurn.currentBlockPlayerOn;

						if (!previewPath.Contains(cBlockPlayerOn))
						{
							previewPath.Add(cBlockPlayerOn);

							GameManager.Instance.currentPlayerTurn.playerActionPoint++;
						}

						SpawnTextActionPointPopUp(_hit.transform);
					}
				}

				if (Physics.Raycast(ray, out _hit, Mathf.Infinity, blocLayerMask) && _canTouchBloc)
				{
					Node.ColorBloc colorBloc = _hit.collider.gameObject.GetComponent<Node>().colorBloc;

					if (colorBloc != Node.ColorBloc.None && !currentPlayerTurn.walking && currentPlayerTurn.nextBlockPath.Contains(_hit.transform) &&
					    !_hit.collider.CompareTag("Player"))
					{
						//If current player have more than 0 action point then he can move bloc
						if (currentPlayerTurn.playerActionPoint > 0)
						{
							if (!hasStopMovingBloc)
							{
								AudioManager.Instance.Play("CubeIsSelected");
								StartMovingBloc(currentPlayerTurn);
							}
						}
					}
				}
				//If press is currently executing
			}
			else if (gesture.State == GestureRecognizerState.Ended && playerCurrentlySelected != null)
			{
				ClearListAfterRelease();
			}
			else if (gesture.State == GestureRecognizerState.Executing)
			{
				if (_isBlocSelected && _canTouchBloc && _blockParentCurrentlySelected != null)
				{
					_touchPos = new Vector3(gesture.DeltaX, gesture.DeltaY, 0);
					BlocMovement(_touchPos);
					_isBlocSelected = false;
				}
			}
			//If press is ended
			else if (gesture.State == GestureRecognizerState.Ended)
			{
				//End of the drag
				EndMovingBloc(GameManager.Instance.currentPlayerTurn);

				_canTouchBloc = true;
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

	#endregion

	/// <summary>
	/// When we start to move a bloc
	/// </summary>

	#region StartMoving Bloc

	private void StartMovingBloc(PlayerStateManager currentPlayerTurn)
	{
		_blockCurrentlySelected = _hit.collider.gameObject;
		_isBlocSelected = true;
		var currentPlayer = currentPlayerTurn.transform;
		_blockParentCurrentlySelected = _blockCurrentlySelected.transform.parent;
		if (_blockParentCurrentlySelected.GetComponent<GroupBlockDetection>() != null)
		{
			_blocParentCurrentlySelectedPos = _blockParentCurrentlySelected.transform.position;
			SpawnTextActionPointPopUp(currentPlayer);
			SetUpBlocPreviewMesh(_blockParentCurrentlySelected);
			hasStopMovingBloc = true;
		}
	}

	#endregion

	/// <summary>
	/// When the bloc is moved 
	/// </summary>

	#region EndMovingBlocState

	private void EndMovingBloc(PlayerStateManager currentPlayerTurn)
	{
		ResetPreviewPathObjects();
		ResetBlocPreviewMesh();
		_isBlocSelected = false;
		_touchPos = Vector3.zero;
		_blockCurrentlySelected = null;
		
		if (_textActionPointPopUp)
		{
			_textActionPointPopUp.SetActive(false);
		}

		if (GameManager.Instance.currentPlayerTurn.playerActionPoint <= 0)
		{
			UiManager.Instance.buttonNextTurn.SetActive(true);
		}

		_lastDirectionBloc = Vector3.zero;
	}

	#endregion

	/// <summary>
	/// Spawn Text Action Point to indicate to the player his action's point
	/// </summary>

	#region SpawnTextActionPoint

	private void SpawnTextActionPointPopUp(Transform currentPlayer)
	{
		totalCurrentActionPoint = GameManager.Instance.currentPlayerTurn.playerActionPoint;
		_textActionPointPopUp =
			PoolManager.Instance.SpawnObjectFromPool("PopUpTextActionPoint", currentPlayer.position + offsetText, Quaternion.identity, currentPlayer);
		_textActionPointPopUp.SetActive(true);
		_textActionPointPopUp.GetComponent<PopUpTextActionPoint>().SetUpText(GameManager.Instance.currentPlayerTurn.playerActionPoint);
	}

	#endregion

	/// <summary>
	/// Update Action Point Text 
	/// </summary>

	#region UpdateActionPointTextPopUp

	public void UpdateActionPointTextPopUp(int actionPointPlayer)
	{
		//Update text action point at player top pos
		actionPointPlayer = totalCurrentActionPoint > 0 ? totalCurrentActionPoint : -totalCurrentActionPoint;
		_textActionPointPopUp.GetComponent<PopUpTextActionPoint>().SetUpText(actionPointPlayer);
		GameManager.Instance.currentPlayerTurn.playerActionPoint = totalCurrentActionPoint;

		if (GameManager.Instance.currentPlayerTurn.playerActionPoint <= 0 && _blockParentCurrentlySelected != null)
		{
			EndMovingBloc(GameManager.Instance.currentPlayerTurn);
		}
	}

	#endregion

	/// <summary>
	/// Set up bloc preview Mesh
	/// </summary>

	#region SetUpBlocPreviewMesh

	private void SetUpBlocPreviewMesh(Transform blocParent)
	{
		ResetBlocPreviewMesh();
		
			foreach (Transform bloc in blocParent.transform)
			{
				var blocPosition = bloc.position;

				GameObject blocPreviewUpMesh = PoolManager.Instance.SpawnObjectFromPool("BlocPreview",
					blocPosition + Vector3.up, Quaternion.identity, null);
				GameObject blocPreviewDownMesh = PoolManager.Instance.SpawnObjectFromPool("BlocPreview",
					blocPosition + Vector3.down * 2, Quaternion.identity, null);

				RoundYBlocPreviewMeshPos(blocPreviewUpMesh);
				RoundYBlocPreviewMeshPos(blocPreviewDownMesh);


				if (Mathf.RoundToInt(blocPreviewUpMesh.transform.position.y) > GameManager.Instance.maxHeightBlocMovement + 1)
				{
					blocPreviewUpMesh.SetActive(false);
				}

				if (Mathf.RoundToInt(blocPreviewDownMesh.transform.position.y) < GameManager.Instance.minHeightBlocMovement)
				{
					blocPreviewDownMesh.SetActive(false);
				}

				_nextBlocUpMeshPos.Add(blocPreviewUpMesh.transform);
				_nextBlocDownMeshPos.Add(blocPreviewDownMesh.transform);
			}
		
	}

	#endregion

	/// <summary>
	/// Reset Bloc Preview Mesh
	/// </summary>

	#region ResetBlocPreviewMesh

	private void ResetBlocPreviewMesh()
	{
		foreach (var nextBlocDownMesh in _nextBlocDownMeshPos)
		{
			nextBlocDownMesh.gameObject.SetActive(false);
		}

		foreach (var nextBlocUpMesh in _nextBlocUpMeshPos)
		{
			nextBlocUpMesh.gameObject.SetActive(false);
		}


		_nextBlocUpMeshPos.Clear();
		_nextBlocDownMeshPos.Clear();
	}

	#endregion

	/// <summary>
	/// Mathf.Round bloc position
	/// </summary>

	#region RoundYBlocPreviewMeshPos

	private void RoundYBlocPreviewMeshPos(GameObject bloc)
	{
		var previewPos = bloc.transform.position;
		previewPos.y = Mathf.Round(previewPos.y);
		bloc.transform.position = previewPos;
	}

	#endregion

	/// <summary>
	/// Know the position if the player swipe to the top or down
	/// </summary>

	#region BlocMovement

	private void BlocMovement(Vector3 touchPos)
	{
		var direction = touchPos.normalized;
		if (_isBlocSelected)
		{
			StartCoroutine(StartBlocMovementCoroutine(touchPos.y, direction));
		}
	}

	#endregion

	/// <summary>
	/// Function to know the position, if the bloc will go to the top or down
	/// </summary>

	#region StartBlocMovementCoroutine

	IEnumerator StartBlocMovementCoroutine(float yPos, Vector3 direction)
	{
		var groupBlocDetection = _blockParentCurrentlySelected.GetComponent<GroupBlockDetection>();
		var blocParentNewPos = _blockParentCurrentlySelected.transform.position;

		if (_lastDirectionBloc == Vector3.zero)
		{
			_lastDirectionBloc = direction;
		}


		if (ActualCamPreset.CamPresetTeam() == ActualCamPreset.Team.TeamOne)
		{
			yPos = +yPos;
		}

		else if (ActualCamPreset.CamPresetTeam() == ActualCamPreset.Team.TeamTwo)
		{
			yPos = -yPos;
			direction = -direction;
		}


		if (yPos > 0.0f)
		{
			AudioManager.Instance.Play("CubeIsMoving");
			
			if (blocParentNewPos.y - GameManager.Instance.maxHeightBlocMovement == 0 || totalCurrentActionPoint == 0 && _lastDirectionBloc.y > 0.0f)
			{
				//TODO Feedback can't move bloc
				Debug.Log("Im blocked up");
			}
			else
			{
				StartMoveBloc(_nextBlocUpMeshPos, groupBlocDetection, blocParentNewPos, movementBlocAmount);
				yield return _timeInSecondsBetweenBlocMovement;
				EndMoveBloc(blocParentNewPos.y - _blocParentCurrentlySelectedPos.y >= 0, direction);
			}
		}
		else if (yPos < 0.0f)
		{
			AudioManager.Instance.Play("CubeIsMoving");
			
			if (blocParentNewPos.y - GameManager.Instance.minHeightBlocMovement == 0 || totalCurrentActionPoint == 0 && _lastDirectionBloc.y < 0.0f)
			{
				//TODO Feedback can't move bloc
				Debug.Log("Im blocked down");
			}
			else
			{
				StartMoveBloc(_nextBlocDownMeshPos, groupBlocDetection, blocParentNewPos, -movementBlocAmount);
				yield return _timeInSecondsBetweenBlocMovement;
				EndMoveBloc(blocParentNewPos.y - _blocParentCurrentlySelectedPos.y <= 0, direction);
			}
		}

		yield return _timeInSecondsBetweenBlocMovement;
		ResetPreviewPathObjects();
		_touchPos = Vector3.zero;
		_isBlocSelected = true;
		hasStopMovingBloc = false;
	}

	#endregion

	/// <summary>
	/// Move the bloc
	/// </summary>

	#region MovingBloc

	private void StartMoveBloc(List<Transform> previewBlocsMesh, GroupBlockDetection groupBlocDetection, Vector3 blocParentNewPos, float moveBlocAmount)
	{
		ResetBlocPreviewMesh();
		foreach (var blocMesh in previewBlocsMesh)
		{
			blocMesh.gameObject.SetActive(false);
		}

		MoveBlocAndPlayer(moveBlocAmount, _blockParentCurrentlySelected, blocParentNewPos, groupBlocDetection);
		var player = GameManager.Instance.currentPlayerTurn;
		player.PlayerActionPointCardState.SetFalsePathObjects();
	}

	private void EndMoveBloc(bool isPlayerUseActionPoint, Vector3 direction)
	{
		switch (isPlayerUseActionPoint)
		{
			case true:
				UpdateActionPointTextPopUp(totalCurrentActionPoint--);
				break;
			case false:
				UpdateActionPointTextPopUp(totalCurrentActionPoint++);
				break;
		}


		if (!hasStopMovingBloc)
		{
			SetUpBlocPreviewMesh(_blockParentCurrentlySelected);
		}

		var player = GameManager.Instance.currentPlayerTurn;
		player.PlayerActionPointCardState.SetFalsePathObjects();
		_lastDirectionBloc = direction;
	}


	private void MoveBlocAndPlayer(float value, Transform blocParent, Vector3 blocParentNewPos, GroupBlockDetection groupBlocDetection)
	{
		blocParent.DOMove(new Vector3(blocParentNewPos.x, blocParentNewPos.y + value, blocParentNewPos.z),
			_timeInSecondsForBlocMove);
		//Move the player with block
		if (groupBlocDetection.playersOnGroupBlock.Count > 0)
		{
			foreach (Transform playerOnGroupBlock in groupBlocDetection.playersOnGroupBlock)
			{
				Vector3 playerOnGroupBlockPos = playerOnGroupBlock.position;
				playerOnGroupBlock.DOMove(
					new Vector3(playerOnGroupBlockPos.x, playerOnGroupBlockPos.y + value, playerOnGroupBlockPos.z),
					_timeInSecondsForBlocMove);
			}
		}
	}

	#endregion

	/// <summary>
	/// Reset Preview Path Objects
	/// </summary>

	#region ResetPreviewPathObjects

	private void ResetPreviewPathObjects()
	{
		var player = GameManager.Instance.currentPlayerTurn;
		player.PlayerActionPointCardState.SetFalsePathObjects();
		player.PlayerActionPointCardState.PreviewPath(player.playerActionPoint, player);
	}

	#endregion

	/// <summary>
	/// Clear List when the player release the ghos
	/// </summary>

	#region ClearListAfterRelease

	private void ClearListAfterRelease() // Clear the list after the player released the Squeeples
	{
		_canTouchBloc = true;
		GameManager.Instance.currentPlayerTurn.nextBlockPath = previewPath;
		timeLeftBetweenSwipe = _timeLeftMax;

		for (int i = 0; i < sphereList.Count; i++)
		{
			sphereList[i].SetActive(false);
		}
		
		GameManager.Instance.currentPlayerTurn.currentTouchBlock = ghostPlayer.GetComponent<CheckUnderGhost>().currentBlockGhostOn;
		GameManager.Instance.currentPlayerTurn.StartPathFinding();
	}

	#endregion

	private void Update()
	{
		if (Math.Abs(GameManager.Instance.currentPlayerTurn.transform.position.x - ghostPlayer.transform.position.x) < 0.05f && 
		    Math.Abs(GameManager.Instance.currentPlayerTurn.transform.position.z - ghostPlayer.transform.position.z) < 0.05f && hasMoved)
		{
			StartCoroutine(ResetPreviewPlatformCoroutine());
			hasMoved = false;
		}
	}

	/// <summary>
	/// Coroutine to displace the player in the direction of the swipe
	/// </summary>

	#region StartPlayerMovementCoroutine

	private IEnumerator StartPlayerMovementCoroutine(int direction) // Depends on the position the player wants to go, he moves in the wished direction
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

	#endregion

	/// <summary>
	/// Preview Path to show where the ghost go 
	/// </summary>

	#region PreviewPath

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
						UpdateActionPointTextPopUp(totalCurrentActionPoint--);
						ghostPlayer.transform.position += _directionPlayer[value];

						StartCoroutine(WaitBeforeCheckUnderPlayerCoroutine());
					}
					
					NFCManager.Instance.displacementActivated = true;
				}
				else if (previewPath.Count - 1 == positionList + 1)
				{
					UpdateActionPointTextPopUp(totalCurrentActionPoint--);
					ghostPlayer.transform.position += _directionPlayer[value];

					StartCoroutine(WaitBeforeCheckUnderPlayerCoroutine());
				}
			}
		}
	}

	#endregion

	/// <summary>
	/// Check Under Player to upload his currentBlockOn
	/// </summary>

	#region WaitBeforeCheckUnderPlayerCoroutine

	private IEnumerator WaitBeforeCheckUnderPlayerCoroutine() // Check the block under the ghost
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

	#endregion

	/// <summary>
	/// Launch Bullet for ghost to show the path to players
	/// </summary>

	#region LaunchBullet

	private void LaunchBullet(Vector3 positionToInstantiate) // Launch bullet 
	{
		GameObject sphere = PoolManager.Instance.SpawnObjectFromPool(
			"SphereShowPath", new Vector3(positionToInstantiate.x, positionToInstantiate.y + 1.2f, positionToInstantiate.z), Quaternion.identity,
			null);

		sphereList.Add(sphere);
	}

	#endregion

	/// <summary>
	/// Reset Preview Platform
	/// </summary>

	#region ResetPreviewPlatformCoroutine

	private IEnumerator ResetPreviewPlatformCoroutine()
	{
		yield return _timeBetweenReloadPath;

		if (!GameManager.Instance.currentPlayerTurn.walking)
		{
			var player = GameManager.Instance.currentPlayerTurn;
			player.PlayerActionPointCardState.SetFalsePathObjects(); 
			player.PlayerActionPointCardState.EnterState( player);
		}
	}

	#endregion

	
	/// <summary>
	/// Reset Displacement when the player take off his card before he moves
	/// </summary>
	#region ResetDisplacement

	public void ResetDisplacement()
	{
		var player = GameManager.Instance.currentPlayerTurn;
		player.PlayerActionPointCardState.SetFalsePathObjects();
		player.playerActionPoint = 0;
		player.PlayerActionPointCardState.PreviewPath(player.playerActionPoint, player);

		GameManager.Instance.DecreaseVariable();
	}

	#endregion
	
	
}