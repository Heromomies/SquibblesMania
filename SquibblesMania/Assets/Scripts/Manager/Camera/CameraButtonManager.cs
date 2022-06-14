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
    public UICamPresets actualUiCamPreset;
    public List<UICamPresets> uiCamPresetList;
    [Serializable]
    public struct UICamPresets
    {
        public Button[] buttonsCamRotate;
    }
    
    [SerializeField] private float radius = 10;
    
    [SerializeField] private GameObject[] uiCamPresets;
    public bool isCamRotateButtonPressed;
    [SerializeField] private float targetAngle;
    private Vector3 _orbitCam;
    
    private void Awake()
    {
        _cameraManager = this;
        _orbitCam = -Vector3.forward * radius;
        if (target!= null)
        {
            CamRotation(Vector3.up, _orbitCam, transform.eulerAngles);
        }
    }
      

    public void TogglePressed(bool value)
    {
        isCamRotateButtonPressed = value;
    }

    private void LateUpdate()
    {
        if (isCamRotateButtonPressed)
        {
            CamRotation(Vector3.up, _orbitCam, transform.eulerAngles);
        }
    }

    private void CamRotation(Vector3 worldUp, Vector3 orbit, Vector3 camEulerAngles)
    {
        orbit = Quaternion.Euler(camEulerAngles.x, camEulerAngles.y + targetAngle, camEulerAngles.z) * orbit;
        
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

    public void StartRotateCam(float rotateAmount)
    {
        targetAngle = rotateAmount;
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
        foreach (var button in actualUiCamPreset.buttonsCamRotate)
        {
            button.interactable = true;
            button.gameObject.GetComponent<EventTrigger>().enabled = true;
        }
        
    }
}


  
