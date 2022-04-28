using System.Collections;
using System.Collections.Generic;
using DigitalRubyShared;
using I2.Loc;
using UnityEngine;
using UnityEngine.EventSystems;

public class SnowGun : MonoBehaviour, IManageEvent
{
    public GameObject snowPrefab;

    [Range(0.0f, 0.1f)] public float speed;
    [Range(0.0f, 10.0f)] public float speedRotationSnowGun;
    [Range(0.0f, 10.0f)] public float ySpawn;
    public GameObject hatchDetectPlayerNearSnowGun;
    public LayerMask playerLayerMask;
    
    public AnimationCurve curve;

    public GameObject snowGun;

    [HideInInspector] public Animator animatorSnowGun;
    [HideInInspector] public List<Vector3> listPoint = new List<Vector3>();
    private List<GameObject> _hatchesList = new List<GameObject>();
    private readonly List<RaycastResult> _raycast = new List<RaycastResult>();
    public PanGestureRecognizer SwapTouchGesture { get; private set; }
    private Camera _cam;
    [HideInInspector] public bool canClick;
    
    private void OnEnable()
    {
        SwapTouchGesture = new PanGestureRecognizer();
        SwapTouchGesture.ThresholdUnits = 0.0f; // start right away
        //Add new gesture
        SwapTouchGesture.StateUpdated += PlayerTouchGestureUpdated;
        SwapTouchGesture.AllowSimultaneousExecutionWithAllGestures();

        FingersScript.Instance.AddGesture(SwapTouchGesture);
        
        snowGun = Instantiate(snowGun, transform.position + new Vector3(0,0.02f,0), Quaternion.identity);
        
        animatorSnowGun = snowGun.GetComponent<Animator>();
        
        ShowEvent();
    }

    private void Start()
    {
        _cam = Camera.main;
    }

    public void ShowEvent()
    {
        var hatchPossiblePath = GetComponentInParent<Node>().possiblePath;
        
        for (int i = 0; i < hatchPossiblePath.Count; i++)
        {
            GameObject go = Instantiate(hatchDetectPlayerNearSnowGun, hatchPossiblePath[i].nextPath.transform.position + new Vector3(0,1.05f, 0), Quaternion.identity, hatchPossiblePath[i].nextPath.transform);
            go.GetComponent<DetectionSnowGun>().snowGun = this;
            
            _hatchesList.Add(go);
        }
    }

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
                if (hitInfo.collider.name != GameManager.Instance.name)
                {
                    var posHitInfo = hitInfo.transform.position;

                    snowGun.gameObject.SetActive(true);

                    var childToMove = snowGun.transform.GetChild(1);

                    var childToMovePos = childToMove.position;
                    Vector3 targetPosition = new Vector3(posHitInfo.x,childToMovePos.y, posHitInfo.z ) ;
                    childToMove.LookAt(targetPosition) ;
                    
                    // Move the snow Gun smoothly but only on one frame 
                    /*Vector3 lookDirection = posHitInfo - childToMovePos;
                    lookDirection.Normalize();
                    childToMove.rotation = Quaternion.Slerp(childToMove.rotation, Quaternion.LookRotation(lookDirection), speedRotationSnowGun * Time.deltaTime); */
                    
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

        GameObject snowBullet = Instantiate(snowPrefab,  snowGun.transform.position, Quaternion.identity);
        BezierAlgorithm.Instance.ObjectJumpWithBezierCurve(snowBullet, point, s, animCurve);

        ClearGun();
    }
    
    void ClearGun()
    {
        animatorSnowGun.SetBool("isShooting", false);
        animatorSnowGun.SetBool("canRemoveCannon", true);

        StartCoroutine(DelayAnimationCanRemoveCannon(animatorSnowGun.GetCurrentAnimatorStateInfo(0).length));
    }

    IEnumerator DelayAnimationCanRemoveCannon(float delay)
    {
        yield return new WaitForSeconds(delay * 2);

        animatorSnowGun.SetBool("onHatche", false);
        StartCoroutine(DelaySetActiveFalseObject(animatorSnowGun.GetCurrentAnimatorStateInfo(0).length));
    }

    IEnumerator DelaySetActiveFalseObject(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        foreach (var h in _hatchesList)
        {
            h.SetActive(false);
        }
        _hatchesList.Clear();
        
        snowGun.SetActive(false);
        gameObject.SetActive(false);
    }
}
