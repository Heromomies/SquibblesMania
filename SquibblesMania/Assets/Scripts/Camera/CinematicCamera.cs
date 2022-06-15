using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicCamera : MonoBehaviour
{

    private bool _cinematic;
    public CinematicBars cinematicbars;
    public List<GameObject> objectsToDeactivate;
    public float speedCam;
    public float radius = 10f;
    
    [Header("CINEMATIC SETTINGS")]
    public GameObject parentCinematic;
    
    
    private static float timeCinematicInSeconds = 5f;
    private WaitForSeconds cinematicDurationWaitForSeconds = new WaitForSeconds(timeCinematicInSeconds);
    
    [SerializeField]
    private CameraButtonManager cameraButtonManager;
    [SerializeField]
    private CameraViewModeGesture cameraViewModeGesture;
    private Vector3 _orbitCam;

    
    private static float cinematicDelayInSeconds = 0.2f;
    private WaitForSeconds cinematicDelayWaitForSeconds = new WaitForSeconds(cinematicDelayInSeconds);
    private void Awake()
    {
        _cinematic = this;
        StartCoroutine(StartCinematicCoroutine());
    }
    
    
    private IEnumerator StartCinematicCoroutine()
    {
        yield return cinematicDelayWaitForSeconds;

        cameraButtonManager.enabled = false;
        cameraViewModeGesture.enabled = false;

        _orbitCam = -Vector3.forward * radius;
        
        StartCoroutine(CameraMovement());
    }

    
    void Update()
    {
        if(_cinematic)
        {
            CamRotation(Vector3.up, _orbitCam, transform.eulerAngles);
        }
    }
    
    private void CamRotation(Vector3 worldUp, Vector3 orbit, Vector3 camEulerAngles)
    {
        orbit = Quaternion.Euler(camEulerAngles.x, camEulerAngles.y + speedCam, camEulerAngles.z) * orbit;

        transform.position = CameraButtonManager.Instance.target.transform.position + orbit;

        if (GameManager.Instance.actualCamPreset.presetNumber <= 2)
        {
            transform.LookAt(CameraButtonManager.Instance.target.position, worldUp);
        }
        else
        {
            transform.LookAt(CameraButtonManager.Instance.target.position, -worldUp);
        }
    }

    IEnumerator CameraMovement()
    {
        foreach (var o in objectsToDeactivate)
        {
            o.SetActive(false);
        }
       
        StartCoroutine(cinematicbars.ShowBar());
        _cinematic = true;

        yield return cinematicDurationWaitForSeconds;

        cameraButtonManager.enabled = true;
        cameraViewModeGesture.enabled = true;
        
        AudioManager.Instance.Play("MainSound");
        
        foreach (var o in objectsToDeactivate)
        {
            o.SetActive(true);
        }
        
        StartCoroutine(cinematicbars.HideBar());
        _cinematic = false;
        parentCinematic.SetActive(false);
       
    }
}
