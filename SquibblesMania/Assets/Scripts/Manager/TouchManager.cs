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
    public LayerMask touchLayersMask;
    private Camera _cam;
    public RaycastHit Hit;
    public GameObject blockCurrentlySelected;
   
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
                PlayerTurnActionStateSelectBlock();
            }

            else
            {
                //If player OnSelect the block, the block get his color back
                if (GameManager.Instance.currentPlayerTurn.isPlayerInActionCardState && MovementBlockManager.Instance.isMovingBlock)
                {
                    blockCurrentlySelected = null;
                }

                gesture.Reset();
                switch (GameManager.Instance.actualCamPreset.presetNumber)
                {
                    case 1: ResetUiButtonsInteraction(0); break;
                    case 2: ResetUiButtonsInteraction(0); break;
                    case 3: ResetUiButtonsInteraction(1); break;
                    case 4: ResetUiButtonsInteraction(1); break;
                }
            }
        }
    }

    private void ResetUiButtonsInteraction(int indexList)
    {
        uiInteraction[indexList].buttonGoToBloc.interactable = true;
        for (int i = 0; i < uiInteraction[indexList].buttonMoveDownUp.Length; i++)
        {
            uiInteraction[indexList].buttonMoveDownUp[i].interactable = true;
        }

        uiInteraction[indexList].uiInteractionParentObject.SetActive(false);
    }

    void PlayerTurnActionStateSelectBlock()
    {
        if (Hit.transform.gameObject.GetComponent<Node>() && !GameManager.Instance.currentPlayerTurn.walking && GameManager.Instance.currentPlayerTurn.isPlayerInActionCardState)
        {
            if (GameManager.Instance.currentPlayerTurn.nextBlockPath.Contains(Hit.transform) && GameManager.Instance.currentPlayerTurn.playerActionPoint > 0)
            {
                switch (GameManager.Instance.actualCamPreset.presetNumber)
                {
                    case 1: ResetUiButtonsInteraction(0); break;
                    case 2: ResetUiButtonsInteraction(0); break;
                    case 3: ResetUiButtonsInteraction(1); break;
                    case 4: ResetUiButtonsInteraction(1); break;
                }
                
                blockCurrentlySelected = Hit.transform.gameObject;
                blockParent = Hit.collider.gameObject.transform.parent;

            }
        }
    }
}