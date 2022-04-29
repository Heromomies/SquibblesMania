using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPlayerStunTween : MonoBehaviour
{
    private RectTransform _rectTransformText;
    [SerializeField] private float tweenTimeInSeconds = 0.5f;
    [SerializeField] private float rotateAmount = -45f;
    private void Awake()
    {
        _rectTransformText = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        LeanTween.move(_rectTransformText, Vector3.down, tweenTimeInSeconds).setEasePunch();
        LeanTween.rotateZ(_rectTransformText.gameObject, rotateAmount, tweenTimeInSeconds).setEasePunch();
    }
}
