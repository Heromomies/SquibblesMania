using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiPopUpTween : MonoBehaviour
{
    [SerializeField] private RectTransform uiRectTransform;
    [SerializeField] private float timeInSecondsScaleAnim = 0.3f;
    [SerializeField] private LeanTweenType scaleEaseType;
    [SerializeField] private Vector3 scaleDesired;
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!uiRectTransform)
        {
            uiRectTransform = GetComponent<RectTransform>();
        }

        if (scaleDesired == Vector3.zero)
        {
            scaleDesired = uiRectTransform.transform.localScale;
        }

       
    }

#endif

    private void OnEnable()
    {
        SetScale(Vector3.zero);
        LeanTween.scale(uiRectTransform, scaleDesired, timeInSecondsScaleAnim).setEase(scaleEaseType);
    }
    
    private void OnDisable()
    {
        SetScale(Vector3.zero);
    }

    private void SetScale(Vector3 scale)
    {
        uiRectTransform.transform.localScale = scale;
    }
}