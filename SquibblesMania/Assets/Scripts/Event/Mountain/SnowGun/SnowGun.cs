using System.Collections.Generic;
using DigitalRubyShared;
using UnityEngine;
using UnityEngine.EventSystems;

public class SnowGun : MonoBehaviour, IManageEvent
{
    public GameObject snowPrefab;

    [Range(0.0f, 0.1f)] public float speed;
    [Range(0.0f, 10.0f)] public float ySpawn;
    public GameObject hatchDetectPlayerNearSnowGun;
    public LayerMask playerLayerMask;
    
    public AnimationCurve curve;

    public Transform snowGun;
    public Transform snowGunEndLaunchSnow;
    
    [HideInInspector] public List<Vector3> listPoint = new List<Vector3>();
    
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
        }
    }

    public void LaunchEvent() // Check the distance between each players launch a bullet to the nearest player
    {
        ClearGun();
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

                    var objectPosition = transform.position;
                    
                    snowGun.gameObject.SetActive(true);
                    
                    snowGun.position = objectPosition + new Vector3(0, 1, 0);
                        
                    Vector3 targetPosition = new Vector3(posHitInfo.x, snowGun.position.y, posHitInfo.z ) ;
                    snowGun.LookAt(targetPosition) ;
                    
                    var snowEndLaunchSnowPos = snowGunEndLaunchSnow.position;
				
                    var xSpawn = (posHitInfo.x + snowEndLaunchSnowPos.x) /2;
                    var zSpawn = (posHitInfo.z + snowEndLaunchSnowPos.z) /2;
                    
                    listPoint.Add(snowEndLaunchSnowPos);
                    listPoint.Add(new Vector3(xSpawn, ySpawn, zSpawn));
                    listPoint.Add(posHitInfo);
                    
                    GameObject snowBullet = Instantiate(snowPrefab,  snowGun.transform.position, Quaternion.identity);
                    
                    BezierAlgorithm.Instance.ObjectJumpWithBezierCurve(snowBullet, listPoint, speed, curve);
                    
                    LaunchEvent();
                }
            }
            else
            {
                gesture.Reset();
            }
        }
    }
    
    void ClearGun()
    {
        canClick = false;
        //snowGun.gameObject.SetActive(false);
    }
}
