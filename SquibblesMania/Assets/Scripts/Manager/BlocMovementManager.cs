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
    [Header("BLOC PARAMETERS")]
    public Transform blockParent;
    public GameObject blockCurrentlySelected;
    public Color blockCurrentlyBaseColor;
    private Vector3 _blocParentPos;
    public bool isBlocSelected;
    
   
    private float _timeForMovement= 0.5f;
    private WaitForSeconds _timeBetweenBlocMovement = new WaitForSeconds(0.3f);
    // Start is called before the first frame update
    void Awake()
    {
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
            
        }
        
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
                if (blockCurrentlySelected != null && !GameManager.Instance.currentPlayerTurn.walking)
                {
                    foreach (Transform child in blockParent.transform)
                    {
                       ResetPreviousBlockColor(child.gameObject);
                    }
                }

                blockCurrentlySelected = _hit.collider.gameObject;
                blockParent = blockCurrentlySelected.transform.parent;
                isBlocSelected = true;
                blockCurrentlyBaseColor = blockCurrentlySelected.GetComponent<Renderer>().materials[2].GetColor("_EmissionColor");
                _blocParentPos = blockParent.transform.position;
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

            if (blockCurrentlySelected != null)
            {
                foreach (Transform child in blockParent.transform)
                {
                    PulsingBloc.PulsingEmissiveColorSquareBloc(blockCurrentlyBaseColor, Color.green, child, 0.3f);
                }
            }
           
            
        }
        //If press is ended
        else if (gesture.State == GestureRecognizerState.Ended)
        {
            //End of the drag
            foreach (Transform bloc in blockParent)
            {
                ResetPreviousBlockColor(bloc.gameObject);
            }
            
            isBlocSelected = false;
            blockCurrentlySelected = null;
            blockParent = null;
            _touchPos = Vector3.zero;
        }
    }

    private void BlocMovement(Vector3 touchPos)
    {
        if (touchPos.y > 0.0f && isBlocSelected)
        {
            isBlocSelected = false;
            StartCoroutine(StartBlocMovement(touchPos.y));
        }
        else if (touchPos.y < 0.0f && isBlocSelected)
        {
            isBlocSelected = false;
            StartCoroutine(StartBlocMovement(touchPos.y));
        }
      
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
            
            blockParent.DOMove(new Vector3(blocParentNewPos.x, blocParentNewPos.y + 1f, blocParentNewPos.z), _timeForMovement);
            blocParentNewPos = blockParent.transform.position;
            //Move the player with block
            if (groupBlocDetection.playersOnGroupBlock.Count > 0)
            {
                foreach (Transform playerOnGroupBlock in groupBlocDetection.playersOnGroupBlock)
                {
                    Vector3 playerOnGroupBlockPos = playerOnGroupBlock.position;
                    playerOnGroupBlock.DOMove(
                        new Vector3(playerOnGroupBlockPos.x, playerOnGroupBlockPos.y + 1f, playerOnGroupBlockPos.z),
                        _timeForMovement);
                }

                yield return _timeBetweenBlocMovement;
            }
            switch (blocParentNewPos.y - _blocParentPos.y >= 0)
            {
                case true: GameManager.Instance.currentPlayerTurn.playerActionPoint--;
                    UiManager.Instance.SetUpCurrentActionPointOfCurrentPlayer(GameManager.Instance.currentPlayerTurn.playerActionPoint); break;
                case false: GameManager.Instance.currentPlayerTurn.playerActionPoint++;
                    UiManager.Instance.SetUpCurrentActionPointOfCurrentPlayer(GameManager.Instance.currentPlayerTurn.playerActionPoint); break;
            }
            
        }
        else if (yPos < 0.0f && blocParentNewPos.y > GameManager.Instance.minHeightBlocMovement)
        {
            if (blocParentNewPos.y - GameManager.Instance.minHeightBlocMovement == 0)
            {
                //TODO Feedback can't move bloc
                yield break;
            }
            blockParent.DOMove(new Vector3(blocParentNewPos.x, blocParentNewPos.y - 1f, blocParentNewPos.z), _timeForMovement);
            blocParentNewPos = blockParent.transform.position;
            //Move the player with block
            if (groupBlocDetection.playersOnGroupBlock.Count > 0)
            {
                foreach (Transform playerOnGroupBlock in groupBlocDetection.playersOnGroupBlock)
                {
                    Vector3 playerOnGroupBlockPos = playerOnGroupBlock.position;
                    playerOnGroupBlock.DOMove(new Vector3(playerOnGroupBlockPos.x, playerOnGroupBlockPos.y - 1f, playerOnGroupBlockPos.z), _timeForMovement);
                }

                yield return _timeBetweenBlocMovement;
            }
            switch (blocParentNewPos.y - _blocParentPos.y <= 0)
             {
                 case true: GameManager.Instance.currentPlayerTurn.playerActionPoint--;
                     UiManager.Instance.SetUpCurrentActionPointOfCurrentPlayer(GameManager.Instance.currentPlayerTurn.playerActionPoint); break;
                 case false: GameManager.Instance.currentPlayerTurn.playerActionPoint++;
                     UiManager.Instance.SetUpCurrentActionPointOfCurrentPlayer(GameManager.Instance.currentPlayerTurn.playerActionPoint); break;
             }
        }

      
        
        ResetPreviewPlatform();
        //isMovingBlock = false;
        TouchManager.Instance.blockParent = null;
        if (GameManager.Instance.currentPlayerTurn.playerActionPoint <= 0)
        {
            UiManager.Instance.buttonNextTurn.SetActive(true);
            foreach (var block in GameManager.Instance.currentPlayerTurn.nextBlockPath)
            {
                ResetPreviousBlockColor(block.gameObject);
            }
        }
        yield return _timeBetweenBlocMovement;
        _touchPos = Vector3.zero;
        isBlocSelected = true;
        /*var player = GameManager.Instance.currentPlayerTurn;
        player.PlayerActionPointCardState.ResetColorPreviewPath(player.PlayerActionPointCardState.previewPath, player.PlayerActionPointCardState.blocBaseEmissiveColor);
        player.PlayerActionPointCardState.PreviewPath(player.playerActionPoint, player);*/
    }
    private void ResetPreviousBlockColor(GameObject bloc)
    {
        Material blockCurrentlySelectedMat = bloc.GetComponent<Renderer>().materials[2];
        blockCurrentlySelectedMat.SetColor("_EmissionColor", blockCurrentlyBaseColor);
    }
    private void ResetPreviewPlatform()
    {
        if (TouchManager.Instance.blockParent != null)
        {
            switch (GameManager.Instance.actualCamPreset.presetNumber)
            {
                case 1: TouchManager.Instance.uiInteraction[0].uiInteractionParentObject.SetActive(false); break;
                case 2: TouchManager.Instance.uiInteraction[0].uiInteractionParentObject.SetActive(false); break;
                case 3: TouchManager.Instance.uiInteraction[1].uiInteractionParentObject.SetActive(false); break;
                case 4: TouchManager.Instance.uiInteraction[1].uiInteractionParentObject.SetActive(false); break;
            }
           
        }
        var player = GameManager.Instance.currentPlayerTurn;
        player.PlayerActionPointCardState.ResetColorPreviewPath(player.PlayerActionPointCardState.previewPath);
    }
    
  
}
