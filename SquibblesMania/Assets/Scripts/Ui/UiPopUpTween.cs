using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiPopUpTween : MonoBehaviour
{
    [SerializeField] private RectTransform textRectTransform;
    [SerializeField] private float timeInSecondsScaleAnim = 0.3f;
    [SerializeField] private LeanTweenType scaleEaseType;
    [SerializeField] private Vector3 scaleDesired;
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!textRectTransform)
        {
            textRectTransform = GetComponent<RectTransform>();
        }

        if (scaleDesired == Vector3.zero)
        {
            scaleDesired = textRectTransform.transform.localScale;
        }

       
    }

#endif

    private void OnEnable()
    {
        SetScale(Vector3.zero);
        LeanTween.scale(textRectTransform, scaleDesired, timeInSecondsScaleAnim).setEase(scaleEaseType);
    }
    
    private void OnDisable()
    {
        SetScale(Vector3.zero);
    }

    private void SetScale(Vector3 scale)
    {
        textRectTransform.transform.localScale = scale;
    }
}