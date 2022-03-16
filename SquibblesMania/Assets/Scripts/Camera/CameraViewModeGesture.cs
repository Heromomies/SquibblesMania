using System;
using System.Collections;
using System.Collections.Generic;
using DigitalRubyShared;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TouchPhase = DigitalRubyShared.TouchPhase;

public class CameraViewModeGesture : MonoBehaviour
{
    private Camera _cam;
    private LongPressGestureRecognizer _longPressViewModeGesture = new LongPressGestureRecognizer();

    public UIViewMode actualUiViewMode;
    public List<UIViewMode> uiViewModeList;

    [Serializable]
    public struct UIViewMode
    {
        public List<Image> uiCircleSelection;
        public Transform uiCamIcon;
        public Transform uiCursorSelection;

        private Vector3 _startCamIconRot => uiCamIcon.eulerAngles;
        private Vector3 _startCursorSelectionPos => uiCursorSelection.position;
    }


    [Header("CURSOR SETTINGS")] [SerializeField]
    private Transform currentSelectedCursor;

    [SerializeField] private bool isCursorSelected;
    [SerializeField] private float scaleAmount = 1.2f;
    private List<RaycastResult> _raycast = new List<RaycastResult>();
    private RaycastHit _hit;
    [SerializeField]
    private GraphicRaycaster graphicRaycaster;
    private void Awake()
    {
        _cam = Camera.main;
    }

    private void OnEnable()
    {
        //Set up the new gesture 
        _longPressViewModeGesture = new LongPressGestureRecognizer();
        _longPressViewModeGesture.PlatformSpecificView = actualUiViewMode.uiCursorSelection;
        _longPressViewModeGesture.StateUpdated += LongPressViewModeGestureOnStateUpdated;
        _longPressViewModeGesture.ThresholdUnits = 0.0f;
        _longPressViewModeGesture.MinimumDurationSeconds = 0.2f;
        FingersScript.Instance.AddGesture(_longPressViewModeGesture);
    }

    private void OnDisable()
    {
        if (FingersScript.HasInstance)
        {
            FingersScript.Instance.RemoveGesture(_longPressViewModeGesture);
        }
    }

    private void LongPressViewModeGestureOnStateUpdated(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Began)
        {
            //Set up the new Pointer Event
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            List<RaycastResult> results = new List<RaycastResult>();
            
            //Raycast using the Graphics Raycaster and mouse click position
            pointerData.position = Input.mousePosition;
            graphicRaycaster.Raycast(pointerData, results);
            
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.transform == actualUiViewMode.uiCursorSelection)
                {
                    currentSelectedCursor = result.gameObject.transform;
                    isCursorSelected = true;
                    break;
                }
            }

            if (isCursorSelected)
            {
                currentSelectedCursor.transform.localScale = Vector3.one * scaleAmount;
            }
            
            
            _raycast.Clear();
            
        }
        else if (gesture.State == GestureRecognizerState.Ended)
        {
           ClearStartState();
           gesture.Reset();
        }
    }





private void ClearStartState()
{
    isCursorSelected = false;
    currentSelectedCursor.transform.localScale = Vector3.one;
    currentSelectedCursor = null;
}

public void SetUpUIViewMode()
{
    _longPressViewModeGesture.PlatformSpecificView = actualUiViewMode.uiCursorSelection;
}


}