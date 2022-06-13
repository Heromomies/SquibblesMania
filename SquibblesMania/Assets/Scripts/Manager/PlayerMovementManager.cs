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
	public TapGestureRecognizer TapGestureRecognizer { get; private set; }
	[Header("TOUCH SETTINGS")] public LayerMask ghostLayerMask;
	public LayerMask blocLayerMask;
	[Range(0.0f, 1.0f)] public float longPressureDurationSeconds;

	[Header("PLAYER SETTINGS")] public GameObject ghostPlayer;
	private Vector3 _touchPos;
	private Camera _cam;
	private RaycastHit _hit;
	public bool hasStopMovingBloc;
	public bool isPlayerPreviewPath;
	[SerializeField] private Transform blockParentCurrentlySelected;

	private GameObject _blockCurrentlySelected;

	[Header("BLOC SETTINGS")] [SerializeField]
	private float movementBlocAmount = 1f;

	[SerializeField] private float minimalDistanceToMoveBloc = 50f;
	private Vector3 _blocParentCurrentlySelectedPos; 
	[SerializeField]
	private bool _isBlocSelected;
	private Vector3 _lastDirectionBloc;
	private float _timeInSecondsForBlocMove = 0.4f;
	private readonly WaitForSeconds _timeInSecondsBetweenBlocMovement = new WaitForSeconds(0.3f);
	private readonly WaitForSeconds _timeInSecondsBetweenBlocSwipe = new WaitForSeconds(0.2f);
	private readonly List<Transform> _nextBlocUpMeshPos = new List<Transform>();
	private readonly List<Transform> _nextBlocDownMeshPos = new List<Transform>();

	private bool _canTouchBloc = true;
	private readonly List<RaycastResult> _raycast = new List<RaycastResult>();

	#region Singleton

	private static PlayerMovementManager playerMovementManager;

	public static PlayerMovementManager Instance => playerMovementManager;
	

	private void Awake()
	{
		_cam = Camera.main;
		playerMovementManager = this;
	}

	#endregion

	/// <summary>
	/// // Start and enable gesture
	/// </summary>

	#region Start

	private void Start()
	{
		TapGestureRecognizer = new TapGestureRecognizer();
		TapGestureRecognizer.StateUpdated += TapGestureRecognizerOnStateUpdated;
		TapGestureRecognizer.ThresholdSeconds = 0.1f;
		TapGestureRecognizer.SendBeginState = true;
		FingersScript.Instance.AddGesture(TapGestureRecognizer);
		TapGestureRecognizer.AllowSimultaneousExecutionWithAllGestures();

		//Set up the new gesture 
		LongPressBlocMovementGesture = new LongPressGestureRecognizer();
		LongPressBlocMovementGesture.StateUpdated += LongPressBlocMovementGestureOnStateUpdated;
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
			FingersScript.Instance.RemoveGesture(TapGestureRecognizer);
		}
	}

	#endregion

	private void TapGestureRecognizerOnStateUpdated(GestureRecognizer gesture)
	{
		PlayerStateManager currentPlayer = GameManager.Instance.currentPlayerTurn;
	
		if (currentPlayer.isPlayerInActionCardState && currentPlayer.playerActionPoint > 0 && !currentPlayer.walking)
		{
			
			if (gesture.State == GestureRecognizerState.Began)
			{
				if (Physics.Raycast(TouchRay(gesture), out _hit, Mathf.Infinity, ghostLayerMask))
				{
					isPlayerPreviewPath = false;
					currentPlayer.StartPlayerMovement();
					return;
				}
				
				if (Physics.Raycast(TouchRay(gesture), out _hit, Mathf.Infinity, blocLayerMask) && !isPlayerPreviewPath)
				{
				
					var hitBlocParentPos = _hit.transform.parent.position;
					
					if (currentPlayer.nextBlockPath.Contains(_hit.transform) && currentPlayer.PlayerActionPointCardState.PathParentPosComparedToPlayerPos(hitBlocParentPos, currentPlayer.transform.position) && HitBlockEqualToCurrentBlockPlayerOn(_hit, currentPlayer))
					{
						currentPlayer.currentTouchBloc = _hit.collider.gameObject.transform;
						isPlayerPreviewPath = true;
						currentPlayer.StartPreviewPathFinding();
					}
					else if (!currentPlayer.nextBlockPath.Contains(_hit.transform) && isPlayerPreviewPath)
					{
						isPlayerPreviewPath = false;
						currentPlayer.ResetPreviewPathFinding();
					}
				}
			
				else if (TouchFailed(gesture) || TouchBlocNotInPreviewPath(gesture, currentPlayer))
				{
					isPlayerPreviewPath = false;
					currentPlayer.ResetPreviewPathFinding();
				}
				
			}


		}
	}

	private bool TouchBlocNotInPreviewPath(GestureRecognizer gesture, PlayerStateManager currentPlayer)
	{
		return Physics.Raycast(TouchRay(gesture), out _hit, Mathf.Infinity, blocLayerMask) && (!currentPlayer.nextBlockPath.Contains(_hit.transform));
	}
	private bool TouchFailed(GestureRecognizer gesture)
	{
		return !Physics.Raycast(TouchRay(gesture)) && isPlayerPreviewPath;
	}

	private bool HitBlockEqualToCurrentBlockPlayerOn(RaycastHit hit, PlayerStateManager player)
	{
		return hit.transform != player.currentBlocPlayerOn;
	}
	private Ray TouchRay(GestureRecognizer gesture)
	{
		PointerEventData p = new PointerEventData(EventSystem.current);
		p.position = new Vector2(gesture.FocusX, gesture.FocusY);
		_raycast.Clear();
		EventSystem.current.RaycastAll(p, _raycast);
		// Cast a ray from the camera
		return _cam.ScreenPointToRay(p.position);
	}


	/// <summary>
	/// Function for the long press gesture
	/// </summary>

	#region LongPressBlocMovementGesture

	//Update method of the long press gesture
	private void LongPressBlocMovementGestureOnStateUpdated(GestureRecognizer gesture)
	{
		PlayerStateManager currentPlayerTurn = GameManager.Instance.currentPlayerTurn;
		
		if (currentPlayerTurn.playerActionPoint >= 0 && !currentPlayerTurn.walking)
		{
			if (gesture.State == GestureRecognizerState.Began)
			{
				
				PointerEventData p = new PointerEventData(EventSystem.current);
				p.position = new Vector2(gesture.FocusX, gesture.FocusY);

				_raycast.Clear();
				EventSystem.current.RaycastAll(p, _raycast);
				// Cast a ray from the camera
				Ray ray = _cam.ScreenPointToRay(p.position);
				
				if (Physics.Raycast(ray, out _hit, Mathf.Infinity, blocLayerMask) && _canTouchBloc)
				{
					if (_hit.collider.gameObject.TryGetComponent(out Node hitBlocNode))
					{
						Node.ColorBloc colorBloc = hitBlocNode.colorBloc;
						
						if (colorBloc != Node.ColorBloc.None && !currentPlayerTurn.walking && currentPlayerTurn.nextBlockPath.Contains(_hit.transform))
						{
							//If current player have more than 0 action point then he can move bloc
							if (currentPlayerTurn.playerActionPoint > 0)
							{
								isPlayerPreviewPath = false;
								currentPlayerTurn.ResetPreviewPathFinding();
								
								if (!hasStopMovingBloc)
								{
									UiManager.Instance.sliderNextTurn.interactable = false;
									AudioManager.Instance.Play("CubeIsSelected");
									StartMovingBloc(currentPlayerTurn);
								}
							}
						}
					}
					
				}
				
			}
			//If press is currently executing
			else if (gesture.State == GestureRecognizerState.Executing)
			{
				if (_isBlocSelected && _canTouchBloc && blockParentCurrentlySelected != null)
				{
					_touchPos = new Vector3(gesture.DeltaX, gesture.DeltaY, 0);
					BlocMovement(_touchPos);
					_isBlocSelected = false;
				}
			}
			//If press is ended
			else if (gesture.State == GestureRecognizerState.Ended && blockParentCurrentlySelected != null)
			{
				//End of the drag
				EndMovingBloc();
				UiManager.Instance.sliderNextTurn.interactable = true;
				_canTouchBloc = true;
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
		blockParentCurrentlySelected = _blockCurrentlySelected.transform.parent;
		UiManager.Instance.totalCurrentActionPoint = currentPlayerTurn.playerActionPoint;
		
		if (blockParentCurrentlySelected.TryGetComponent(out GroupBlockDetection groupBlockDetection))
		{
			_blocParentCurrentlySelectedPos = blockParentCurrentlySelected.transform.position;
			if (!UiManager.Instance.textActionPointPopUp)
			{
				UiManager.Instance.SpawnTextActionPointPopUp(currentPlayer);
			}
		
			if(blockParentCurrentlySelected != null) SetUpBlocPreviewMesh(blockParentCurrentlySelected);
			hasStopMovingBloc = true;
		}
	}

	#endregion

	/// <summary>
	/// When the bloc is moved 
	/// </summary>

	#region EndMovingBlocState

	private void EndMovingBloc()
	{
		ResetPreviewPathObjects();
		ResetBlocPreviewMesh();
		_isBlocSelected = false;
		_touchPos = Vector3.zero;
		hasStopMovingBloc = false;
		_blockCurrentlySelected = null;
		blockParentCurrentlySelected = null;
		_lastDirectionBloc = Vector3.zero;
	}

	#endregion
	

	/// <summary>
	/// Update Action Point Text 
	/// </summary>

	#region UpdateActionPointTextPopUp

	private void UpdateActionPointTextPopUp(int actionPointPlayer)
	{
		//Update text action point at player top pos
		actionPointPlayer = UiManager.Instance.totalCurrentActionPoint > 0 ? UiManager.Instance.totalCurrentActionPoint : -UiManager.Instance.totalCurrentActionPoint;

		if (UiManager.Instance.textActionPointPopUp.TryGetComponent(out PopUpTextActionPoint popUpTextActionPoint))
		{
			popUpTextActionPoint.SetUpText(actionPointPlayer);
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

				GameObject blocPreviewUpMesh = PoolManager.Instance.SpawnObjectFromPool("BlocPreviewUp",
					blocPosition + Vector3.up, Quaternion.identity, null);
				GameObject blocPreviewDownMesh = PoolManager.Instance.SpawnObjectFromPool("BlocPreviewDown",
					blocPosition + Vector3.down * 2, Quaternion.identity, null);

				RoundYBlocPreviewMeshPos(blocPreviewUpMesh);
				RoundYBlocPreviewMeshPos(blocPreviewDownMesh);

				if (Mathf.RoundToInt(blocPreviewUpMesh.transform.position.y) > GameManager.Instance.maxHeightBlocMovement)
				{
					blocPreviewUpMesh.SetActive(false);
				}
				
				if (Mathf.RoundToInt(blocPreviewDownMesh.transform.position.y) < GameManager.Instance.minHeightBlocMovement - 1)
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
		Debug.Log(yPos);
		if (blockParentCurrentlySelected.TryGetComponent(out GroupBlockDetection groupBlocDetection))
		{
			var blocParentNewPos = blockParentCurrentlySelected.transform.position;
				
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


			if (yPos > minimalDistanceToMoveBloc)
			{
				
				if (blocParentNewPos.y - GameManager.Instance.maxHeightBlocMovement == 0 || UiManager.Instance.totalCurrentActionPoint == 0 && _lastDirectionBloc.y > 0.0f)
				{
					AudioManager.Instance.Play("CardFalse");
				}
				else
				{
					AudioManager.Instance.Play("CubeIsMoving");
					StartMoveBloc(_nextBlocUpMeshPos, groupBlocDetection, blocParentNewPos, movementBlocAmount);
					yield return _timeInSecondsBetweenBlocSwipe;
					EndMoveBloc(blocParentNewPos.y - _blocParentCurrentlySelectedPos.y >= 0, direction);
				}
			}
			else if (yPos < -minimalDistanceToMoveBloc)
			{
				if (blocParentNewPos.y - GameManager.Instance.minHeightBlocMovement == 0 || UiManager.Instance.totalCurrentActionPoint == 0 && _lastDirectionBloc.y < 0.0f)
				{
					AudioManager.Instance.Play("CardFalse");
				}
				else
				{
					AudioManager.Instance.Play("CubeIsMoving");
					StartMoveBloc(_nextBlocDownMeshPos, groupBlocDetection, blocParentNewPos, -movementBlocAmount);
					yield return _timeInSecondsBetweenBlocSwipe;
					EndMoveBloc(blocParentNewPos.y - _blocParentCurrentlySelectedPos.y <= 0, direction);
				}
			}

			yield return _timeInSecondsBetweenBlocMovement;
			ResetPreviewPathObjects();
			_touchPos = Vector3.zero;
			hasStopMovingBloc = false;
			_isBlocSelected = true;
			if (GameManager.Instance.currentPlayerTurn.playerActionPoint <= 0 && !blockParentCurrentlySelected)
			{
				EndMovingBloc();
			}
			GameManager.Instance.PlayerMoving();
		}
		
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

		MoveBlocAndPlayer(moveBlocAmount, blockParentCurrentlySelected, blocParentNewPos, groupBlocDetection);
		var player = GameManager.Instance.currentPlayerTurn;
		player.PlayerActionPointCardState.SetFalsePathObjects();
		
	}

	private void EndMoveBloc(bool isPlayerUseActionPoint, Vector3 direction)
	{
		switch (isPlayerUseActionPoint)
		{
			case true: UpdateActionPointTextPopUp(UiManager.Instance.totalCurrentActionPoint--);
				GameManager.Instance.currentPlayerTurn.playerActionPoint = UiManager.Instance.totalCurrentActionPoint;
				break;
			case false: UpdateActionPointTextPopUp(UiManager.Instance.totalCurrentActionPoint++); 
				GameManager.Instance.currentPlayerTurn.playerActionPoint = UiManager.Instance.totalCurrentActionPoint;
				break;
		}
		
		if (!hasStopMovingBloc && blockParentCurrentlySelected != null)
		{
			SetUpBlocPreviewMesh(blockParentCurrentlySelected);
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
				playerOnGroupBlock.DOMove(new Vector3(playerOnGroupBlockPos.x, playerOnGroupBlockPos.y + value, playerOnGroupBlockPos.z),
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
		player.PlayerActionPointCardState.EnterState(player);
	}

	#endregion
	/// <summary>
	/// Reset Displacement when the player take off his card before he moves
	/// </summary>
	#region ResetDisplacement

	public void ResetDisplacement()
	{
		var player = GameManager.Instance.currentPlayerTurn;
		isPlayerPreviewPath = false;
		player.PlayerActionPointCardState.SetFalsePathObjects();
		player.playerActionPoint = 0;
		player.PlayerActionPointCardState.EnterState(player);

		if (UiManager.Instance.textActionPointPopUp != null)
		{
			UiManager.Instance.textActionPointPopUp.SetActive(false);
			UiManager.Instance.textActionPointPopUp = null;
		}
		GameManager.Instance.DecreaseVariable();
	}

	#endregion
	
	
}