using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DigitalRubyShared;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchManager : MonoBehaviour
{
	private readonly List<RaycastResult> raycast = new List<RaycastResult>();


	public PanGestureRecognizer PlayerTouchGesture { get; private set; }

	[Header("TOUCH MANAGER")] [HideInInspector]
	public Transform blockParent;

	[SerializeField] private RectTransform canvasTransform;
	[SerializeField] private Vector3 offsetPos;




	public LayerMask touchLayersMask;
	private Camera _cam;
	public RaycastHit Hit;
	public GameObject blockCurrentlySelected;
	public Color blockCurrentlyBaseColor;
	private static TouchManager _touchManager;

	public static TouchManager Instance => _touchManager;

	public List<UiInteraction> uiInteraction;

	[Serializable]
	public struct UiInteraction
	{
		public GameObject uiInteractionParentObject;
		public Button buttonGoToBloc;
		public Button[] buttonMoveDownUp;
	}

	private void Awake()
	{
		_touchManager = this;
	}

	private void Start()
	{
		_cam = Camera.main;
	}

	private void OnEnable()
	{
		PlayerTouchGesture = new PanGestureRecognizer();
		PlayerTouchGesture.ThresholdUnits = 0.0f; // start right away
		//Add new gesture
		PlayerTouchGesture.StateUpdated += PlayerTouchGestureUpdated;
		PlayerTouchGesture.AllowSimultaneousExecutionWithAllGestures();

		FingersScript.Instance.AddGesture(PlayerTouchGesture);

		//Allow gesture to work through certain objects

		for (int i = 0; i < uiInteraction.Count; i++)
		{
			FingersScript.Instance.PassThroughObjects.Add(uiInteraction[i].uiInteractionParentObject);
		}

		if (UiManager.Instance != null)
		{
			FingersScript.Instance.PassThroughObjects.Add(UiManager.Instance.buttonNextTurn);
		}
	}

	private void OnDisable()
	{
		if (FingersScript.HasInstance)
		{
			FingersScript.Instance.RemoveGesture(PlayerTouchGesture);
		}


		for (int i = 0; i < uiInteraction.Count; i++)
		{
			FingersScript.Instance.PassThroughObjects.Remove(uiInteraction[i].uiInteractionParentObject);
		}

		FingersScript.Instance.PassThroughObjects.Remove(UiManager.Instance.buttonNextTurn);
		FingersScript.Instance.PassThroughObjects.Clear();
	}

	public void RemoveFingerScriptPassThroughObject()
	{
		FingersScript.Instance.PassThroughObjects.Remove(UiManager.Instance.buttonNextTurn);
		FingersScript.Instance.PassThroughObjects.Remove(UiManager.Instance.currentActionPointsOfCurrentPlayerTurn
			.gameObject);
	}

	public void AddFingerScriptPassTroughObject()
	{
		FingersScript.Instance.PassThroughObjects.Add(UiManager.Instance.buttonNextTurn);
		FingersScript.Instance.PassThroughObjects.Add(UiManager.Instance.currentActionPointsOfCurrentPlayerTurn
			.gameObject);
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

			if (Physics.Raycast(ray, out Hit, Mathf.Infinity, touchLayersMask))
			{
				if (blockCurrentlySelected != null && !GameManager.Instance.currentPlayerTurn.walking)
				{
					//Previous selected block get his base color back
					MovementBlockManager.Instance.ResetPreviousBlockColor();

				}

				PlayerTurnActionStateSelectBlock();
			}

			else
			{
				//If player OnSelect the block, the block get his color back
				if (GameManager.Instance.currentPlayerTurn.isPlayerInActionCardState &&
				    MovementBlockManager.Instance.isMovingBlock)
				{
					MovementBlockManager.Instance.ResetPreviousBlockColor();
					blockCurrentlySelected = null;
				}

				gesture.Reset();
				if (GameManager.Instance.actualCamPreset.presetNumber == 1 ||
				    GameManager.Instance.actualCamPreset.presetNumber == 2)
				{
					uiInteraction[0].uiInteractionParentObject.SetActive(false);
				}
				else if (GameManager.Instance.actualCamPreset.presetNumber == 3 ||
				         GameManager.Instance.actualCamPreset.presetNumber == 4)
				{
					uiInteraction[1].uiInteractionParentObject.SetActive(false);
				}

			}
		}
	}

	void PlayerTurnActionStateSelectBlock()
	{
		if (Hit.transform.gameObject.GetComponent<Node>() && !GameManager.Instance.currentPlayerTurn.walking &&
		    GameManager.Instance.currentPlayerTurn.isPlayerInActionCardState)
		{
			Debug.Log(GameManager.Instance.currentPlayerTurn.nextBlockPath.Contains(Hit.transform));
			if (GameManager.Instance.currentPlayerTurn.nextBlockPath.Contains(Hit.transform) &&
			    GameManager.Instance.currentPlayerTurn.playerActionPoint > 0)
			{

				switch (GameManager.Instance.actualCamPreset.presetNumber)
				{
					case 1:
						uiInteraction[0].uiInteractionParentObject.SetActive(false);
						break;
					case 2:
						uiInteraction[0].uiInteractionParentObject.SetActive(false);
						break;
					case 3:
						uiInteraction[1].uiInteractionParentObject.SetActive(false);
						break;
					case 4:
						uiInteraction[1].uiInteractionParentObject.SetActive(false);
						break;
				}

				//Take the block group parent from hit block gameobject
				GroupBlockDetection blockGroupParent = Hit.transform.parent.GetComponent<GroupBlockDetection>();


				//Take the current player position
				Vector3 currentPlayerPos = GameManager.Instance.currentPlayerTurn.gameObject.transform.position;
				//Take the current block group selected position
				Vector3 blockGroupParentPos = blockGroupParent.gameObject.transform.position;
				//Change pos of canvas base on the current block selected

				//canvasTransform.position = Hit.transform.position + offsetPos;
				if (GameManager.Instance.actualCamPreset.presetNumber == 1 ||
				    GameManager.Instance.actualCamPreset.presetNumber == 2)
				{
					uiInteraction[0].uiInteractionParentObject.SetActive(true);
					uiInteraction[0].buttonGoToBloc.interactable = true;

				}
				else if (GameManager.Instance.actualCamPreset.presetNumber == 3 ||
				         GameManager.Instance.actualCamPreset.presetNumber == 4)
				{
					uiInteraction[1].uiInteractionParentObject.SetActive(true);
					uiInteraction[1].buttonGoToBloc.interactable = true;
				}

				blockCurrentlySelected = Hit.transform.gameObject;
				blockParent = Hit.collider.gameObject.transform.parent;

				if (blockCurrentlySelected.CompareTag("Untagged"))
				{
					if (GameManager.Instance.actualCamPreset.presetNumber == 1 ||
					    GameManager.Instance.actualCamPreset.presetNumber == 2)
					{
						uiInteraction[0].buttonGoToBloc.interactable = true;
						for (int i = 0; i < uiInteraction[0].buttonMoveDownUp.Length; i++)
						{
							uiInteraction[0].buttonMoveDownUp[i].interactable = false;
						}
					}
					
					else if (GameManager.Instance.actualCamPreset.presetNumber == 3 || GameManager.Instance.actualCamPreset.presetNumber == 4)
					{
						uiInteraction[1].buttonGoToBloc.interactable = true;
						for (int i = 0; i < uiInteraction[1].buttonMoveDownUp.Length; i++)
						{
							uiInteraction[1].buttonMoveDownUp[i].interactable = false;
						}
					}
					

				}

				//If the current block group if below or above the player pos
				if (blockGroupParentPos.y + 2.5f - currentPlayerPos.y > -0.1f &&
				    blockGroupParentPos.y + 2.5f - currentPlayerPos.y < 0.1f)
				{
					if (GameManager.Instance.actualCamPreset.presetNumber == 1 ||
					    GameManager.Instance.actualCamPreset.presetNumber == 2)
						uiInteraction[0].buttonGoToBloc.interactable = true;
					else if (GameManager.Instance.actualCamPreset.presetNumber == 3 ||
					         GameManager.Instance.actualCamPreset.presetNumber == 4)
						uiInteraction[1].buttonGoToBloc.interactable = true;
				}
				else
				{
					if (GameManager.Instance.actualCamPreset.presetNumber == 1 ||
					    GameManager.Instance.actualCamPreset.presetNumber == 2)
						uiInteraction[0].buttonGoToBloc.interactable = false;
					else if (GameManager.Instance.actualCamPreset.presetNumber == 3 ||
					         GameManager.Instance.actualCamPreset.presetNumber == 4)
						uiInteraction[1].buttonGoToBloc.interactable = false;
				}

				if (GameManager.Instance.currentPlayerTurn.currentBlockPlayerOn == blockCurrentlySelected.transform)
				{
					if (GameManager.Instance.actualCamPreset.presetNumber == 1 ||
					    GameManager.Instance.actualCamPreset.presetNumber == 2)
						uiInteraction[0].buttonGoToBloc.interactable = false;
					for (int i = 0; i < uiInteraction[0].buttonMoveDownUp.Length; i++)
						uiInteraction[0].buttonMoveDownUp[i].interactable = true;
				}
				else if (GameManager.Instance.actualCamPreset.presetNumber == 3 ||
				         GameManager.Instance.actualCamPreset.presetNumber == 4)
				{
					uiInteraction[1].buttonGoToBloc.interactable = false;
					for (int i = 0; i < uiInteraction[1].buttonMoveDownUp.Length; i++)
						uiInteraction[1].buttonMoveDownUp[i].interactable = true;
				}
				
			}
		}
	}

}