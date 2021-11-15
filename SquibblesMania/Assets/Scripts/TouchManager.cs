using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DigitalRubyShared;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchManager : MonoBehaviour
{
    private readonly List<RaycastResult> raycast = new List<RaycastResult>();
    [SerializeField] private Canvas canvas;
    private PanGestureRecognizer platformeMoveGesture;

    [SerializeField] private Transform platform;
    [SerializeField] private GameObject[] buttonScale;


    private void OnEnable()
    {
        platformeMoveGesture = new PanGestureRecognizer();
        platformeMoveGesture.ThresholdUnits = 0.0f; // start right away
        platformeMoveGesture.StateUpdated += PlatformeGestureUpdated;
        FingersScript.Instance.AddGesture(platformeMoveGesture);
    }

    private void PlatformeGestureUpdated(GestureRecognizer gesture)
    {
        //Debug.Log(gesture.State);

        if (gesture.State == GestureRecognizerState.Began)
        {
            PointerEventData p = new PointerEventData(EventSystem.current);
            p.position = new Vector2(gesture.FocusX, gesture.FocusY);
            raycast.Clear();
            EventSystem.current.RaycastAll(p, raycast);

            foreach (RaycastResult result in raycast)
            {
                if (result.gameObject.CompareTag("Platforme"))
                {
                    platform = result.gameObject.transform;
                  
                    foreach (GameObject gameObject in buttonScale)
                    {
                        gameObject.SetActive(true);
                    }
                    
                    break;
                }
            }

            if (platform == null)
            {
                gesture.Reset();
            }
        }


        if (gesture.State == GestureRecognizerState.Executing)
        {
        }
        else if (gesture.State == GestureRecognizerState.Ended)
        {
        }
    }

    public void PlatformeUp()
    {
        if (platform.localScale.y >= 4)
        {
            return;
        }
        
       
        platform.DOScaleY(platform.localScale.y + 1, 0.25f);
        platform.DOMove(new Vector3(platform.position.x, platform.position.y + 0.5f, platform.position.z), 0.25f);
        
        foreach (GameObject gameObject in buttonScale)
        {
            gameObject.SetActive(false);
        }

        platform = null;
    }

    public void PlatformeDown()
    {
        if (platform.localScale.y <= 1)
        {
            return;
        }
        
        platform.DOScaleY(platform.localScale.y - 1, 0.25f);
        platform.DOMove(new Vector3(platform.position.x, platform.position.y - 0.5f, platform.position.z), 0.25f);
        
        foreach (GameObject gameObject in buttonScale)
        {
            gameObject.SetActive(false);
        }
        platform = null;
    }
}