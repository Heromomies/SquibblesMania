using System;
using System.Collections;
using System.Collections.Generic;
using DigitalRubyShared;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraButtonManager : MonoBehaviour
{
    private Transform _cam;
    [SerializeField] private Transform target;


    private static CameraButtonManager _cameraManager;

    public static CameraButtonManager Instance => _cameraManager;

    [Header("UI CAM BUTTONS")] [SerializeField]
    private GameObject[] uiButtonCams;


    [SerializeField] private GameObject[] buttonViewMode;

    private void OnEnable()
    {
        foreach (GameObject uiButton in uiButtonCams)
        {
            uiButton.SetActive(true);
            FingersScript.Instance.PassThroughObjects.Add(uiButton);
        }
    }

    private void OnDisable()
    {
        foreach (GameObject uiButton in uiButtonCams)
        {
            uiButton.SetActive(false);
            FingersScript.Instance.PassThroughObjects.Remove(uiButton);
        }
    }

    private void Awake()
    {
        _cameraManager = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _cam = Camera.main.transform;
        ResetBaseViewButton();
    }

    public void ResetBaseViewButton()
    {
        buttonViewMode[0].SetActive(false);
        buttonViewMode[2].SetActive(false);
    }

    public void TopViewMode()
    {
        int presetNumber = GameManager.Instance.actualCamPreset.presetNumber;
        if (presetNumber == 1 || presetNumber == 2)
        {
            buttonViewMode[0].SetActive(true);
            buttonViewMode[1].SetActive(false);
            _cam.RotateAround(target.transform.position, _cam.right, 45f);
        }
        else if (presetNumber == 3 || presetNumber == 4)
        {
            buttonViewMode[2].SetActive(true);
            buttonViewMode[3].SetActive(false);
            _cam.RotateAround(target.transform.position, _cam.right, -45f);
        }

        
    }

    public void BaseViewMode()
    {
        int presetNumber = GameManager.Instance.actualCamPreset.presetNumber;
        if (presetNumber == 1 || presetNumber == 2)
        {
            buttonViewMode[0].SetActive(false);
            buttonViewMode[1].SetActive(true);
            _cam.RotateAround(target.transform.position, _cam.right, -45f);
        }
        else if (presetNumber == 3 || presetNumber == 4)
        {
            buttonViewMode[2].SetActive(false);
            buttonViewMode[3].SetActive(true);
            _cam.RotateAround(target.transform.position, _cam.right, 45f);
        }

        
    }


    public void RotateCamera(float angle)
    {
        _cam.RotateAround(target.transform.position, Vector3.up, angle);
    }
}