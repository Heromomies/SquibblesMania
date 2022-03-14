using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DigitalRubyShared;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

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

    [SerializeField] private List<Image> uiCamSquare;
    [SerializeField] private int indexUiCamSquareList;
    
    private void Awake()
    {
        _cameraManager = this;
        _cam = Camera.main.transform;
    }

    private void Start()
    {
        indexUiCamSquareList++;
        SetUpSquareIcon(indexUiCamSquareList);
    }

    public void StartRotateCam(float rotateAmount)
    {
        if (rotateAmount < 0.0f)
        {
            indexUiCamSquareList++;
            if (indexUiCamSquareList > uiCamSquare.Count-1) indexUiCamSquareList = 0;
            SetUpSquareIcon(indexUiCamSquareList);
        }
        else
        {
            indexUiCamSquareList--;
            if (indexUiCamSquareList < 0) indexUiCamSquareList = uiCamSquare.Count - 1;
            SetUpSquareIcon(indexUiCamSquareList);
        }
        
        StartCoroutine(RotateCamCoroutine(rotateAmount));
        EnableCamRotateButtons();
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
                    EnableCamRotateButtons();                    
                    yield break;
                }

                // Otherwise, continue next frame
                yield return null;
            }
        }
    }

    private void EnableCamRotateButtons()
    {
        foreach (var button in buttonsCamRotate)
        {
            button.enabled = !button.enabled;
        }
    }

    private void SetUpSquareIcon(int indexSquareIcon)
    {
      Transform childIconSelected = uiCamSquare[indexSquareIcon].transform.GetChild(0);
      childIconSelected.gameObject.SetActive(true);
      var uiCamSquareColor =  uiCamSquare[indexSquareIcon].color;
      uiCamSquareColor.a = 1f;
      uiCamSquare[indexSquareIcon].color = uiCamSquareColor;
      
      foreach (var iconImage in uiCamSquare)
      {
          if (iconImage != uiCamSquare[indexSquareIcon])
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
}