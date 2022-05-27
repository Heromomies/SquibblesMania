using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPlayerStunTween : MonoBehaviour
{
    private RectTransform _rectTransformText;
    [SerializeField] private float tweenTimeInSeconds = 0.5f;
    [SerializeField] private float rotateAmount = -45f;

#if UNITY_EDITOR
    private void OnValidate()
    {
        _rectTransformText = GetComponent<RectTransform>();
    }

#endif
    
   private void OnEnable()
    {
        LeanTween.move(_rectTransformText, Vector3.down, tweenTimeInSeconds).setEasePunch();
        LeanTween.rotateZ(_rectTransformText.gameObject, rotateAmount, tweenTimeInSeconds).setEasePunch();
    }
}
