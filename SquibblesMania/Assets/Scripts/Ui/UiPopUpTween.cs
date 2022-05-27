using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiPopUpTween : MonoBehaviour
{
    [SerializeField] private RectTransform textRectTransform;
    [SerializeField] private float timeInSecondsScaleAnim = 0.3f;
    [SerializeField] private LeanTweenType scaleEaseType;
    private Vector3 _scaleDesired;

    private Menu _menu;
#if UNITY_EDITOR
    private void OnValidate()
    {
        textRectTransform = GetComponent<RectTransform>();
        _scaleDesired = textRectTransform.transform.localScale;
        _menu = FindObjectOfType<Menu>();
    }

#endif

    private void OnEnable()
    {
        SetScale(Vector3.zero);
        LeanTween.scale(textRectTransform, _scaleDesired, timeInSecondsScaleAnim).setEase(scaleEaseType);

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