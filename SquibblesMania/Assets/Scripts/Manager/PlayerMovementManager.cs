using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DigitalRubyShared;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovementManager : MonoBehaviour
{
	private readonly List<RaycastResult> raycast = new List<RaycastResult>();
	public LongPressGestureRecognizer LongPressBlocMovementGesture { get; private set; }

	[Header("TOUCH PARAMETERS")] private Vector3 _touchPos;
	public LayerMask touchLayerMask;
	public LayerMask blocLayerMask;
	private Camera _cam;
	private RaycastHit _hit;
	[Header("Player PARAMETERS")]
	public GameObject playerCurrentlySelected;
	public float playerMovementSpeed;
	public bool isPlayerSelected;
	public float valueInputDisplacement;
	public float raycastDistance;
	
	private readonly List<Vector3> _directionPlayer = new List<Vector3> {Vector3.back, Vector3.forward, Vector3.right, Vector3.left, Vector3.down};
	
	private WaitForSeconds _timeBetweenPlayerMovement = new WaitForSeconds(0.2f);

	// Start is called before the first frame update
	void Awake()
	{
		_cam = Camera.main;
	}

	private void OnEnable()
	{
		//Set up the new gesture 
		LongPressBlocMovementGesture = new LongPressGestureRecognizer();
		LongPressBlocMovementGesture.StateUpdated += LongPressBlocMovementGestureOnStateUpdated;
		LongPressBlocMovementGesture.ThresholdUnits = 0.0f;
		LongPressBlocMovementGesture.MinimumDurationSeconds = 0.3f;
		//LongPressBlocMovementGesture.AllowSimultaneousExecutionWithAllGestures();
		FingersScript.Instance.AddGesture(LongPressBlocMovementGesture);
	}

	private void OnDisable()
	{
		if (FingersScript.HasInstance)
		{
			FingersScript.Instance.RemoveGesture(LongPressBlocMovementGesture);
		}
	}

	//Update method of the long press gesture
	private void LongPressBlocMovementGestureOnStateUpdated(GestureRecognizer gesture)
	{
		/*if (GameManager.Instance.currentPlayerTurn.isPlayerInActionCardState)
		{
		}*/

		if (gesture.State == GestureRecognizerState.Began)
		{
			PointerEventData p = new PointerEventData(EventSystem.current);
			p.position = new Vector2(gesture.FocusX, gesture.FocusY);

			raycast.Clear();
			EventSystem.current.RaycastAll(p, raycast);
			// Cast a ray from the camera
			Ray ray = _cam.ScreenPointToRay(p.position);

			if (Physics.Raycast(ray, out _hit, Mathf.Infinity, touchLayerMask))
			{
				if (_hit.collider.name == GameManager.Instance.currentPlayerTurn.name)
				{
					playerCurrentlySelected = _hit.collider.gameObject;
					isPlayerSelected = true;
				}
			}
		}
		else if (gesture.State == GestureRecognizerState.Executing)
		{
			if (isPlayerSelected)
			{
				_touchPos = new Vector3(gesture.DeltaX, gesture.DeltaY, 0);
				PlayerMovement(_touchPos);
			}
		}
		else if (gesture.State == GestureRecognizerState.Ended)
		{
			//End of the drag
			isPlayerSelected = false;
			playerCurrentlySelected = null;
			_touchPos = Vector3.zero;
		}
	}

	private void PlayerMovement(Vector3 touchPos)
	{
		isPlayerSelected = false;
		StartCoroutine(StartPlayerMovement(touchPos.x, touchPos.y));
	}

	IEnumerator StartPlayerMovement(float xPos, float zPos)
	{
		#region Displacement
		
		if (xPos > 5 && zPos > 5)
		{
			if (Physics.Raycast(playerCurrentlySelected.transform.position, new Vector3(0,-0.5f,1), out var hit, raycastDistance, blocLayerMask))
			{
				if (Math.Abs(hit.transform.position.y - GameManager.Instance.currentPlayerTurn.currentBlockPlayerOn.position.y) < 0.1f)
				{
					playerCurrentlySelected.transform.DOMove(playerCurrentlySelected.transform.position + _directionPlayer[1], playerMovementSpeed);
				}
			}
		}
		if (xPos < -2 && zPos < -2)
		{
			if (Physics.Raycast(playerCurrentlySelected.transform.position, new Vector3(0,-0.5f,-1), out var hit, raycastDistance, blocLayerMask))
			{
				if (Math.Abs(hit.transform.position.y - GameManager.Instance.currentPlayerTurn.currentBlockPlayerOn.position.y) < 0.1f)
				{
					playerCurrentlySelected.transform.DOMove(playerCurrentlySelected.transform.position + _directionPlayer[0], playerMovementSpeed);
				}
			}
		}
		if (xPos > 2.5f && zPos < -5)
		{
			if (Physics.Raycast(playerCurrentlySelected.transform.position, new Vector3(1,-0.5f,0), out var hit, raycastDistance, blocLayerMask))
			{
				if (Math.Abs(hit.transform.position.y - GameManager.Instance.currentPlayerTurn.currentBlockPlayerOn.position.y) < 0.1f)
				{
					playerCurrentlySelected.transform.DOMove(playerCurrentlySelected.transform.position + _directionPlayer[2], playerMovementSpeed);
				}
			}
		}

		if (xPos < -8 && zPos > 2.5f)
		{
			if (Physics.Raycast(playerCurrentlySelected.transform.position, new Vector3(-1,-0.5f,0), out var hit, raycastDistance, blocLayerMask))
			{
				if (Math.Abs(hit.transform.position.y - GameManager.Instance.currentPlayerTurn.currentBlockPlayerOn.position.y) < 0.1f)
				{
					playerCurrentlySelected.transform.DOMove(playerCurrentlySelected.transform.position + _directionPlayer[3], playerMovementSpeed);
				}
			}
		}
	
		#endregion
		
		/*if (GameManager.Instance.currentPlayerTurn.playerActionPoint > 0)
		{
			if (xPos > 0.0f && zPos > 0.0f)
			{
				GameManager.Instance.currentPlayerTurn.playerActionPoint--;
				playerCurrentlySelected.transform.DOMove(playerCurrentlySelected.transform.position + _directionPlayer[1], playerMovementSpeed);
			}
			if (xPos < 0.0f && zPos > 0.0f)
			{
				GameManager.Instance.currentPlayerTurn.playerActionPoint--;
				playerCurrentlySelected.transform.DOMove(playerCurrentlySelected.transform.position + _directionPlayer[3], playerMovementSpeed);
			}
			if (xPos > 0.0f && zPos < 0.0f)
			{
				GameManager.Instance.currentPlayerTurn.playerActionPoint--;
				playerCurrentlySelected.transform.DOMove(playerCurrentlySelected.transform.position + _directionPlayer[2], playerMovementSpeed);
			}
			if (xPos < 0.0f && zPos < 0.0f)
			{
				GameManager.Instance.currentPlayerTurn.playerActionPoint--;
				playerCurrentlySelected.transform.DOMove(playerCurrentlySelected.transform.position + _directionPlayer[0], playerMovementSpeed);
			}
			UiManager.Instance.SetUpCurrentActionPointOfCurrentPlayer(GameManager.Instance.currentPlayerTurn.playerActionPoint);
		}*/

		yield return _timeBetweenPlayerMovement;
		isPlayerSelected = true;
	}
	
}