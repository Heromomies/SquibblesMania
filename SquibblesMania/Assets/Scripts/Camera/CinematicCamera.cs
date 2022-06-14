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
    public GameObject parentCinematic;
    
    private static float _timeCinematic = 5f;
    private WaitForSeconds _timeBeforeStopCinematic = new WaitForSeconds(_timeCinematic);
    private CameraButtonManager _cameraButtonManager;
    private CameraViewModeGesture _cameraViewModeGesture;
    private Vector3 _orbitCam;
    public float radius = 10f;
    
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        _cameraButtonManager = GetComponent<CameraButtonManager>();
        _cameraViewModeGesture = GetComponent<CameraViewModeGesture>();

        _cameraButtonManager.enabled = false;
        _cameraViewModeGesture.enabled = false;

        _orbitCam = -Vector3.forward * radius;
        
        StartCoroutine(CameraMovement());
    }


    // Update is called once per frame
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

        yield return _timeBeforeStopCinematic;

        _cameraButtonManager.enabled = true;
        _cameraViewModeGesture.enabled = true;
        
        AudioManager.Instance.Play("MainSound");
        
        foreach (var o in objectsToDeactivate)
        {
            o.SetActive(true);
        }
        
        StartCoroutine(cinematicbars.HideBar());
        _cinematic = false;
        parentCinematic.SetActive(false);
        //transform.LookAt(CameraButtonManager.Instance.target);
    }
}
