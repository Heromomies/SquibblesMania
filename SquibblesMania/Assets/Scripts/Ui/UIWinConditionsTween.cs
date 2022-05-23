using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWinConditionsTween : MonoBehaviour
{

    public Image itemParentUi;
    
    [SerializeField] private float timeInSecondsColorTween = 1f;
    
    public Color endZoneItemColor;
    [SerializeField] private LeanTweenType easeType;
    
   
   
    private void Start()
    {
        TweenColor();
    }

    private void TweenColor()
    {
        LeanTween.value(itemParentUi.gameObject, Color.white, endZoneItemColor, timeInSecondsColorTween).setOnUpdateColor((color) =>
            { itemParentUi.color = color;
            }).setEase(easeType).setOnComplete(TweenColor);
    }

   
    
}
