using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DigitalRubyShared;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchManager : MonoBehaviour
{
    private readonly List<RaycastResult> raycast = new List<RaycastResult>();


    public PanGestureRecognizer PlayerTouchGesture { get; private set; }

    [SerializeField] private GameObject uiScaleBlockParentObject;

    private Transform _blockParent;
    [SerializeField] private RectTransform canvasTransform;
    [SerializeField] private Vector3 offsetPos;
    [SerializeField] GameObject uiInteractionParentObject;
    [SerializeField] private GameObject buttonGoToTheBlock;

    public LayerMask touchLayersMask;
    private Camera _cam;
    private RaycastHit hit;

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
                if (hit.transform.gameObject.GetComponent<Node>() && !PlayerController.Instance.walking)
                {
                    //Take the block group parent from hit block gameobject
                    GroupBlockDetection blockGroupParent = hit.transform.parent.GetComponent<GroupBlockDetection>();
                    //Take the current player position
                    Vector3 currentPlayerPos = PlayerController.Instance.gameObject.transform.position;
                    //Take the current block group selected position
                    Vector3 blockGroupParentPos = blockGroupParent.gameObject.transform.position;
                    //Change pos of canvas base on the current block selected
                    canvasTransform.position = hit.transform.position + offsetPos;
                    uiInteractionParentObject.SetActive(true);
                    buttonGoToTheBlock.SetActive(true);

                    //If the current block group if below or above the player pos
                    if (blockGroupParentPos.y + 1 > currentPlayerPos.y ||
                        blockGroupParentPos.y + 1 < currentPlayerPos.y)
                    {
                        buttonGoToTheBlock.SetActive(false);
                    }
                }
            }

            else
            {
                gesture.Reset();
            }
        }
    }

    public void ButtonGoToBlock()
    {
        PlayerController.Instance.currentTouchBlock = hit.transform;
        PlayerController.Instance.StartPathFinding();
        uiInteractionParentObject.SetActive(false);
    }

    public void ButtonUpDown()
    {
        uiInteractionParentObject.SetActive(false);
        uiScaleBlockParentObject.SetActive(true);
    }

    public void PlatformeUp()
    {
        _blockParent = hit.collider.gameObject.transform.parent;

        GroupBlockDetection groupBlockDetection = _blockParent.GetComponent<GroupBlockDetection>();


        if (_blockParent.position.y >= 4)
        {
            return;
        }


        Vector3 positionBlockParent = _blockParent.position;
        _blockParent.DOMove(new Vector3(positionBlockParent.x, positionBlockParent.y + 1f, positionBlockParent.z),
            0.25f);
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


        var positionBlockParent = _blockParent.position;
        _blockParent.DOMove(new Vector3(positionBlockParent.x, positionBlockParent.y - 1f, positionBlockParent.z),
            0.25f);

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


        _blockParent = null;
    }
}