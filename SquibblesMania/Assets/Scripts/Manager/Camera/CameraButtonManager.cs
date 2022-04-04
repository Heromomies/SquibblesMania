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

public class CameraButtonManager : MonoBehaviour
{
  
    public Transform target;

    private static CameraButtonManager _cameraManager;
    public static CameraButtonManager Instance => _cameraManager;
    
    [SerializeField] private float radius = 10;
    
    [SerializeField] private GameObject[] uiCamPresets;
    [SerializeField] private bool isCamRotateButtonPressed;
    [SerializeField] private float targetAngle;
    private void Awake()
    {
        _cameraManager = this;
    }

    public void TogglePressed(bool value)
    {
        isCamRotateButtonPressed = value;
    }

    private void Update()
    {
        if (isCamRotateButtonPressed)
        {
            Vector3 worldUp = Vector3.up;
            Vector3 orbit = -Vector3.forward * radius;
            var camEulerAngles = transform.eulerAngles;
            orbit = Quaternion.Euler(camEulerAngles.x, camEulerAngles.y + (targetAngle * Time.deltaTime) , camEulerAngles.z) * orbit;
            
            
            transform.position = target.transform.position + orbit;

            if (GameManager.Instance.actualCamPreset.presetNumber <= 2)
            {
                transform.LookAt(target.position, worldUp);
            }
            else
            {
                transform.LookAt(target.position, -worldUp);
            }
            
            
        }
    }

    public void StartRotateCam(float rotateAmount)
    {
        AudioManager.Instance.Play("Button");
        targetAngle = rotateAmount;
        
    }

    public void SetUpUiCamPreset()
    {
        // Set up cam ui buttons
        
        foreach (var uiCamPreset in uiCamPresets) uiCamPreset.SetActive(false);

        if (GameManager.Instance.actualCamPreset.presetNumber <= 2)
        {
            uiCamPresets[0].SetActive(true);
        }
        else
        {
            uiCamPresets[1].SetActive(true);
        }
    }
    

   /* private void SetUpSquareIcon(int indexSquareIcon)
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
    }*/
     
    

   /* public void TopViewMode()
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
    }*/

  
}