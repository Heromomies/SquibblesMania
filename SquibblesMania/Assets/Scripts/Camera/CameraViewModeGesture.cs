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
    

    private WaitForSeconds _timeInSecondsForSwitchViewMode = new WaitForSeconds(1.1f);

    [SerializeField] private float timeRotateSpeedInSeconds = 1f;
    [SerializeField] private Image currentUiCircleSelection;
    private int _indexUiCircleSelection;


  
    public List<SavedCamViewModeGesture> savedCamViewModeGestures = new List<SavedCamViewModeGesture>();

    
    [Serializable]
    public struct SavedCamViewModeGesture
    {
        public int lastIndexUiCircleSelection;
        public float lastCamIconAngleView;
    }

    private void Start()
    {
        _cam = Camera.main;
        _mapTarget = GameObject.FindGameObjectWithTag("Map").transform;
    }

    public void ChangeViewMode(int indexUiCircleSelection)
    {
        if (indexUiCircleSelection != _indexUiCircleSelection)
        {
            CameraButtonManager.Instance.isCamRotateButtonPressed = false;
            AudioManager.Instance.Play("Button");
            foreach (var image in actualUiViewMode.uiCircleSelection)
            {
                var imageColor = image.color;
                imageColor.a = 0f;
                image.color = imageColor;
                image.gameObject.SetActive(false);
            }

            actualUiViewMode.uiCircleSelection[indexUiCircleSelection].gameObject.SetActive(true);
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
                        StartCoroutine(StartMovementViewModeStateCoroutine(indexUiCircleSelection, uiCircleColor,
                            minAngle, camIconAngleZCameraBaseView));
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
        CheckIndexCamButtonManager(indexUiCircleSelection);
        actualUiViewMode.uiCircleSelection[indexUiCircleSelection].color = uiCircleColor;
        SetCamIconEulerAngles(actualUiViewMode.uiCamIcon.eulerAngles, camIconAngleZ);
        StartCoroutine(StartRotateCam(GameManager.Instance.actualCamPreset.presetNumber, angleRotation));
        
        _indexUiCircleSelection = indexUiCircleSelection;
        currentUiCircleSelection = actualUiViewMode.uiCircleSelection[indexUiCircleSelection];
        yield return _timeInSecondsForSwitchViewMode;
        foreach (var image in actualUiViewMode.uiCircleSelection)
        {
            image.gameObject.SetActive(true);
        }
    }

    void CheckIndexCamButtonManager(int indexUiCircleSelection)
    {
        if (indexUiCircleSelection >= 2)
        {
            foreach (var button in CameraButtonManager.Instance.actualUiCamPreset.buttonsCamRotate)
            {
                var eventTriggerButton = button.gameObject.GetComponent<EventTrigger>();
                if (button.interactable)
                {
                    button.interactable = false;
                    eventTriggerButton.enabled = false;
                }
                    
            }
        }
        else
        {
            foreach (var button in CameraButtonManager.Instance.actualUiCamPreset.buttonsCamRotate)
            {
                var eventTriggerButton = button.gameObject.GetComponent<EventTrigger>();
                if (!button.interactable)
                {
                    button.interactable = true;
                    eventTriggerButton.enabled = true;
                }
            }
        }
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
    public void SetUpCameraViewMode(bool isFirstTurn, int index)
    {
        int actualCamPresetNumber = GameManager.Instance.actualCamPreset.presetNumber;
        if (isFirstTurn)
        {
            if (actualCamPresetNumber <= 2)
            {
                SetObjectStateUiViewModeItem(uiViewModeList[0], index);
            }
            else
            {
                 SetObjectStateUiViewModeItem(uiViewModeList[1], index);
            }
          
        }
        else
        {
            if (actualCamPresetNumber <= 2)
            {
                SetObjectStateUiViewModeItem(uiViewModeList[0], savedCamViewModeGestures[index].lastIndexUiCircleSelection);
            }
            else
            {
                SetObjectStateUiViewModeItem(uiViewModeList[1], savedCamViewModeGestures[index].lastIndexUiCircleSelection);
            }
        }
        
      

      
    }

    /// <summary>
    /// //Set up the ui game objects of the desiredUi view mode
    /// </summary>
    /// <param name="desiredUIViewMode"></param>
    private void SetObjectStateUiViewModeItem(UIViewMode desiredUIViewMode, int indexCircleSelection)
    {
        foreach (var uiViewMode in uiViewModeList)
        {
            uiViewMode.parentUiCamViewMode.gameObject.SetActive(false);
        }
        
        
        desiredUIViewMode.parentUiCamViewMode.gameObject.SetActive(true);
        actualUiViewMode = desiredUIViewMode;
        
        CheckIndexCamButtonManager(indexCircleSelection);
        switch (indexCircleSelection)
        {
            case 0:
                SetCamIconEulerAngles(actualUiViewMode.uiCamIcon.eulerAngles, camIconAngleZCameraLowerView);
                break;
            case 1:
                SetCamIconEulerAngles(actualUiViewMode.uiCamIcon.eulerAngles, camIconAngleZCameraBaseView);
                break;
            case 2:
                SetCamIconEulerAngles(actualUiViewMode.uiCamIcon.eulerAngles, camIconAngleZCameraTopView);
                break;
        }
        
        if (currentUiCircleSelection != null)
        {
           UiCircleSelectionColor(currentUiCircleSelection.color, 0);
        }

        ResetUICirclesSelectionColor(actualUiViewMode.uiCircleSelection, 0);
        currentUiCircleSelection = actualUiViewMode.uiCircleSelection[indexCircleSelection];
        UiCircleSelectionColor(currentUiCircleSelection.color, 1);
        _indexUiCircleSelection = indexCircleSelection;
    }

    void UiCircleSelectionColor(Color color, float alphaValue)
    {
        color.a = alphaValue;
        currentUiCircleSelection.color = color;
    }

    void ResetUICirclesSelectionColor(List<Image> images, float alphaValue)
    {
        foreach (var img in images)
        {
            Color color = img.color;
            color.a = alphaValue;
            img.color = color;
        }
    }

   private void SetCamIconEulerAngles(Vector3 camEulerAngles, float desiredAngle)
    {
        if (GameManager.Instance.actualCamPreset.presetNumber <= 2)
        {
            camEulerAngles.z = desiredAngle;
            actualUiViewMode.uiCamIcon.eulerAngles = camEulerAngles;
        }
        else
        {
            camEulerAngles.z = desiredAngle + 180f;
            actualUiViewMode.uiCamIcon.eulerAngles = camEulerAngles;
        }
    }

   public void SavePreviousViewModeGesture(int countTurn)
   {
       var savedCamViewModeGesture = savedCamViewModeGestures[countTurn];
       savedCamViewModeGesture.lastCamIconAngleView = actualUiViewMode.uiCamIcon.eulerAngles.z;
       savedCamViewModeGesture.lastIndexUiCircleSelection = _indexUiCircleSelection;
       savedCamViewModeGestures[countTurn] = savedCamViewModeGesture;
   }
}