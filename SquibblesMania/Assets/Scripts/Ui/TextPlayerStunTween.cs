using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPlayerStunTween : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectTransformText;
    [SerializeField] private float tweenTimeInSeconds = 0.5f;
    [SerializeField] private float rotateAmount = -45f;
    
    
   private void OnEnable()
    {
        LeanTween.move(rectTransformText, Vector3.down, tweenTimeInSeconds).setEasePunch();
        LeanTween.rotateZ(rectTransformText.gameObject, rotateAmount, tweenTimeInSeconds).setEasePunch();
    }
}
