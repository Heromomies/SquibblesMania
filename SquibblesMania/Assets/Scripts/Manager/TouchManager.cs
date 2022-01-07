using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DigitalRubyShared;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchManager : MonoBehaviour
{
    private readonly List<RaycastResult> raycast = new List<RaycastResult>();


    public PanGestureRecognizer PlayerTouchGesture { get; private set; }
    [Header("TOUCH MANAGER")] public Transform blockParent;
    [SerializeField] private RectTransform canvasTransform;
    [SerializeField] private Vector3 offsetPos;
    public GameObject uiInteractionParentObject;
    [SerializeField] private Button buttonGoToTheBlock;

    public LayerMask touchLayersMask;
    private Camera _cam;
    public RaycastHit Hit;
    public GameObject blockCurrentlySelected;
    public Color blockCurrentlySelectedColor;

    private static TouchManager _touchManager;
   
    public static TouchManager Instance => _touchManager;

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
        FingersScript.Instance.PassThroughObjects.Add(MovementBlockManager.Instance.buttonMoveBlockParentObject);
        FingersScript.Instance.PassThroughObjects.Add(uiInteractionParentObject);
        FingersScript.Instance.PassThroughObjects.Add(UiManager.Instance.buttonNextTurn);
    }

    private void OnDisable()
    {
        if (FingersScript.HasInstance)
        {
            FingersScript.Instance.RemoveGesture(PlayerTouchGesture);
        }


        FingersScript.Instance.PassThroughObjects.Remove(MovementBlockManager.Instance.buttonMoveBlockParentObject);
        FingersScript.Instance.PassThroughObjects.Remove(uiInteractionParentObject);
        FingersScript.Instance.PassThroughObjects.Remove(UiManager.Instance.buttonNextTurn);
        FingersScript.Instance.PassThroughObjects.Clear();
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
                if (blockCurrentlySelected != null)
                {
                    //Previous selected block get his base color back

                    MovementBlockManager.Instance.ResetPreviousBlockColor();
                    GameManager.Instance.isPathRefresh = true;
                }

                PlayerTurnActionStateSelectBlock();
            }

            else
            {
                //If player OnSelect the block, the block get his color back
                if (GameManager.Instance.currentPlayerTurn.isPlayerInActionCardState && MovementBlockManager.Instance.isMovingBlock)
                {
                    MovementBlockManager.Instance.ResetPreviousBlockColor();
                    GameManager.Instance.isPathRefresh = true;
                    blockCurrentlySelected = null;
                }

                gesture.Reset();
                uiInteractionParentObject.SetActive(false);
            }
        }
    }

    void PlayerTurnActionStateSelectBlock()
    {
        if (Hit.transform.gameObject.GetComponent<Node>() && !GameManager.Instance.currentPlayerTurn.walking &&
            GameManager.Instance.currentPlayerTurn.isPlayerInActionCardState)
        {
            if (GameManager.Instance.currentPlayerTurn.PlayerActionPointCardState.previewPath.Contains(Hit.transform) &&
                GameManager.Instance.currentPlayerTurn.playerActionPoint > 0)
            {
                MovementBlockManager.Instance.buttonMoveBlockParentObject.SetActive(false);
                //Take the block group parent from hit block gameobject
                GroupBlockDetection blockGroupParent = Hit.transform.parent.GetComponent<GroupBlockDetection>();

                //Take the current player position
                Vector3 currentPlayerPos = GameManager.Instance.currentPlayerTurn.gameObject.transform.position;
                //Take the current block group selected position
                Vector3 blockGroupParentPos = blockGroupParent.gameObject.transform.position;
                //Change pos of canvas base on the current block selected
                canvasTransform.position = Hit.transform.position + offsetPos;
                uiInteractionParentObject.SetActive(true);
                buttonGoToTheBlock.interactable = true;

                blockCurrentlySelected = Hit.transform.gameObject;
                //If the current block group if below or above the player pos
                if (blockGroupParentPos.y + 1 > currentPlayerPos.y || blockGroupParentPos.y + 1 < currentPlayerPos.y)
                {
                    buttonGoToTheBlock.interactable = false;
                }
            }
        }
    }
}