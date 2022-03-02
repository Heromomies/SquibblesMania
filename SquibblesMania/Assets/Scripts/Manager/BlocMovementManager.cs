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
    
    [Header("TOUCH PARAMETERS")]
    
    public LayerMask touchLayersMask;
    private Camera _cam;
    public RaycastHit Hit;
    [Header("BLOC PARAMETERS")]
    public Transform blockParent;
    public GameObject blockCurrentlySelected;
    public Color blockCurrentlyBaseColor;

    public bool isBlocSelected;
    private float _timeForBlocParentMove = 0.5f;
    private float _timeForPlayersOnBlocMove = 0.2f;
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
        LongPressBlocMovementGesture.MinimumDurationSeconds = 0.5f;
        LongPressBlocMovementGesture.AllowSimultaneousExecutionWithAllGestures();
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
        
        if (gesture.State == GestureRecognizerState.Began)
        {
            PointerEventData p = new PointerEventData(EventSystem.current);
            p.position = new Vector2(gesture.FocusX, gesture.FocusY);

            raycast.Clear();
            EventSystem.current.RaycastAll(p, raycast);
            // Cast a ray from the camera
            Ray ray = _cam.ScreenPointToRay(p.position);

            if (Physics.Raycast(ray, out Hit, Mathf.Infinity, touchLayersMask))
            {
                if (blockCurrentlySelected != null && !GameManager.Instance.currentPlayerTurn.walking)
                {
                    ResetPreviousBlockColor(blockCurrentlySelected);
                }

                blockCurrentlySelected = Hit.collider.gameObject;
                isBlocSelected = true;
                blockCurrentlyBaseColor = blockCurrentlySelected.GetComponent<Renderer>().materials[2].GetColor("_EmissionColor");

            }
        }
        else if (gesture.State == GestureRecognizerState.Executing)
        {
            if (isBlocSelected)
            {
                PulsingBloc.PulsingEmissiveColorSquareBloc(blockCurrentlyBaseColor, Color.red, blockCurrentlySelected.transform, 0.3f);
                blockParent = blockCurrentlySelected.transform.parent;
                Vector3 touchPos = new Vector3(gesture.DistanceX, gesture.DistanceY, 0);
                Debug.Log(touchPos.y);
                BlocMovement(touchPos);
            }
            
        }
        else if (gesture.State == GestureRecognizerState.Ended)
        {
            //End of the drag
            ResetPreviousBlockColor(blockCurrentlySelected);
            isBlocSelected = false;
            blockCurrentlySelected = null;
            blockParent = null;
          
      
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
        Vector3 blocParentPos = blockParent.transform.position;
        
        if (yPos > 0.0f && blocParentPos.y < GameManager.Instance.maxHeightBlocMovement)
        {
            blockParent.DOMove(new Vector3(blocParentPos.x, blocParentPos.y + 1f, blocParentPos.z), _timeForBlocParentMove);

            //Move the player with block
            if (groupBlocDetection.playersOnGroupBlock.Count > 0)
            {
                foreach (Transform playerOnGroupBlock in groupBlocDetection.playersOnGroupBlock)
                {
                    Vector3 playerOnGroupBlockPos = playerOnGroupBlock.position;
                    playerOnGroupBlock.DOMove(
                        new Vector3(playerOnGroupBlockPos.x, playerOnGroupBlockPos.y + 1f, playerOnGroupBlockPos.z),
                        _timeForPlayersOnBlocMove);
                }

                yield return _timeBetweenBlocMovement;
            }
            GameManager.Instance.currentPlayerTurn.playerActionPoint--;
            UiManager.Instance.SetUpCurrentActionPointOfCurrentPlayer(GameManager.Instance.currentPlayerTurn.playerActionPoint);
         
        }
        else if (yPos < 0.0f && blocParentPos.y > GameManager.Instance.minHeightBlocMovement)
        {
            blockParent.DOMove(new Vector3(blocParentPos.x, blocParentPos.y - 1f, blocParentPos.z), _timeForBlocParentMove);
            //Move the player with block
            if (groupBlocDetection.playersOnGroupBlock.Count > 0)
            {
                foreach (Transform playerOnGroupBlock in groupBlocDetection.playersOnGroupBlock)
                {
                    Vector3 playerOnGroupBlockPos = playerOnGroupBlock.position;
                    playerOnGroupBlock.DOMove(new Vector3(playerOnGroupBlockPos.x, playerOnGroupBlockPos.y - 1f, playerOnGroupBlockPos.z), _timeForPlayersOnBlocMove);
                }

                yield return _timeBetweenBlocMovement;
            }
            GameManager.Instance.currentPlayerTurn.playerActionPoint--;
            UiManager.Instance.SetUpCurrentActionPointOfCurrentPlayer(GameManager.Instance.currentPlayerTurn.playerActionPoint);
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
        player.PlayerActionPointCardState.ResetColorPreviewPath(player.PlayerActionPointCardState.previewPath, player.PlayerActionPointCardState.blocBaseEmissiveColor);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    
  
}
