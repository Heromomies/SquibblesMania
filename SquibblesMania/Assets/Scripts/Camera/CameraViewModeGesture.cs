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
    private Transform _mapTarget;

    [Header("SWIPE SETTINGS")] [Range(1, 10), SerializeField]
    private int swipeTouchCount = 1;

    [Range(0.0f, 10.0f), SerializeField] private float swipeThresholdSeconds;
    [Range(0.0f, 1.0f), SerializeField] private float minimumDistanceUnits = 0.2f;

    public UIViewMode actualUiViewMode;
    public List<UIViewMode> uiViewModeList;

    [Serializable]
    public struct UIViewMode
    {
        public List<Image> uiCircleSelection;
        public Transform uiCamIcon;
        public Transform uiCursorSelection;
        public Transform parentUiCamViewMode;
    }

    private WaitForSeconds _timeInSecondsForSwitchViewMode = new WaitForSeconds(0.3f);

    [Header("CURSOR SETTINGS")] [SerializeField]
    private bool isCursorSelected;

    [SerializeField] private float scaleCursorAmount = 1.2f;
   
    private Transform _currentCircleTransform;
    private List<RaycastResult> _raycast = new List<RaycastResult>();
    [SerializeField] private List<RectTransform> uiCircleRectTransforms;
    private RaycastHit _hit;
    [SerializeField] private GraphicRaycaster graphicRaycaster;

    private void Awake()
    {
        _cam = Camera.main;
        _mapTarget = GameObject.FindGameObjectWithTag("Map").transform;
    }

    private void OnEnable()
    {
        //Set up the new gesture 
        _swipeViewModeGesture = new SwipeGestureRecognizer();
        _swipeViewModeGesture.StateUpdated += SwipeViewModeGestureOnStateUpdated;
        _swipeViewModeGesture.DirectionThreshold = 0;
        _swipeViewModeGesture.MinimumNumberOfTouchesToTrack =
            _swipeViewModeGesture.MaximumNumberOfTouchesToTrack = swipeTouchCount;
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

    private void OnDisable()
    {
        if (FingersScript.HasInstance)
        {
            FingersScript.Instance.RemoveGesture(_longPressViewModeGesture);
            FingersScript.Instance.RemoveGesture(_swipeViewModeGesture);
        }
    }

    private void SwipeViewModeGestureOnStateUpdated(GestureRecognizer gesture)
    {
        SwipeGestureRecognizer swipeGestureRecognizer = gesture as SwipeGestureRecognizer;
        if (gesture.State == GestureRecognizerState.Ended && isCursorSelected)
        {
            var cursorEulerAngles = actualUiViewMode.uiCursorSelection.eulerAngles;
            var camIconEulerAngles = actualUiViewMode.uiCamIcon.transform.eulerAngles;
            
            if (!_currentCircleTransform)
            {
                _currentCircleTransform = uiCircleRectTransforms[1].transform;
            }

            if (GameManager.Instance.actualCamPreset.presetNumber <= 2)
            {
                switch (swipeGestureRecognizer.EndDirection)
                {
                    case SwipeGestureRecognizerDirection.Down:
                      
                        if (_currentCircleTransform == uiCircleRectTransforms[0].transform)
                        {
                            StartCoroutine(StartMovementViewModeStateCoroutine(uiCircleRectTransforms[1].transform, new Vector2(-56f, 0), cursorEulerAngles, camIconEulerAngles, 60f, -30f));
                        }
                        else
                        {
                            StartCoroutine(StartMovementViewModeStateCoroutine(uiCircleRectTransforms[2].transform, new Vector2(-40, 0), cursorEulerAngles, camIconEulerAngles, 20f, -10f));
                        }
                        break;
                    case SwipeGestureRecognizerDirection.Up:
                        if (_currentCircleTransform == uiCircleRectTransforms[1].transform)
                        {
                            StartCoroutine(StartMovementViewModeStateCoroutine(uiCircleRectTransforms[0].transform,
                                new Vector2(0, 40), cursorEulerAngles, camIconEulerAngles, -60f, -90f, -90f));
                        }
                        else
                        {
                            StartCoroutine(StartMovementViewModeStateCoroutine(uiCircleRectTransforms[1].transform,
                                new Vector2(-56f, 0), cursorEulerAngles, camIconEulerAngles, -20f, -30f));
                        }
                        break;
                }
            }
            else
            {
                switch (swipeGestureRecognizer.EndDirection)
                {
                    case SwipeGestureRecognizerDirection.Down:
                        
                        if (_currentCircleTransform == uiCircleRectTransforms[1].transform)
                        {
                            StartCoroutine(StartMovementViewModeStateCoroutine(uiCircleRectTransforms[0].transform, new Vector2(0, -40), cursorEulerAngles, camIconEulerAngles, -60f, 90f, 90f));
                        }
                        else
                        {
                            StartCoroutine(StartMovementViewModeStateCoroutine(uiCircleRectTransforms[1].transform, new Vector2(56f, 0), cursorEulerAngles, camIconEulerAngles, 20f, 210f, 180f));
                        }
                        break;
                        
                    case SwipeGestureRecognizerDirection.Up:
                        if (_currentCircleTransform == uiCircleRectTransforms[0].transform)
                        {
                            StartCoroutine(StartMovementViewModeStateCoroutine(uiCircleRectTransforms[1].transform, new Vector2(56f, 0), cursorEulerAngles, camIconEulerAngles, 60f, -210f, 180f));
                        }
                        else
                        {
                            StartCoroutine(StartMovementViewModeStateCoroutine(uiCircleRectTransforms[2].transform, new Vector2(40, 0), cursorEulerAngles, camIconEulerAngles, -20f, -190f, 180f));
                        }
                        break;
                       
                }
            }
            
            
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
                    actualUiViewMode.uiCursorSelection = result.gameObject.transform;
                    isCursorSelected = true;
                    break;
                }
            }

            if (isCursorSelected)
            {
                actualUiViewMode.uiCursorSelection.transform.localScale = Vector3.one * scaleCursorAmount;
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

    private IEnumerator StartMovementViewModeStateCoroutine(Transform uiCircleRectTransform, Vector2 offset, Vector3 cursorEulerAngles, Vector3 camIconEulerAngles, float angleToAddCamRotate, float camIconAngleZ = 0f, float angleZ = 0f)
    {
        if (_currentCircleTransform)
        {
            _currentCircleTransform.gameObject.SetActive(false);
        }

        _currentCircleTransform = uiCircleRectTransform;
        uiCircleRectTransform.gameObject.SetActive(true);
        cursorEulerAngles.z = angleZ;
        actualUiViewMode.uiCursorSelection.eulerAngles = cursorEulerAngles;
        actualUiViewMode.uiCursorSelection.transform.position = uiCircleRectTransform.position + (Vector3)offset;
        camIconEulerAngles.z = camIconAngleZ;
        actualUiViewMode.uiCamIcon.eulerAngles = camIconEulerAngles;
        isCursorSelected = false;
        StartRotateCam(GameManager.Instance.actualCamPreset.presetNumber, angleToAddCamRotate);
        yield return _timeInSecondsForSwitchViewMode;
        isCursorSelected = true;
    }

    private void StartRotateCam(int presetCamNumber, float angleRotation)
    {
        if (presetCamNumber <= 2)
        {
            _cam.transform.RotateAround(_mapTarget.position, _cam.transform.right, -angleRotation);
        }
        else
        {
            _cam.transform.RotateAround(_mapTarget.position, _cam.transform.right, angleRotation);
        }
    }

    private void ClearStartState()
    {
        isCursorSelected = false;
        if (actualUiViewMode.uiCursorSelection != null)
        { 
            actualUiViewMode.uiCursorSelection.localScale = Vector3.one;
        }
        uiCircleRectTransforms.Clear();
    }

    public void SetUpCameraBaseViewMode()
    {
        int actualCamPresetNumber = GameManager.Instance.actualCamPreset.presetNumber;
        
        if (actualCamPresetNumber <= 2)
        {
            SetObjectStateUiViewModeItem(uiViewModeList[0]);
        }
        else
        {
            SetObjectStateUiViewModeItem(uiViewModeList[1]);
        }
        _longPressViewModeGesture.PlatformSpecificView = actualUiViewMode.uiCursorSelection;
    }

    private void SetObjectStateUiViewModeItem(UIViewMode desiredUIViewMode)
    {
        
        foreach (var uiViewMode in uiViewModeList)
        {
            uiViewMode.parentUiCamViewMode.gameObject.SetActive(false);
        }
        
        desiredUIViewMode.parentUiCamViewMode.gameObject.SetActive(true);
        actualUiViewMode = desiredUIViewMode;
    }
   
}