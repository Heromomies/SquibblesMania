using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUpTextActionPoint : MonoBehaviour
{
    private TextMeshPro _text;
    [SerializeField] private Transform cam;

    void Awake()
    {
        _text = GetComponent<TextMeshPro>();
        cam = Camera.main.transform;
    }

    public void SetUpText(int value)
    {
        _text.SetText(value + "/" + GameManager.Instance.currentPlayerTurn.playerActionPoint);
    }

    void LateUpdate()
    {
        if (gameObject.activeSelf)
        {
            transform.LookAt(transform.position + cam.forward);
        }
    }
}