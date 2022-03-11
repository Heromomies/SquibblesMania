using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DigitalRubyShared;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class CameraButtonManager : MonoBehaviour
{
    private Transform _cam;
    public Transform target;

    private static CameraButtonManager _cameraManager;
    public static CameraButtonManager Instance => _cameraManager;
    [SerializeField] private bool isCamRotate;

    private float _timeRotateSpeedInSeconds = 0.5f;
    [SerializeField]
    private Button[] buttonsCamRotate;
    private void Awake()
    {
        _cameraManager = this;
        _cam = Camera.main.transform;
    }

    public void StartRotateCam(float rotateAmount)
    {
        StartCoroutine(RotateCamCoroutine(rotateAmount));
        foreach (var button in buttonsCamRotate)
        {
            button.enabled = !button.enabled;
        }
    }

    private IEnumerator RotateCamCoroutine(float rotateAmount)
    {
        float timeSinceStarted = 0f;
        if (ActualCamPreset.CamPresetTeam() == ActualCamPreset.Team.TeamOne)
        {
            while (true)
            {
                timeSinceStarted += Time.deltaTime * _timeRotateSpeedInSeconds;
                _cam.transform.RotateAround(target.transform.position, Vector3.up, rotateAmount * Time.deltaTime);
                // If the object has arrived, stop the coroutine
                if (timeSinceStarted >= 0.45f)
                {
                    Debug.Log("Object arrived");
                    foreach (var button in buttonsCamRotate)
                    {
                        button.enabled = !button.enabled;
                    }
                    yield break;
                }

                // Otherwise, continue next frame
                yield return null;
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
}