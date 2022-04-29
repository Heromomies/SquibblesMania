using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteArrowTween : MonoBehaviour
{

    private RectTransform _spriteArrow;
    [SerializeField] private float tweenTimeInSeconds = 0.5f;
    [SerializeField] private Vector3 tweenScale = new Vector3(1.2f, 1.2f, 1.2f);
    
    private void Awake()
    {
        _spriteArrow = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        LeanTween.scale(_spriteArrow, tweenScale, tweenTimeInSeconds).setLoopPingPong();
    }
}
