using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DigitalRubyShared;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TouchPhase = UnityEngine.TouchPhase;

public class UiManager : MonoBehaviour
{
    //Manager for simple button Ui
    [Header("MANAGER UI")]
    private static UiManager _uiManager;
    [HideInInspector]
    public Slider sliderNextTurn;

    [Header("WIN PANEL")] 
    public float valueBeforeValidateSlider;
    public GameObject winPanel;
    public GameObject textTeamOne, textTeamTwo;
    [SerializeField] private GameObject playersUiGlobal;
    [SerializeField] private Image imagePanelEnd;
    [SerializeField] private Sprite spritesWinPanel;
    [SerializeField] private SquipyAnimTween winSquipyAnimTween, looseSquipyAnimTween;
    public static UiManager Instance => _uiManager;

    [Header("CARD UI VFX")]
    public Transform[] parentSpawnCardUiVFX;
    
    [Header("POP UP TEXT PARAMETERS")]
    public GameObject textActionPointPopUp;
    public int totalCurrentActionPoint;
    [SerializeField] private Vector3 offsetText;
    public Camera uiCam;

    [Header("STUN TEXT PARAMETERS")]
    [SerializeField] private UiPlayerStun[] uiPlayerStuns;


    [Serializable]
    public struct UiPlayerStun
    {
        public GameObject playerStunTextParent;
        public Transform[] arrowSprite;
    }
    private void Awake()
    {
        _uiManager = this;
    }

    public void OnPointerDown(Image circleToMove)
    {
        circleToMove.color = Color.white;
    }

    public void OnPointerUp(Image circleToMove)
    {
        circleToMove.color = Color.black;
        
        if (sliderNextTurn.value < valueBeforeValidateSlider)
        {
            sliderNextTurn.value = 0f;
        }
    }
    
    public void MoveSliderDemiCircle(Image demiCircleOnTop)
    {
        if (sliderNextTurn.value >= valueBeforeValidateSlider)
        {
            demiCircleOnTop.color = Color.white;
        }
        else
        {
            demiCircleOnTop.color = Color.black;
        }
    }
    
    public void MoveSliderCircleToMove(Image circleToMove)
    {
        circleToMove.color = Color.white;
    }
    
    public void EndDragSliderCircleToMove(Image circleToMove) // When we change the value of the slider
    {
        if (sliderNextTurn.value >= valueBeforeValidateSlider)
        {
            NextTurn();
        }
        
        circleToMove.color = Color.black;
        sliderNextTurn.value = 0f;
    }
    
    public void EndDragSliderDemiCircle(Image demiCircleOnTop) // When we change the value of the slider
    {
        demiCircleOnTop.color = Color.black;
    }
    
    private void Start()
    {
        PlayerStateEventManager.Instance.ONPlayerStunTextTriggerEnter += StunTextPopUp;
    }

    public void SwitchUiForPlayer(Slider buttonNextTurnPlayer)
    {
        sliderNextTurn = buttonNextTurnPlayer;
        sliderNextTurn.gameObject.SetActive(true);
    }


    public void NextTurn()
    {
        AudioManager.Instance.Play("ButtonNextTurn");
        NFCManager.Instance.numberOfTheCard = 0;
        NFCManager.Instance.displacementActivated = false;
        NFCManager.Instance.newCardDetected = false;
        NFCManager.Instance.powerActivated = false;
        PowerManager.Instance.isPlayerInJumpOrSwap = false;
        
        PlayerStateManager currentPlayer = GameManager.Instance.currentPlayerTurn;
        CameraButtonManager.Instance.isCamRotateButtonPressed = false;
        
        if (currentPlayer.isPlayerStun)
        {
            PlayerStateEventManager.Instance.PlayerStunTextTriggerEnter(GameManager.Instance.actualCamPreset.presetNumber, false);
            currentPlayer.stunCount--;
            currentPlayer.stunCount = (int)Mathf.Clamp( currentPlayer.stunCount, 0, Mathf.Infinity);
        }
        sliderNextTurn.gameObject.SetActive(false);
        currentPlayer.canSwitch = true;
        currentPlayer.CurrentState.ExitState(GameManager.Instance.currentPlayerTurn);
       
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        textTeamOne.SetActive(false);
        textTeamTwo.SetActive(false);
    }


    private void StunTextPopUp(int actualCamPresetNumber, bool setActiveGameObject)
    {

        if (actualCamPresetNumber <= 2)
        {
            uiPlayerStuns[0].playerStunTextParent.SetActive(setActiveGameObject);
            Transform spriteArrow;
            switch (GameManager.Instance.currentPlayerTurn.playerNumber)
            {
                case 0: spriteArrow = uiPlayerStuns[0].arrowSprite[1];
                        spriteArrow.gameObject.SetActive(setActiveGameObject); break;
                case 2: spriteArrow = uiPlayerStuns[0].arrowSprite[0]; 
                        spriteArrow.gameObject.SetActive(setActiveGameObject); break;
            }
        }
        else
        {
            uiPlayerStuns[1].playerStunTextParent.SetActive(setActiveGameObject);
            Transform spriteArrow;
            switch (GameManager.Instance.currentPlayerTurn.playerNumber)
            {
                case 1: spriteArrow = uiPlayerStuns[1].arrowSprite[1]; 
                    spriteArrow.gameObject.SetActive(setActiveGameObject); break;
                case 3: spriteArrow = uiPlayerStuns[1].arrowSprite[0];
                    spriteArrow.gameObject.SetActive(setActiveGameObject); break;
            }
        }
    }
    
    public void WinSetUp(Player.PlayerTeam currentPlayerTeam)
    {
        winPanel.SetActive(true);
        playersUiGlobal.SetActive(false);
        imagePanelEnd.sprite = spritesWinPanel;
        
        var currentPlayer = GameManager.Instance.currentPlayerTurn;
        PlayerStateManager otherPlayer = null;
        
        foreach (var player in GameManager.Instance.players)
        {
            if (player.playerTeam != currentPlayerTeam)
            {
                otherPlayer = player;
                break;
            }
                
        }
        
        if (GameManager.Instance.volume.profile.TryGet(out DepthOfField depthOfField))
        {
            depthOfField.active = true;
        }

        if (otherPlayer != null)
        {
            if (currentPlayerTeam == Player.PlayerTeam.TeamOne)
            {
                textTeamOne.SetActive(true);
                winSquipyAnimTween.imgSquipy.color = currentPlayer.playerColor;
                looseSquipyAnimTween.imgSquipy.color = otherPlayer.playerColor;
            }
            else
            {
                textTeamTwo.SetActive(true);
                winPanel.transform.rotation *= Quaternion.Euler(0,0,180f);
                winSquipyAnimTween.imgSquipy.color = currentPlayer.playerColor;
                looseSquipyAnimTween.imgSquipy.color = otherPlayer.playerColor;
            }
        }
      
    }
    
    #region SpawnTextActionPoint

    public void SpawnTextActionPointPopUp(Transform currentPlayer)
    {
        totalCurrentActionPoint = GameManager.Instance.currentPlayerTurn.playerActionPoint;
        if (!textActionPointPopUp)
        {
            textActionPointPopUp = PoolManager.Instance.SpawnObjectFromPool("PopUpTextActionPoint", currentPlayer.position + offsetText, Quaternion.identity, currentPlayer);
        }
        else
        {
            textActionPointPopUp.SetActive(true);
        }
        
        textActionPointPopUp.GetComponent<PopUpTextActionPoint>().SetUpText(GameManager.Instance.currentPlayerTurn.playerActionPoint);
    }

    #endregion
    
}