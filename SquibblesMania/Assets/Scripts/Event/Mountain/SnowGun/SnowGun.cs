using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DigitalRubyShared;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class SnowGun : MonoBehaviour, IManageEvent
{
    public GameObject snowPrefab;

    [Range(0.0f, 0.1f)] public float speed;
    //[Range(0.0f, 10.0f)] public float speedRotationSnowGun;
    [Range(0.0f, 10.0f)] public float ySpawn;
    [Range(0.0f, 10.0f)] public float radius;
    public GameObject hatchDetectPlayerNearSnowGun;
    public LayerMask playerLayerMask;
    public LayerMask layerInteractable;
    
    public AnimationCurve curve;

    [Header("TEXT SETTINGS")]
    public GameObject goToAntennaTxt;
    public GameObject shootPlayerTxt;

    public Collider[] col;
    
    [HideInInspector] public Animator animatorSnowGun;
    [HideInInspector] public List<Vector3> listPoint = new List<Vector3>();
    private List<GameObject> _hatchesList = new List<GameObject>();
    private readonly List<RaycastResult> _raycast = new List<RaycastResult>();
    public PanGestureRecognizer SwapTouchGesture { get; private set; }
    private Camera _cam;
    [HideInInspector] public bool canClick;
    private static readonly Vector3 vectorSpawnAntenna = new Vector3(0, 1.05f, 0);
    
    private void OnEnable()
    {
        SwapTouchGesture = new PanGestureRecognizer();
        SwapTouchGesture.ThresholdUnits = 0.0f; // start right away
        //Add new gesture
        SwapTouchGesture.StateUpdated += PlayerTouchGestureUpdated;
        SwapTouchGesture.AllowSimultaneousExecutionWithAllGestures();

        FingersScript.Instance.AddGesture(SwapTouchGesture);
        
        animatorSnowGun = GetComponent<Animator>();
        
        ShowEvent();
    }

    private void Start()
    {
        _cam = Camera.main;
    }

    public void ShowEvent() // Show Event, spawn antenna 
    {
        // ReSharper disable once Unity.PreferNonAllocApi
        col = Physics.OverlapSphere(transform.position, radius, layerInteractable); // Detect bloc around the object

        foreach (var c in col)
        {
            if (c.TryGetComponent(out Node node))
            {
                if (!node.isActive)
                {
                    col.ToList().Remove(c);
                }
            }
        }
        
        var randomNumber = Random.Range(0, col.Length);

        GameObject go = Instantiate(hatchDetectPlayerNearSnowGun, col[randomNumber].transform.position + vectorSpawnAntenna, Quaternion.identity, col[randomNumber].transform);
        go.GetComponent<DetectionSnowGun>().snowGun = this;
            
        goToAntennaTxt.SetActive(true);
        
        _hatchesList.Add(go);
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

#endif
    
    public void LaunchEvent() 
    {
    }

    private void PlayerTouchGestureUpdated(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Began && canClick)
        {
            PointerEventData p = new PointerEventData(EventSystem.current);
            p.position = new Vector2(gesture.FocusX, gesture.FocusY);

            _raycast.Clear();
            EventSystem.current.RaycastAll(p, _raycast);

            Ray ray = _cam.ScreenPointToRay(p.position);

            if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, playerLayerMask))
            {
                if (hitInfo.collider.name != GameManager.Instance.currentPlayerTurn.name)
                {
                    var posHitInfo = hitInfo.transform.position;
                    
                    var childToMove = transform.GetChild(1);

                    var childToMovePos = childToMove.position;
                    Vector3 targetPosition = new Vector3(posHitInfo.x,childToMovePos.y, posHitInfo.z ) ;
                    childToMove.LookAt(targetPosition) ;
                    
                    var snowEndLaunchSnowPos = childToMove.GetChild(0).GetChild(0).position;
				
                    var xSpawn = (posHitInfo.x + snowEndLaunchSnowPos.x) /2;
                    var zSpawn = (posHitInfo.z + snowEndLaunchSnowPos.z) /2;
                    
                    listPoint.Add(snowEndLaunchSnowPos);
                    listPoint.Add(new Vector3(xSpawn, ySpawn, zSpawn));
                    listPoint.Add(posHitInfo);
                    
                    animatorSnowGun.SetBool("isShooting", true);
                    
                    StartCoroutine(DelayAnimationOut(animatorSnowGun.GetCurrentAnimatorStateInfo(0).length, listPoint, speed, curve));

                    
                    canClick = false;
                }
            }
            else
            {
                gesture.Reset();
            }
        }
    }

    IEnumerator DelayAnimationOut(float delay, List<Vector3> point, float s, AnimationCurve animCurve)
    {
        yield return new WaitForSeconds(delay * 0.8f);

        GameObject snowBullet = Instantiate(snowPrefab, transform.position, Quaternion.identity);
        BezierAlgorithm.Instance.ObjectJumpWithBezierCurve(snowBullet, point, s, animCurve);
    }

    void CanonShootSound()
    {
        AudioManager.Instance.Play("CanonShot");
    }
    
    void ClearGun()
    {
        animatorSnowGun.SetBool("isShooting", false);
        animatorSnowGun.SetBool("canRemoveCannon", true);
    }

    void CanRemoveCannon()
    {
        animatorSnowGun.SetBool("onHatche", false);
    }

    void CanonActivation()
    {
        AudioManager.Instance.Play("CanonActivation");
    }
    
    void CanonOpenOrClose()
    {
        AudioManager.Instance.Play("CanonOpen");
    }
    
    void SetActiveFalseObject()
    {
        shootPlayerTxt.SetActive(false);
        
        foreach (var h in _hatchesList)
        {
            h.SetActive(false);
        }
        _hatchesList.Clear();
        
        gameObject.SetActive(false);
    }
}
