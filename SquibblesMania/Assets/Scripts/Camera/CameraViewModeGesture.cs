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
    private SwipeGestureRecognizer _swipeViewModeGesture = new SwipeGestureRecognizer();
    [Header("SWIPE SETTINGS")]
    [Range(1, 10)] public int swipeTouchCount = 1;
    [Range(0.0f, 10.0f)] public float swipeThresholdSeconds;
    [Range(0.0f, 1.0f)] public float minimumDistanceUnits = 0.2f;
    
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

    private WaitForSeconds _timeInSecondsForSwitchViewMode = new WaitForSeconds(0.3f);
    [Header("CURSOR SETTINGS")] private Transform _currentSelectedCursor;
    private Transform _currentCircleTransform;
    [SerializeField] private bool isCursorSelected;
    [SerializeField] private float scaleAmount = 1.2f;
    private List<RaycastResult> _raycast = new List<RaycastResult>();
    [SerializeField] private List<RectTransform> uiCircleRectTransforms;
    private RaycastHit _hit;
    [SerializeField] private GraphicRaycaster graphicRaycaster;

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void OnEnable()
    {
        //Set up the new gesture 
        _swipeViewModeGesture = new SwipeGestureRecognizer();
        _swipeViewModeGesture.StateUpdated += SwipeViewModeGestureOnStateUpdated;
        _swipeViewModeGesture.DirectionThreshold = 0;
        _swipeViewModeGesture.MinimumNumberOfTouchesToTrack = _swipeViewModeGesture.MaximumNumberOfTouchesToTrack = swipeTouchCount;
        _swipeViewModeGesture.ThresholdSeconds = swipeThresholdSeconds;
        _swipeViewModeGesture.MinimumDistanceUnits = minimumDistanceUnits;
        _swipeViewModeGesture.EndMode = SwipeGestureRecognizerEndMode.EndContinusously;
        _swipeViewModeGesture.AllowSimultaneousExecution(_longPressViewModeGesture);
        FingersScript.Instance.AddGesture(_swipeViewModeGesture);
        
        
        //Set up the new gesture 
        _longPressViewModeGesture = new LongPressGestureRecognizer();
        _longPressViewModeGesture.PlatformSpecificView = actualUiViewMode.uiCursorSelection;
        _longPressViewModeGesture.StateUpdated += LongPressViewModeGestureOnStateUpdated;
        _longPressViewModeGesture.ThresholdUnits = 0.0f;
        _longPressViewModeGesture.MinimumDurationSeconds = 0.2f;
        FingersScript.Instance.AddGesture(_longPressViewModeGesture);
    }

    private void SwipeViewModeGestureOnStateUpdated(GestureRecognizer gesture)
    {
        SwipeGestureRecognizer swipeGestureRecognizer = gesture as SwipeGestureRecognizer;
        if (gesture.State == GestureRecognizerState.Ended)
        {
            if (isCursorSelected)
            {
                var cursorEulerAngles = _currentSelectedCursor.eulerAngles;
                if (!_currentCircleTransform)
                {
                    _currentCircleTransform = uiCircleRectTransforms[1].transform;
                }
                switch (swipeGestureRecognizer.EndDirection)
                {
                    case SwipeGestureRecognizerDirection.Up:
                        if (_currentCircleTransform == uiCircleRectTransforms[1].transform)
                        {
                            StartCoroutine(StartMovementViewModeStateCoroutine(uiCircleRectTransforms[0].transform,  new Vector2(0,40),cursorEulerAngles, -90f));
                        }
                        else
                        {
                            StartCoroutine(StartMovementViewModeStateCoroutine(uiCircleRectTransforms[1].transform,  new Vector2(-56f,0),cursorEulerAngles));
                        }
                        break;
                    case SwipeGestureRecognizerDirection.Down:
                        if (_currentCircleTransform == uiCircleRectTransforms[0].transform)
                        {
                            StartCoroutine(StartMovementViewModeStateCoroutine(uiCircleRectTransforms[1].transform,  new Vector2(-56f,0),cursorEulerAngles ));
                        }
                        else
                        {
                            StartCoroutine(StartMovementViewModeStateCoroutine(uiCircleRectTransforms[2].transform,  new Vector2(-40,0),cursorEulerAngles));
                        }
                        break;
                }
               
             
               
            }
        }
        
    }

    private void OnDisable()
    {
        if (FingersScript.HasInstance)
        {
            FingersScript.Instance.RemoveGesture(_longPressViewModeGesture);
            FingersScript.Instance.RemoveGesture(_swipeViewModeGesture);
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
                    _currentSelectedCursor = result.gameObject.transform;
                    isCursorSelected = true;
                    break;
                }
            }

            if (isCursorSelected)
            {
                _currentSelectedCursor.transform.localScale = Vector3.one * scaleAmount;
                foreach (var circle in actualUiViewMode.uiCircleSelection)
                {
                    uiCircleRectTransforms.Add(circle.GetComponent<RectTransform>());
                }
            }

            _raycast.Clear();
        }
        
        else if (gesture.State == GestureRecognizerState.Ended)
        {
            ClearStartState();
            gesture.Reset();
        }
    }

    private IEnumerator StartMovementViewModeStateCoroutine(Transform uiCircleRectTransform, Vector2 offset, Vector3 cursorEulerAngles, float angleZ = 0f)
    { 
        _currentCircleTransform = uiCircleRectTransform;
        cursorEulerAngles.z = angleZ;
        _currentSelectedCursor.eulerAngles = cursorEulerAngles;
        _currentSelectedCursor.transform.position = uiCircleRectTransform.position + (Vector3)offset;
        isCursorSelected = false;
        yield return _timeInSecondsForSwitchViewMode;
        isCursorSelected = true;
    }


    private void ClearStartState()
    {
        isCursorSelected = false;
        _currentSelectedCursor.transform.localScale = Vector3.one;
        _currentSelectedCursor = null;
        uiCircleRectTransforms.Clear();
    }

    public void SetUpUIViewMode()
    {
        _longPressViewModeGesture.PlatformSpecificView = actualUiViewMode.uiCursorSelection;
    }
}