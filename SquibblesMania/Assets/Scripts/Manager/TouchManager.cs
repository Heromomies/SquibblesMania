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
    [Header("TOUCH MANAGER")] public GameObject uiScaleBlockParentObject;
    private Transform _blockParent;
    [SerializeField] private RectTransform canvasTransform;
    [SerializeField] private Vector3 offsetPos;
    public GameObject uiInteractionParentObject;
    [SerializeField] private Button buttonGoToTheBlock;

    public LayerMask touchLayersMask;
    private Camera _cam;
    [HideInInspector] public RaycastHit hit;
    private GameObject _blockCurrentlySelected;
    public Color blockCurrentlySelectedColor;

    private Vector3 _blockParentStartPos;
    private static TouchManager _touchManager;
    public bool isMovingBlock;
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
        FingersScript.Instance.PassThroughObjects.Add(uiScaleBlockParentObject);
        FingersScript.Instance.PassThroughObjects.Add(uiInteractionParentObject);
    }

    private void OnDisable()
    {
        if (FingersScript.HasInstance)
        {
            FingersScript.Instance.RemoveGesture(PlayerTouchGesture);
        }


        FingersScript.Instance.PassThroughObjects.Remove(uiScaleBlockParentObject);
        FingersScript.Instance.PassThroughObjects.Remove(uiInteractionParentObject);
        FingersScript.Instance.PassThroughObjects.Clear();
    }

    private void ResetPreviewPlatform()
    {
        if (_blockParent != null)
        {
            uiScaleBlockParentObject.SetActive(false);
        }
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

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, touchLayersMask))
            {
                if (_blockCurrentlySelected != null)
                {
                    //Previous selected block get his base color back

                    ResetPreviousBlockColor();
                    GameManager.Instance.isPathRefresh = true;
                }

                PlayerTurnActionStateSelectBlock();
            }

            else
            {
                //If player OnSelect the block, the block get his color back
                if (GameManager.Instance.currentPlayerTurn.isPlayerInActionCardState && isMovingBlock)
                {
                    ResetPreviousBlockColor();
                    GameManager.Instance.isPathRefresh = true;
                    _blockCurrentlySelected = null;
                }

                gesture.Reset();
                uiInteractionParentObject.SetActive(false);
            }
        }
    }

    void PlayerTurnActionStateSelectBlock()
    {
        if (hit.transform.gameObject.GetComponent<Node>() && !GameManager.Instance.currentPlayerTurn.walking &&
            GameManager.Instance.currentPlayerTurn.isPlayerInActionCardState)
        {
            if (GameManager.Instance.currentPlayerTurn.PlayerActionPointCardState.previewPath.Contains(hit.transform))
            {
                uiScaleBlockParentObject.SetActive(false);
                //Take the block group parent from hit block gameobject
                GroupBlockDetection blockGroupParent = hit.transform.parent.GetComponent<GroupBlockDetection>();
                _blockParentStartPos = hit.transform.parent.position;
                //Take the current player position
                Vector3 currentPlayerPos = GameManager.Instance.currentPlayerTurn.gameObject.transform.position;
                //Take the current block group selected position
                Vector3 blockGroupParentPos = blockGroupParent.gameObject.transform.position;
                //Change pos of canvas base on the current block selected
                canvasTransform.position = hit.transform.position + offsetPos;
                uiInteractionParentObject.SetActive(true);
                buttonGoToTheBlock.interactable = true;

                _blockCurrentlySelected = hit.transform.gameObject;
                //If the current block group if below or above the player pos
                if (blockGroupParentPos.y + 1 > currentPlayerPos.y || blockGroupParentPos.y + 1 < currentPlayerPos.y)
                {
                    buttonGoToTheBlock.interactable = false;
                }
            }
        }
    }


    public void PlatformeUp()
    {
        _blockParent = hit.collider.gameObject.transform.parent;

        GroupBlockDetection groupBlockDetection = _blockParent.GetComponent<GroupBlockDetection>();


        Vector3 positionBlockParent = _blockParent.position;

        if (_blockParent.position.y >= 4)
        {
            return;
        }


        _blockParent.DOMove(new Vector3(positionBlockParent.x, positionBlockParent.y + 1f, positionBlockParent.z),
            0.25f);

        positionBlockParent = _blockParent.position;
        //We compare if the startPos of the block parent is bigger than his actual pos
        if (_blockParentStartPos.y > positionBlockParent.y ||
            Math.Abs(_blockParentStartPos.y - positionBlockParent.y) < 0.1f)
        {
            //We want to substract action point from the current player if he move up/down the block
            GameManager.Instance.currentPlayerTurn.playerActionPoint--;

            if (GameManager.Instance.currentPlayerTurn.playerActionPoint <= 0)
            {
                GameManager.Instance.currentPlayerTurn.playerActionPoint = 0;
            }


            UiManager.Instance.SetUpCurrentActionPointOfCurrentPlayer(GameManager.Instance.currentPlayerTurn
                .playerActionPoint);
            GameManager.Instance.isPathRefresh = true;
        }
        else
        {
            //If the block going back to his initial pos we add action point to the player (it's as if he nullify his action)
            GameManager.Instance.currentPlayerTurn.playerActionPoint++;
            UiManager.Instance.SetUpCurrentActionPointOfCurrentPlayer(GameManager.Instance.currentPlayerTurn
                .playerActionPoint);
            GameManager.Instance.isPathRefresh = true;
        }


        //Move the player with block
        if (groupBlockDetection.playersOnGroupBlock.Count > 0)
        {
            foreach (Transform playerOnGroupBlock in groupBlockDetection.playersOnGroupBlock)
            {
                Vector3 playerOnGroupBlockPos = playerOnGroupBlock.position;
                playerOnGroupBlock.DOMove(
                    new Vector3(playerOnGroupBlockPos.x, playerOnGroupBlockPos.y + 1f, playerOnGroupBlockPos.z), 0.25f);
            }
        }

        ResetPreviewPlatform();
        isMovingBlock = false;
        _blockParent = null;
    }

    public void PlatformeDown()
    {
        _blockParent = hit.collider.gameObject.transform.parent;

        GroupBlockDetection groupBlockDetection = _blockParent.GetComponent<GroupBlockDetection>();
        if (_blockParent.position.y <= 0)
        {
            return;
        }


        Vector3 positionBlockParent = _blockParent.position;
        _blockParent.DOMove(new Vector3(positionBlockParent.x, positionBlockParent.y - 1f, positionBlockParent.z),
            0.25f);

        positionBlockParent = _blockParent.position;
        //We compare if the parent block position its bigger than his lastPos or equal 
        if (positionBlockParent.y > _blockParentStartPos.y ||
            Math.Abs(_blockParentStartPos.y - positionBlockParent.y) > 0.1f)
        {
            //We want to substract action point from the current player if he move up/down the block

            GameManager.Instance.currentPlayerTurn.playerActionPoint--;
            if (GameManager.Instance.currentPlayerTurn.playerActionPoint <= 0)
            {
                GameManager.Instance.currentPlayerTurn.playerActionPoint = 0;
            }

            UiManager.Instance.SetUpCurrentActionPointOfCurrentPlayer(GameManager.Instance.currentPlayerTurn
                .playerActionPoint);
            GameManager.Instance.isPathRefresh = true;
        }
        else
        {
            //If the block going back to his initial pos we add action point to the player (it's as if he nullify his action)
            GameManager.Instance.currentPlayerTurn.playerActionPoint++;
            UiManager.Instance.SetUpCurrentActionPointOfCurrentPlayer(GameManager.Instance.currentPlayerTurn
                .playerActionPoint);
            GameManager.Instance.isPathRefresh = true;
        }

        //Move the player with block
        if (groupBlockDetection.playersOnGroupBlock.Count > 0)
        {
            foreach (Transform playerOnGroupBlock in groupBlockDetection.playersOnGroupBlock)
            {
                Vector3 playerOnGroupBlockPos = playerOnGroupBlock.position;
                playerOnGroupBlock.DOMove(
                    new Vector3(playerOnGroupBlockPos.x, playerOnGroupBlockPos.y - 1f, playerOnGroupBlockPos.z), 0.25f);
            }
        }


        ResetPreviewPlatform();

        isMovingBlock = false;
        _blockParent = null;
    }

    /* void SelectedBlockColor(Color color)
     {
         Material blockCurrentlySelectedMat = _blockCurrentlySelected.GetComponent<Renderer>().material;
         blockCurrentlySelectedColor = blockCurrentlySelectedMat.color;
         blockCurrentlySelectedMat.color = color;
     }*/

    void ResetPreviousBlockColor()
    {
        Material blockCurrentlySelectedMat = _blockCurrentlySelected.GetComponent<Renderer>().material;

        blockCurrentlySelectedMat.color = blockCurrentlySelectedColor;
    }
}