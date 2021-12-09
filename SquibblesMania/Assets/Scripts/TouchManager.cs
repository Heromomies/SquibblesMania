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

    [SerializeField] private GameObject uiButtonScale;


    [SerializeField] private RectTransform canvasTransform;
    [SerializeField] private Vector3 offsetPos;
    [SerializeField] GameObject interactionParentObject;

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
        //On ajoute une nouvelle gesture
        PlayerTouchGesture.StateUpdated += PlayerTouchGestureUpdated;
        PlayerTouchGesture.AllowSimultaneousExecutionWithAllGestures();

        FingersScript.Instance.AddGesture(PlayerTouchGesture);
        //On permet a la gesture de fonctionner à travers certains objets
        FingersScript.Instance.PassThroughObjects.Add(uiButtonScale);
        FingersScript.Instance.PassThroughObjects.Add(interactionParentObject);
    }

    private void OnDisable()
    {
        if (FingersScript.HasInstance)
        {
            FingersScript.Instance.RemoveGesture(PlayerTouchGesture);
        }


        FingersScript.Instance.PassThroughObjects.Remove(uiButtonScale);
        FingersScript.Instance.PassThroughObjects.Remove(interactionParentObject);
        FingersScript.Instance.PassThroughObjects.Clear();
    }

    /* private void ResetPreviewPlatform()
     {
         if (platform != null)
         {
             platform.gameObject.GetComponent<Renderer>().material.color = _platformBaseColor;
             
             foreach (GameObject gameObject in uiButtonScale)
             {
                 gameObject.SetActive(false);
             }
         }
         
     }*/
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
                if (hit.transform.gameObject.GetComponent<Node>())
                {
                    canvasTransform.position = hit.transform.position + offsetPos;
                    interactionParentObject.SetActive(true);
                }
            }


            else
            {
                gesture.Reset();
            }
        }


        if (gesture.State == GestureRecognizerState.Executing)
        {
        }
        else if (gesture.State == GestureRecognizerState.Ended)
        {
        }
    }

    public void ButtonGoToBlock()
    {
        PlayerController.Instance.currentTouchBlock = hit.transform;
        PlayerController.Instance.StartPathFinding();
        interactionParentObject.SetActive(false);
    }

    public void ButtonUpDown()
    {
        interactionParentObject.SetActive(false);
        uiButtonScale.SetActive(true);
    }

    public void PlatformeUp()
    {
        Transform blockParent = hit.collider.gameObject.transform.parent;

        if (blockParent.localScale.y >= 4)
        {
            return;
        }


        Vector3 positionBlockParent = blockParent.position;
        blockParent.DOMove(new Vector3(positionBlockParent.x, positionBlockParent.y + 1f, positionBlockParent.z),
            0.25f);


        blockParent = null;
    }

    public void PlatformeDown()
    {
        Transform blockParent = hit.collider.gameObject.transform.parent;
        if (blockParent.localScale.y <= 1)
        {
            return;
        }

        
        var positionBlockParent = blockParent.position;
        blockParent.DOMove(new Vector3(positionBlockParent.x, positionBlockParent.y - 1f, positionBlockParent.z),
            0.25f);


        blockParent = null;
    }
}