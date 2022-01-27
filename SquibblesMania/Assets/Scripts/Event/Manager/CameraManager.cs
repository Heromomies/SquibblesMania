using System;
using System.Collections;
using System.Collections.Generic;
using DigitalRubyShared;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraManager : MonoBehaviour
{
    private Transform _cam;
    [SerializeField] private Transform target;


    private static CameraManager _cameraManager;

    public static CameraManager Instance => _cameraManager;
    [Header("UI CAM BUTTONS")]

    [SerializeField] private GameObject[] uiButtonCams;


    [SerializeField] public GameObject buttonTopViewMode, buttonBaseViewMode;
    
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
        buttonBaseViewMode.SetActive(false);
    }


    public void TopViewMode()
    {
        buttonBaseViewMode.SetActive(true);
        buttonTopViewMode.SetActive(false);
        _cam.RotateAround(target.transform.position, _cam.right, 45f);
    }

    public void BaseViewMode()
    {
        buttonBaseViewMode.SetActive(false);
        buttonTopViewMode.SetActive(true);
        _cam.RotateAround(target.transform.position, _cam.right, -45f);
    }

    public void RotateCameraRight()
    {
        _cam.RotateAround(target.transform.position, Vector3.up, 90f);
    }


    public void RotateCameraLeft()
    {
        _cam.RotateAround(target.transform.position, Vector3.up, -90f);
    }
}