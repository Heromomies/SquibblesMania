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
    }


    private void Start()
    {
        _cam = Camera.main;
        ShowEvent();
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
                    var player = GameManager.Instance.currentPlayerTurn;
                    var tCurrentPlayerTurn = player.transform;
                    
                    var posHitInfo = hitInfo.transform.position;

                    var playerPos = tCurrentPlayerTurn.position;
				
                    var xSpawn = (posHitInfo.x + playerPos.x) /2;
                    var zSpawn = (posHitInfo.z + playerPos.z) /2;
                    
                    listPoint.Add(playerPos);
                    listPoint.Add(new Vector3(xSpawn, ySpawn, zSpawn));
                    listPoint.Add(posHitInfo);
                    
                    GameObject snowBullet = Instantiate(snowPrefab, transform.position, Quaternion.identity);
                    
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
    }
    
    #region CalculateVelocity

    Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float velocity) // Function to make a parabola
    {
        //define the distance x and y first
        Vector3 distance = target - origin;
        Vector3 distanceXZ = distance;
        distanceXZ.Normalize();
        distanceXZ.y = 0;

        //creating a float that represents our distance 
        float sy = distance.y;
        float sxz = distance.magnitude;

        //calculating initial x velocity
        //Vx = x / t

        float vxz = sxz / velocity;

        ////calculating initial y velocity
        //Vy0 = y/t + 1/2 * g * t

        float vy = sy / velocity + 0.6f * Mathf.Abs(Physics.gravity.y + 0.7f) * velocity;
        Vector3 result = distanceXZ * vxz;
        result.y = vy;

        return result;
    }

    #endregion
}
