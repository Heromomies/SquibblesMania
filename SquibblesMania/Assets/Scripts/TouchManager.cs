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
    public PanGestureRecognizer platformeMoveGesture { get; private set; }

    [SerializeField] private Transform platform;
    [SerializeField] private GameObject[] uiButtonScale;

    private Color _platformBaseColor;

    private Vector3 startpos;
    
    private void OnEnable()
    {
        platformeMoveGesture = new PanGestureRecognizer();

        platformeMoveGesture.ThresholdUnits = 0.0f; // start right away
        //On ajoute une nouvelle gesture
        platformeMoveGesture.StateUpdated += PlatformeGestureUpdated;
        platformeMoveGesture.AllowSimultaneousExecutionWithAllGestures();

        FingersScript.Instance.AddGesture(platformeMoveGesture);
        //On permet a la gesture de fonctionner à travers certains objets
        foreach (GameObject gameObject in uiButtonScale)
        {
            FingersScript.Instance.PassThroughObjects.Add(gameObject);
        }
    }

    private void OnDisable()
    {
        if (FingersScript.HasInstance)
        {
            FingersScript.Instance.RemoveGesture(platformeMoveGesture);
        }

        foreach (GameObject gameObject in uiButtonScale)
        {
            FingersScript.Instance.PassThroughObjects.Remove(gameObject);
            FingersScript.Instance.PassThroughObjects.Clear();
        }
    }

    private void ResetPreviewPlatform()
    {
        if (platform != null)
        {
            platform.gameObject.GetComponent<Renderer>().material.color = _platformBaseColor;
            
            foreach (GameObject gameObject in uiButtonScale)
            {
                gameObject.SetActive(false);
            }
        }
        
    }
    private void PlatformeGestureUpdated(GestureRecognizer gesture)
    {

        if (gesture.State == GestureRecognizerState.Began)
        {
            PointerEventData p = new PointerEventData(EventSystem.current);
            p.position = new Vector2(gesture.FocusX, gesture.FocusY);
            raycast.Clear();
            EventSystem.current.RaycastAll(p, raycast);

            var ray = Camera.main.ScreenPointToRay(p.position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.red, 3);
                if (hit.transform.gameObject.CompareTag("Platform"))
                {
                    //On reset la platform précédente
                    ResetPreviewPlatform();
                    platform = hit.transform.gameObject.transform;
                    
                    _platformBaseColor = platform.gameObject.GetComponent<Renderer>().material.color;
                    hit.transform.gameObject.GetComponent<Renderer>().material.color = Color.white;
                    foreach (GameObject gameObject in uiButtonScale)
                    {
                        gameObject.SetActive(true);
                    }
                    
                }
            }

            /*foreach (RaycastResult result in raycast)
            {
                startpos = result.screenPosition;
                Debug.DrawLine(result.screenPosition, result.worldPosition, Color.red, 3);
                if (result.gameObject.CompareTag("Platform"))
                {
                    //On reset la platform précédente
                    ResetPreviewPlatform();
                    platform = result.gameObject.transform;
                    
                    _platformBaseColor = platform.gameObject.GetComponent<Renderer>().material.color;
                    result.gameObject.GetComponent<Renderer>().material.color = Color.white;
                    foreach (GameObject gameObject in uiButtonScale)
                    {
                        gameObject.SetActive(true);
                    }
                    
                }
              
            }*/

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
        
        ResetPreviewPlatform();
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

        
        ResetPreviewPlatform();
        platform = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(startpos, 0.3f);
    }
}