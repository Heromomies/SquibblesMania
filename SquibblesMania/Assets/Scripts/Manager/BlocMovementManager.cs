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
    private readonly List<RaycastResult> _raycast = new List<RaycastResult>();
    private LongPressGestureRecognizer LongPressBlocMovementGesture { get; set; }

    [Header("TOUCH PARAMETERS")] private Vector3 _touchPos;
    public LayerMask blocMoveLayersMask;
    private Camera _cam;
    private RaycastHit _hit;
    public bool hasStopMovingBloc;

    [Header("BLOC PARAMETERS")] 
    [SerializeField] private Transform blockParentCurrentlySelected;
    [SerializeField] private GameObject blockCurrentlySelected;
    [SerializeField] private float movementBlocAmount = 1f;
    private Vector3 _blocParentCurrentlySelectedPos;
    [SerializeField] private bool isBlocSelected;
    private Vector3 _lastDirectionBloc;
    private float _timeInSecondsForBlocMove = 0.4f;
    private readonly WaitForSeconds _timeInSecondsBetweenBlocMovement = new WaitForSeconds(0.4f);
    
    [Header("POP UP TEXT PARAMETERS")] 
    [SerializeField] private int totalCurrentActionPoint;
    [SerializeField] private GameObject textActionPointPopUp;
    [SerializeField] private Vector3 offsetText;
    
   
    private readonly List<Transform> _nextBlocUpMeshPos = new List<Transform>();
    private readonly List<Transform> _nextBlocDownMeshPos = new List<Transform>();
    private static BlocMovementManager _blocMovementManager;

    public static BlocMovementManager Instance => _blocMovementManager;

  

    [SerializeField] private PlayerMovementManager playerMovementManager;

    // Start is called before the first frame update
   private void Awake()
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
        LongPressBlocMovementGesture.MinimumDurationSeconds = 0.1f;
        LongPressBlocMovementGesture.AllowSimultaneousExecution(playerMovementManager.LongPressBlocMovementGesture);
        LongPressBlocMovementGesture.AllowSimultaneousExecution(playerMovementManager.swipe);
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
                _raycast.Clear();
                EventSystem.current.RaycastAll(p, _raycast);
                // Cast a ray from the camera
                Ray ray = _cam.ScreenPointToRay(p.position);

                if (Physics.Raycast(ray, out _hit, Mathf.Infinity, blocMoveLayersMask))
                {
                    
                    Node.ColorBloc colorBloc = _hit.collider.gameObject.GetComponent<Node>().colorBloc;
                    
                    if (colorBloc != Node.ColorBloc.None && !currentPlayerTurn.walking && currentPlayerTurn.nextBlockPath.Contains(_hit.transform) && !_hit.collider.CompareTag("Player"))
                    {
                     
                        //If current player have more than 0 action point then he can move bloc
                        if (currentPlayerTurn.playerActionPoint > 0)
                        {
                           // playerMovementManager.enabled = false;
                            if (!hasStopMovingBloc)
                            {
                                StartMovingBloc(currentPlayerTurn);
                            }
                            
                        }
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
                    isBlocSelected = false;
                  
                }
            }
            //If press is ended
            else if (gesture.State == GestureRecognizerState.Ended)
            {
                hasStopMovingBloc = true;
                //playerMovementManager.enabled = true;
                if (hasStopMovingBloc)
                {
                    //End of the drag
                    EndMovingBloc(currentPlayerTurn);
                }
            }
           
        }
    }

    #region Moving Bloc State

    private void EndMovingBloc(PlayerStateManager currentPlayerTurn)
    {
        ResetPreviewPathObjects();
        ResetBlocPreviewMesh();
        isBlocSelected = false;
        _touchPos = Vector3.zero;
        blockCurrentlySelected = null;
        blockParentCurrentlySelected = null;
        if (textActionPointPopUp)
        {
            textActionPointPopUp.SetActive(false);
        }

     
        _lastDirectionBloc = Vector3.zero;
      
        if (currentPlayerTurn.playerActionPoint <= 0)
        {
            UiManager.Instance.buttonNextTurn.SetActive(true);
        }

        else
        {
            UiManager.Instance.buttonNextTurn.SetActive(false);
        }
    }

    private void StartMovingBloc(PlayerStateManager currentPlayerTurn)
    {
        blockCurrentlySelected = _hit.collider.gameObject;
        isBlocSelected = true;
        var currentPlayer = currentPlayerTurn.transform;
        blockParentCurrentlySelected = blockCurrentlySelected.transform.parent;
        _blocParentCurrentlySelectedPos = blockParentCurrentlySelected.transform.position;
        SpawnTextActionPointPopUp(currentPlayer);
        SetUpBlocPreviewMesh(blockParentCurrentlySelected);
        hasStopMovingBloc = true;
    }

    #endregion

 

    private void SpawnTextActionPointPopUp(Transform currentPlayer)
    {
        totalCurrentActionPoint = GameManager.Instance.currentPlayerTurn.playerActionPoint;
        textActionPointPopUp = PoolManager.Instance.SpawnObjectFromPool("PopUpTextActionPoint", currentPlayer.position + offsetText, Quaternion.identity, currentPlayer);
        textActionPointPopUp.SetActive(true);
        textActionPointPopUp.GetComponent<PopUpTextActionPoint>().SetUpText(GameManager.Instance.currentPlayerTurn.playerActionPoint);
    }

    #region BlocPreviewMesh

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

    private void SetUpBlocPreviewMesh(Transform blocParent)
    {
        ResetBlocPreviewMesh();
        foreach (Transform bloc in blocParent.transform)
        {
            var blocPosition = bloc.position;


            GameObject blocPreviewUpMesh = PoolManager.Instance.SpawnObjectFromPool("BlocPreview",
                blocPosition + Vector3.up, Quaternion.identity, null);
            GameObject blocPreviewDownMesh = PoolManager.Instance.SpawnObjectFromPool("BlocPreview",
                blocPosition + Vector3.down * 2, Quaternion.identity, null);

            RoundYBlocPreviewMeshPos(blocPreviewUpMesh);
            RoundYBlocPreviewMeshPos(blocPreviewDownMesh);


            if (Mathf.RoundToInt(blocPreviewUpMesh.transform.position.y) > GameManager.Instance.maxHeightBlocMovement + 1)
            {
                blocPreviewUpMesh.SetActive(false);
            }

            if (Mathf.RoundToInt(blocPreviewDownMesh.transform.position.y) < GameManager.Instance.minHeightBlocMovement)
            {
                blocPreviewDownMesh.SetActive(false);
            }

            _nextBlocUpMeshPos.Add(blocPreviewUpMesh.transform);
            _nextBlocDownMeshPos.Add(blocPreviewDownMesh.transform);
        }
    }

    private void RoundYBlocPreviewMeshPos(GameObject bloc)
    {
        var previewPos = bloc.transform.position;
        previewPos.y = Mathf.Round(previewPos.y);
        bloc.transform.position = previewPos;
    }


    #endregion
  
    private void BlocMovement(Vector3 touchPos)
    {
        var direction = touchPos.normalized;
        if (isBlocSelected)
        {
            StartCoroutine(StartBlocMovementCoroutine(touchPos.y, direction));
        }
     
    }

    IEnumerator StartBlocMovementCoroutine(float yPos, Vector3 direction)
    {
        GroupBlockDetection groupBlocDetection = blockParentCurrentlySelected.GetComponent<GroupBlockDetection>();
        var blocParentNewPos = blockParentCurrentlySelected.transform.position;

        if (_lastDirectionBloc == Vector3.zero)
        {
            _lastDirectionBloc = direction;
        }
        
        
        if (ActualCamPreset.CamPresetTeam() == ActualCamPreset.Team.TeamOne)
        {
            yPos = +yPos;
        }

        else if (ActualCamPreset.CamPresetTeam() == ActualCamPreset.Team.TeamTwo)
        {
            yPos = -yPos;
            direction = -direction;
        }


        if (yPos > 0.0f)
        {
            if (blocParentNewPos.y - GameManager.Instance.maxHeightBlocMovement == 0 || totalCurrentActionPoint == 0 && _lastDirectionBloc.y > 0.0f)
            {
                //TODO Feedback can't move bloc
                Debug.Log("Im blocked up");
            }
            else
            {
                StartMoveBloc(_nextBlocUpMeshPos, groupBlocDetection, blocParentNewPos, movementBlocAmount);
                yield return _timeInSecondsBetweenBlocMovement;
                EndMoveBloc(blocParentNewPos.y - _blocParentCurrentlySelectedPos.y >= 0, direction);
            }
        }
        else if (yPos < 0.0f)
        {
            if (blocParentNewPos.y - GameManager.Instance.minHeightBlocMovement == 0 || totalCurrentActionPoint == 0 && _lastDirectionBloc.y < 0.0f)
            {
                //TODO Feedback can't move bloc
                Debug.Log("Im blocked down");
            }
            else
            {
                StartMoveBloc(_nextBlocDownMeshPos, groupBlocDetection, blocParentNewPos, -movementBlocAmount);
                yield return _timeInSecondsBetweenBlocMovement;
                EndMoveBloc(blocParentNewPos.y - _blocParentCurrentlySelectedPos.y <= 0, direction);
            }
        }
        
        yield return _timeInSecondsBetweenBlocMovement;
        ResetPreviewPathObjects();
        _touchPos = Vector3.zero;
        isBlocSelected = true;
        hasStopMovingBloc = false;
    }

    #region MovingBloc

    private void StartMoveBloc(List<Transform> previewBlocsMesh, GroupBlockDetection groupBlocDetection, Vector3 blocParentNewPos, float moveBlocAmount)
    {
        ResetBlocPreviewMesh();
        foreach (var blocMesh in previewBlocsMesh)
        {
            blocMesh.gameObject.SetActive(false);
        }
        
        MoveBlocAndPlayer(moveBlocAmount, blockParentCurrentlySelected, blocParentNewPos, groupBlocDetection);
        var player = GameManager.Instance.currentPlayerTurn;
        player.PlayerActionPointCardState.SetFalsePathObjects();
    }

    private void EndMoveBloc(bool isPlayerUseActionPoint, Vector3 direction)
    {
        switch (isPlayerUseActionPoint)
        {
            case true:
                UpdateActionPointTextPopUp(totalCurrentActionPoint--);
                break;
            case false:
                UpdateActionPointTextPopUp(totalCurrentActionPoint++);
                break;
        }

       
        if (!hasStopMovingBloc)
        {
            SetUpBlocPreviewMesh(blockParentCurrentlySelected);
        }
        var player = GameManager.Instance.currentPlayerTurn;
        player.PlayerActionPointCardState.SetFalsePathObjects();
        _lastDirectionBloc = direction;
    }

    

    private void MoveBlocAndPlayer(float value, Transform blocParent, Vector3 blocParentNewPos, GroupBlockDetection groupBlocDetection)
    {
        blocParent.DOMove(new Vector3(blocParentNewPos.x, blocParentNewPos.y + value, blocParentNewPos.z),
            _timeInSecondsForBlocMove);
        //Move the player with block
        if (groupBlocDetection.playersOnGroupBlock.Count > 0)
        {
            foreach (Transform playerOnGroupBlock in groupBlocDetection.playersOnGroupBlock)
            {
                Vector3 playerOnGroupBlockPos = playerOnGroupBlock.position;
                playerOnGroupBlock.DOMove(
                    new Vector3(playerOnGroupBlockPos.x, playerOnGroupBlockPos.y + value, playerOnGroupBlockPos.z),
                    _timeInSecondsForBlocMove);
            }
        }
    }

    #endregion
  

    private void UpdateActionPointTextPopUp(int actionPoint)
    {
        //Update text action point at player top pos
        actionPoint = totalCurrentActionPoint > 0 ? totalCurrentActionPoint : -totalCurrentActionPoint;
        textActionPointPopUp.GetComponent<PopUpTextActionPoint>().SetUpText(actionPoint);
        GameManager.Instance.currentPlayerTurn.playerActionPoint = totalCurrentActionPoint;
        UiManager.Instance.SetUpCurrentActionPointOfCurrentPlayer(GameManager.Instance.currentPlayerTurn.playerActionPoint);
    }
    private void ResetPreviewPathObjects()
    {
        var player = GameManager.Instance.currentPlayerTurn;
        player.PlayerActionPointCardState.SetFalsePathObjects();
        player.PlayerActionPointCardState.PreviewPath(player.playerActionPoint, player);
    }
}

