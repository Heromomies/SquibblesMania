using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWinConditionsTween : MonoBehaviour
{

    public Image itemParentUi;

    [SerializeField] private float timeInSecondsColorTween = 0.7f;
    public void OnEnable()
    {
      
    }

    private void Start()
    {
        LeanTween.value(itemParentUi.gameObject, SetColorCallBack, Color.white, Color.green, timeInSecondsColorTween).setLoopPingPong();
    }

    private void SetColorCallBack(Color color)
    {
        itemParentUi.color = color;
    }
}
