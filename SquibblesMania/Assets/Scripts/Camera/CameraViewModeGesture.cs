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
    private Transform _mapTarget;

    [Header("UI VIEW MODE SETTINGS")] public UIViewMode actualUiViewMode;
    public List<UIViewMode> uiViewModeList;

    [Header("CAM SETTINGS")] [Space(10f)] [SerializeField]
    private float camIconAngleZCameraTopView;
    [SerializeField] private float camIconAngleZCameraBaseView;
    [SerializeField] private float camIconAngleZCameraLowerView;
    [Space(10f)] [SerializeField] private float maxAngle;
    [SerializeField] private float minAngle;

    [Serializable]
    public struct UIViewMode
    {
        public List<Image> uiCircleSelection;
        public Transform uiCamIcon;
        public Transform parentUiCamViewMode;
    }

    private WaitForSeconds _timeInSecondsForSwitchViewMode = new WaitForSeconds(0.3f);

    [SerializeField] private float timeRotateSpeedInSeconds = 1f;
    [SerializeField] private Image currentUiCircleSelection;
    private int _indexUiCircleSelection;

    private void Awake()
    {
        _cam = Camera.main;
        _mapTarget = GameObject.FindGameObjectWithTag("Map").transform;
    }


    public void ChangeViewMode(int indexUiCircleSelection)
    {
        if (indexUiCircleSelection != this._indexUiCircleSelection)
        {
            foreach (var image in actualUiViewMode.uiCircleSelection)
            {
                var imageColor = image.color;
                imageColor.a = 0f;
                image.color = imageColor;
            }

            var uiCircleColor = actualUiViewMode.uiCircleSelection[indexUiCircleSelection].color;
            uiCircleColor.a = 1f;
            
            switch (indexUiCircleSelection)
            {
                case 0:
                    if (currentUiCircleSelection == actualUiViewMode.uiCircleSelection[2])
                    {
                        StartCoroutine(StartMovementViewModeStateCoroutine(indexUiCircleSelection, uiCircleColor, maxAngle, camIconAngleZCameraLowerView));
                    }

                    if (currentUiCircleSelection == actualUiViewMode.uiCircleSelection[1])
                    {
                        StartCoroutine(StartMovementViewModeStateCoroutine(indexUiCircleSelection, uiCircleColor, minAngle, camIconAngleZCameraLowerView));
                    }
                    break;
                
                case 1:
                    if (currentUiCircleSelection == actualUiViewMode.uiCircleSelection[0])
                    {
                        StartCoroutine(StartMovementViewModeStateCoroutine(indexUiCircleSelection, uiCircleColor, -minAngle,camIconAngleZCameraBaseView));
                    }

                    if (currentUiCircleSelection == actualUiViewMode.uiCircleSelection[2])
                    {
                        StartCoroutine(StartMovementViewModeStateCoroutine(indexUiCircleSelection, uiCircleColor, minAngle,camIconAngleZCameraBaseView));
                    }
                    break;

                case 2:
                    if (currentUiCircleSelection == actualUiViewMode.uiCircleSelection[0])
                    {
                        StartCoroutine(StartMovementViewModeStateCoroutine(indexUiCircleSelection, uiCircleColor, -maxAngle, camIconAngleZCameraTopView));
                    }

                    if (currentUiCircleSelection == actualUiViewMode.uiCircleSelection[1])
                    {
                        StartCoroutine(StartMovementViewModeStateCoroutine(indexUiCircleSelection, uiCircleColor, -minAngle, camIconAngleZCameraTopView));
                    }
                    break;
            }
        }
    }

    private IEnumerator StartMovementViewModeStateCoroutine(int indexUiCircleSelection, Color uiCircleColor, float angleRotation, float camIconAngleZ)
    {
        var camEulerAngles = actualUiViewMode.uiCamIcon.eulerAngles;
        actualUiViewMode.uiCircleSelection[indexUiCircleSelection].color = uiCircleColor;
        
        if (GameManager.Instance.actualCamPreset.presetNumber <= 2)
        {
            camEulerAngles.z = camIconAngleZ;
            actualUiViewMode.uiCamIcon.eulerAngles = camEulerAngles;
        }
        else
        {
            camEulerAngles.z = camIconAngleZ + 180f;
            actualUiViewMode.uiCamIcon.eulerAngles = camEulerAngles;
        }
        
        StartCoroutine(StartRotateCam(GameManager.Instance.actualCamPreset.presetNumber, angleRotation));
        this._indexUiCircleSelection = indexUiCircleSelection;
        currentUiCircleSelection = actualUiViewMode.uiCircleSelection[indexUiCircleSelection];
        yield return _timeInSecondsForSwitchViewMode;
    }

    private IEnumerator StartRotateCam(int presetCamNumber, float angleRotation)
    {
        var timeSinceStarted = 0f;
        var startEulerAnglesX = transform.eulerAngles.x;
        var camTransform = _cam.transform;

        if (presetCamNumber <= 2)
        {
            while (timeSinceStarted <= timeRotateSpeedInSeconds)
            {
                timeSinceStarted += Time.deltaTime;
                _cam.transform.RotateAround(_mapTarget.transform.position, _cam.transform.right,
                    -angleRotation * Time.deltaTime / timeRotateSpeedInSeconds);
                yield return null;
            }

            // We forced the value to applied to the y value of cam euler angles
            var camEulerAngles = _cam.transform.eulerAngles;
            camEulerAngles.x = startEulerAnglesX + -angleRotation;
            camTransform.eulerAngles = camEulerAngles;
        }
        else
        {
            while (timeSinceStarted <= timeRotateSpeedInSeconds)
            {
                timeSinceStarted += Time.deltaTime;
                _cam.transform.RotateAround(_mapTarget.transform.position, _cam.transform.right,
                    angleRotation * Time.deltaTime / timeRotateSpeedInSeconds);
                yield return null;
            }

            // We forced the value to applied to the y value of cam euler angles
            var camEulerAngles = _cam.transform.eulerAngles;
            camEulerAngles.x = startEulerAnglesX - angleRotation;
            camTransform.eulerAngles = camEulerAngles;
        }
    }

    /// <summary>
    /// When player switch we Setup the cam to its base view 
    /// </summary>
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
    }

    /// <summary>
    /// //Set up the ui game objects of the desiredUi view mode
    /// </summary>
    /// <param name="desiredUIViewMode"></param>
    private void SetObjectStateUiViewModeItem(UIViewMode desiredUIViewMode)
    {
        foreach (var uiViewMode in uiViewModeList)
        {
            uiViewMode.parentUiCamViewMode.gameObject.SetActive(false);
        }

        desiredUIViewMode.parentUiCamViewMode.gameObject.SetActive(true);
        actualUiViewMode = desiredUIViewMode;
        currentUiCircleSelection = actualUiViewMode.uiCircleSelection[0];
        _indexUiCircleSelection = 0;
    }
}