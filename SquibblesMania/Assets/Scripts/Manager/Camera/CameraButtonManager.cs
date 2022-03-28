using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DigitalRubyShared;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class CameraButtonManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Transform _cam;
    public Transform target;

    private static CameraButtonManager _cameraManager;
    public static CameraButtonManager Instance => _cameraManager;
    
    
    [SerializeField] private int indexUiCamSquareList;
    [SerializeField] private int baseIndexUICamSquare = 2;
    public UICamPresets actualUiCamPreset;
    public List<UICamPresets> uiCamPresetList;
    [SerializeField]
    private float timeRotateSpeedInSeconds = 0.5f;

    [SerializeField] private float radius = 10;
    [Serializable]
    public struct UICamPresets
    {
        public List<Image> uiCamSquare;
        public Button[] buttonsCamRotate;
    }
    
    [SerializeField] private GameObject[] uiCamPresets;
    [SerializeField] private bool isCamRotateButtonPressed;
    [SerializeField] private float targetAngle;
    private void Awake()
    {
        _cameraManager = this;
        _cam = Camera.main.transform;
    }

    public void TogglePressed(bool value)
    {
        isCamRotateButtonPressed = value;
    }

    private void Update()
    {
        /*if (isCamRotateButtonPressed)
        {
            Vector3 orbit = -Vector3.forward * radius;
            var camEulerAngles = transform.eulerAngles;
            orbit = Quaternion.Euler(camEulerAngles.x, camEulerAngles.y + targetAngle , camEulerAngles.z) * orbit;
            transform.position = target.transform.position + orbit;
            transform.LookAt(target.position);
        }*/
    }

    public void StartRotateCam(float rotateAmount)
    {
        AudioManager.Instance.Play("Button");
        if (rotateAmount < 0f)
        {
            if (indexUiCamSquareList >= actualUiCamPreset.uiCamSquare.Count - 1)
            {
                indexUiCamSquareList = 0;
                SetUpSquareIcon(indexUiCamSquareList);
            }
            else
            {
                indexUiCamSquareList++;
                SetUpSquareIcon(indexUiCamSquareList);
            }
            
        }
        else
        {
            if (indexUiCamSquareList <= 0)
            {
                indexUiCamSquareList = actualUiCamPreset.uiCamSquare.Count - 1;
                SetUpSquareIcon(indexUiCamSquareList);
            }
            else
            {
                indexUiCamSquareList--;
                SetUpSquareIcon(indexUiCamSquareList);
            }
          
        }

        targetAngle = rotateAmount;
        StartCoroutine(RotateCamCoroutine(rotateAmount));
        EnableCamRotateButtons(rotateAmount);
    }

    public void SetUpUiCamPreset()
    {
        // Set up cam ui buttons
        
        foreach (var uiCamPreset in uiCamPresets) uiCamPreset.SetActive(false);

        if (GameManager.Instance.actualCamPreset.presetNumber <= 2)
        {
            uiCamPresets[0].SetActive(true);
            actualUiCamPreset = uiCamPresetList[0];
        }
        else
        {
            uiCamPresets[1].SetActive(true);
            actualUiCamPreset = uiCamPresetList[1];
        }
        indexUiCamSquareList = baseIndexUICamSquare;
        SetUpSquareIcon(indexUiCamSquareList);
    }
 
    private IEnumerator RotateCamCoroutine(float angle)
    {
        Vector3 worldUp = Vector3.up;
        Vector3 orbit = -Vector3.forward * radius;
        
        var camEulerAngles = transform.eulerAngles;
        orbit = Quaternion.Euler(camEulerAngles.x, camEulerAngles.y + angle, camEulerAngles.z) * orbit;
        transform.position = target.transform.position + orbit;
        if (GameManager.Instance.actualCamPreset.presetNumber <= 2)
        {
            transform.LookAt(target.position, worldUp);
        }
        else
        {
            transform.LookAt(target.position, -worldUp);
        }
      
        yield return null;
        EnableCamRotateButtons(angle);
    }
    
    private void EnableCamRotateButtons(float rotateIndexAmount)
    {
        //Set buttons interactable or not interactable
        if (rotateIndexAmount < 0f)
        {
            actualUiCamPreset.buttonsCamRotate[1].interactable = !actualUiCamPreset.buttonsCamRotate[1].interactable;
        }
        else
        {
            actualUiCamPreset.buttonsCamRotate[0].interactable = !actualUiCamPreset.buttonsCamRotate[0].interactable;
        }
        
    }

    private void SetUpSquareIcon(int indexSquareIcon)
    {
        Transform childIconSelected = actualUiCamPreset.uiCamSquare[indexSquareIcon].transform.GetChild(0);
        childIconSelected.gameObject.SetActive(true);
        var uiCamSquareColor = actualUiCamPreset.uiCamSquare[indexSquareIcon].color;
        uiCamSquareColor.a = 1f;
        actualUiCamPreset.uiCamSquare[indexSquareIcon].color = uiCamSquareColor;
      
        foreach (var iconImage in actualUiCamPreset.uiCamSquare)
        {
            if (iconImage != actualUiCamPreset.uiCamSquare[indexSquareIcon])
            {
                var iconImageColor = iconImage.color;
                iconImageColor.a = 0.75f;
                iconImage.color = iconImageColor;

                Transform childIconImage = iconImage.transform.GetChild(0);
                childIconImage.gameObject.SetActive(false);
            }
        }
    }
     
    

    public void TopViewMode()
    {
        int presetNumber = GameManager.Instance.actualCamPreset.presetNumber;
        if (presetNumber == 1 || presetNumber == 2) _cam.RotateAround(target.transform.position, _cam.right, 45f);
        else if (presetNumber == 3 || presetNumber == 4) _cam.RotateAround(target.transform.position, _cam.right, -45f);
    }

    public void BaseViewMode()
    {
        int presetNumber = GameManager.Instance.actualCamPreset.presetNumber;
        if (presetNumber == 1 || presetNumber == 2) _cam.RotateAround(target.transform.position, _cam.right, -45f);
        else if (presetNumber == 3 || presetNumber == 4) _cam.RotateAround(target.transform.position, _cam.right, 45f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isCamRotateButtonPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isCamRotateButtonPressed = false;
    }
}