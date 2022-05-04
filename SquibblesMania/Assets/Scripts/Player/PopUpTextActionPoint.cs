using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUpTextActionPoint : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro text;
    [SerializeField] private Transform cam;
    private int _actionPoint;
    private int _actionPointMax;

    private void Awake()
    {
       
        if (Camera.main)
            cam = Camera.main.transform;
    }

    private void OnEnable()
    {
        if (GameManager.Instance.currentPlayerTurn != null)
        {
            if(GameManager.Instance.currentPlayerTurn.isPlayerInActionCardState)
                _actionPointMax = GameManager.Instance.currentPlayerTurn.playerActionPoint;
        }
       
    }

    public void SetUpText(int value)
    {
        _actionPoint = value;
        text.SetText(_actionPoint + "/" + _actionPointMax);
    }

    void LateUpdate()
    {
        if (gameObject.activeSelf)
        {
            transform.LookAt(transform.position + cam.forward);
        }
    }
}