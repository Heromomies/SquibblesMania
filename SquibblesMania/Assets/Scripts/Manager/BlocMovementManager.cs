using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DG.Tweening;
using DigitalRubyShared;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlocMovementManager : MonoBehaviour
{
    
    private readonly List<RaycastResult> raycast = new List<RaycastResult>();
    public LongPressGestureRecognizer LongPressBlocMovementGesture { get; private set;}

    [Header("TOUCH PARAMETERS")] private Vector3 _touchPos;
    public LayerMask touchLayersMask;
    private Camera _cam;
    private RaycastHit _hit;
    public bool hasStopMovingBloc;
    [Header("BLOC PARAMETERS")]
    public Transform blockParent;
    public GameObject blockCurrentlySelected;
    [SerializeField]
    private float movementBlocAmount = 1f;
    private Vector3 _blocParentPos;
    public bool isBlocSelected;
    [SerializeField]
    private int totalCurrentActionPoint;
    private GameObject textActionPointPopUp;
    [SerializeField]
    private Vector3 offsetText;
    private float _timeForMovement= 0.5f;
    private WaitForSeconds _timeBetweenBlocMovement = new WaitForSeconds(0.5f);

    private List<Transform> _nextBlocUpMeshPos = new List<Transform>();
    private List<Transform> _nextBlocDownMeshPos = new List<Transform>();
    private static BlocMovementManager _blocMovementManager;

    public static BlocMovementManager Instance => _blocMovementManager;
    // Start is called before the first frame update
    void Awake()
    {
        _blocMovementManager = this;
        _cam = Camera.main;
    }

    private void OnEnable()
    {
        //Set up the new gesture 
        LongPressBlocMovementGesture = new LongPressGestureRecognizer();
        LongPressBlocMovementGesture.StateUpdated += LongPressBlocMovementGestureOnStateUpdated;
        LongPressBlocMovementGesture.ThresholdUnits = 0.0f;
        LongPressBlocMovementGesture.MinimumDurationSeconds = 0.3f;
        FingersScript.Instance.AddGesture(LongPressBlocMovementGesture);
    }

    private void OnDisable()
    {
        if (FingersScript.HasInstance)
        {
            FingersScript.Instance.RemoveGesture(LongPressBlocMovementGesture);
        }

    }

    //Update method of the long press gesture
    private void LongPressBlocMovementGestureOnStateUpdated(GestureRecognizer gesture)
    {
        if (GameManager.Instance.currentPlayerTurn.isPlayerInActionCardState)
        { 
            PlayerStateManager currentPlayerTurn = GameManager.Instance.currentPlayerTurn; 
            //If press is began
        if (gesture.State == GestureRecognizerState.Began)
        {
            PointerEventData p = new PointerEventData(EventSystem.current);
            p.position = new Vector2(gesture.FocusX, gesture.FocusY);
            raycast.Clear();
            EventSystem.current.RaycastAll(p, raycast);
            // Cast a ray from the camera
            Ray ray = _cam.ScreenPointToRay(p.position);

            if (Physics.Raycast(ray, out _hit, Mathf.Infinity, touchLayersMask))
            {
                if (_hit.collider.gameObject.GetComponent<Node>() && !currentPlayerTurn.walking && currentPlayerTurn.nextBlockPath.Contains(_hit.transform))
                {
                    blockCurrentlySelected = _hit.collider.gameObject;
                    isBlocSelected = true;
                    Transform currentPlayer = GameManager.Instance.currentPlayerTurn.transform;
                    if (!hasStopMovingBloc)
                    {
                        blockParent = blockCurrentlySelected.transform.parent;
                        _blocParentPos = blockParent.transform.position;
                        totalCurrentActionPoint = GameManager.Instance.currentPlayerTurn.playerActionPoint;
                        textActionPointPopUp = PoolManager.Instance.SpawnObjectFromPool("PopUpTextActionPoint", currentPlayer.position + offsetText, Quaternion.identity, currentPlayer);
                    }
                    textActionPointPopUp.SetActive(true);
                    textActionPointPopUp.GetComponent<PopUpTextActionPoint>().SetUpText(GameManager.Instance.currentPlayerTurn.playerActionPoint);
                    SetUpPreviewBloc(blockParent);
                    hasStopMovingBloc = true;
                }
                
            }
        }
        //If press is currently executing
        else if (gesture.State == GestureRecognizerState.Executing)
        {
            if (isBlocSelected)
            {
                _touchPos = new Vector3(gesture.DeltaX, gesture.DeltaY, 0);
                BlocMovement(_touchPos);
            }

        }
        //If press is ended
        else if (gesture.State == GestureRecognizerState.Ended)
        {
            //End of the drag
            ResetBlocPreviewMesh();
            isBlocSelected = false;
            _touchPos = Vector3.zero;
            GameManager.Instance.currentPlayerTurn.playerActionPoint = totalCurrentActionPoint;
            UiManager.Instance.SetUpCurrentActionPointOfCurrentPlayer(GameManager.Instance.currentPlayerTurn.playerActionPoint);
            textActionPointPopUp.SetActive(false);
        }
            
        }
        
       
    }

   private void ResetBlocPreviewMesh()
    {
        if (_nextBlocDownMeshPos.Count > 0 || _nextBlocUpMeshPos.Count > 0)
        {
            foreach (var nextBlocDownMesh in _nextBlocDownMeshPos)
            {
                nextBlocDownMesh.gameObject.SetActive(false);
            }
            foreach (var nextBlocUpMesh in _nextBlocUpMeshPos)
            {
                nextBlocUpMesh.gameObject.SetActive(false);
            }
               
        }
        _nextBlocUpMeshPos.Clear();
        _nextBlocDownMeshPos.Clear();
    }

    private void SetUpPreviewBloc(Transform blocParent)
    {
        ResetBlocPreviewMesh();
        foreach (Transform bloc in blocParent.transform)
        {
            var blocPosition = bloc.position;
            
            
            GameObject blocPreviewUp = PoolManager.Instance.SpawnObjectFromPool("BlocPreview", blocPosition + Vector3.up, Quaternion.identity, null);
            GameObject blocPreviewDown = PoolManager.Instance.SpawnObjectFromPool("BlocPreview", blocPosition + Vector3.down * 2, Quaternion.identity, null);

            RoundYBlocPreviewPos(blocPreviewUp);
            RoundYBlocPreviewPos(blocPreviewDown);
            
            
            if (Mathf.RoundToInt(blocPreviewUp.transform.position.y) > GameManager.Instance.maxHeightBlocMovement+1)
            {
                blocPreviewUp.SetActive(false);
            }
                
            if (Mathf.RoundToInt(blocPreviewDown.transform.position.y)  < GameManager.Instance.minHeightBlocMovement)
            {
                blocPreviewDown.SetActive(false);
            }
            
            _nextBlocUpMeshPos.Add(blocPreviewUp.transform);
            _nextBlocDownMeshPos.Add(blocPreviewDown.transform);
        }
    }

    void RoundYBlocPreviewPos(GameObject bloc)
    {
        var previewPos = bloc.transform.position;
        previewPos.y = Mathf.Round(previewPos.y);
        bloc.transform.position = previewPos;
    }

    private void BlocMovement(Vector3 touchPos)
    {
        isBlocSelected = false;
        StartCoroutine(StartBlocMovement(touchPos.y));
    }

    IEnumerator StartBlocMovement(float yPos)
    {
        GroupBlockDetection groupBlocDetection = blockParent.GetComponent<GroupBlockDetection>();
        Vector3 blocParentNewPos = blockParent.transform.position;
        
        if (yPos > 0.0f && blocParentNewPos.y < GameManager.Instance.maxHeightBlocMovement)
        {
            if (blocParentNewPos.y - GameManager.Instance.maxHeightBlocMovement == 0)
            {
                //TODO Feedback can't move bloc
                yield break;
            }

            foreach (var nextBlocDownMesh in _nextBlocDownMeshPos)
            {
                nextBlocDownMesh.gameObject.SetActive(false);
            }
            
            MovementBlocAndPlayer(movementBlocAmount, blockParent, blocParentNewPos, groupBlocDetection);
            yield return _timeBetweenBlocMovement;
            switch (blocParentNewPos.y - _blocParentPos.y >= 0)
            { 
                case true: UpdateActionPointText(totalCurrentActionPoint--); break;
                case false: UpdateActionPointText(totalCurrentActionPoint++); break;
                
            } 
            SetUpPreviewBloc(blockParent);

        }
        else if (yPos < 0.0f && blocParentNewPos.y > GameManager.Instance.minHeightBlocMovement)
        {
            if (blocParentNewPos.y - GameManager.Instance.minHeightBlocMovement == 0)
            {
                //TODO Feedback can't move bloc
                yield break;
            }

            foreach (var nextBlocUpMesh in _nextBlocUpMeshPos)
            {
                nextBlocUpMesh.gameObject.SetActive(false);
            }
            MovementBlocAndPlayer(-movementBlocAmount, blockParent, blocParentNewPos, groupBlocDetection);
            yield return _timeBetweenBlocMovement;
            switch (blocParentNewPos.y - _blocParentPos.y <= 0)
             {
                 case true: UpdateActionPointText(totalCurrentActionPoint--); break;
                 
                 case false: UpdateActionPointText(totalCurrentActionPoint++); break;
             }
            SetUpPreviewBloc(blockParent);
        }
        
        ResetPreviewPlatform();
        
        if (GameManager.Instance.currentPlayerTurn.playerActionPoint <= 0)
        {
            UiManager.Instance.buttonNextTurn.SetActive(true);
        }
        yield return _timeBetweenBlocMovement;
        
        _touchPos = Vector3.zero;
        isBlocSelected = true;
      
    }

    private void UpdateActionPointText (int actionPoint)
    {
        //Update text action point at player top pos
        actionPoint = totalCurrentActionPoint > 0 ? totalCurrentActionPoint : -totalCurrentActionPoint;
        textActionPointPopUp.GetComponent<PopUpTextActionPoint>().SetUpText(actionPoint);
        
    }

   private void MovementBlocAndPlayer(float value, Transform blocParent, Vector3 blocParentNewPos, GroupBlockDetection groupBlocDetection)
    {
        blocParent.DOMove(new Vector3(blocParentNewPos.x, blocParentNewPos.y + value, blocParentNewPos.z), _timeForMovement);
        //Move the player with block
        if (groupBlocDetection.playersOnGroupBlock.Count > 0)
        {
            foreach (Transform playerOnGroupBlock in groupBlocDetection.playersOnGroupBlock)
            {
                Vector3 playerOnGroupBlockPos = playerOnGroupBlock.position;
                playerOnGroupBlock.DOMove(new Vector3(playerOnGroupBlockPos.x, playerOnGroupBlockPos.y + value, playerOnGroupBlockPos.z), _timeForMovement);
            }
        }
    }
   
   
    private void ResetPreviewPlatform()
    {
        var player = GameManager.Instance.currentPlayerTurn;
        player.PlayerActionPointCardState.SetFalsePathObjects();
        player.PlayerActionPointCardState.PreviewPath(player.playerActionPoint, player);
    }
    
  
}
