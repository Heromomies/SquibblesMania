using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DigitalRubyShared;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    //Manager for simple button Ui
    [Header("MANAGER UI")] public TextMeshProUGUI currentActionPointsOfCurrentPlayerTurn;

    public TextMeshProUGUI turnCountText;
    private static UiManager _uiManager;
    public GameObject buttonNextTurn;
    public GameObject conditionEvent, conditionInventory;

    [Header("WIN PANEL")] public GameObject winPanel;
    public TextMeshProUGUI winText;
    public static UiManager Instance => _uiManager;

    [SerializeField] [Header("CAM SWITCH PARAMETERS")]
    private bool isSwitchChanged;

    public GameObject[] uiCamGameObject;

    private void Awake()
    {
        _uiManager = this;
    }

    public void SwitchUiForPlayer(GameObject buttonNextTurnPlayer, TextMeshProUGUI currentActionPoint)
    {
        buttonNextTurn = buttonNextTurnPlayer;
        currentActionPointsOfCurrentPlayerTurn = currentActionPoint;
        SetUpCurrentActionPointOfCurrentPlayer(GameManager.Instance.currentPlayerTurn.playerActionPoint);
    }

    public void ButtonPathFindingBlock()
    {
        GameManager.Instance.currentPlayerTurn.currentTouchBlock = TouchManager.Instance.Hit.transform;
        GameManager.Instance.currentPlayerTurn.StartPathFinding();
        TouchManager.Instance.uiInteractionParentObject.SetActive(false);
    }

    public void ButtonUpDownBlock()
    {
        TouchManager.Instance.uiInteractionParentObject.SetActive(false);
        MovementBlockManager.Instance.buttonMoveBlockParentObject.SetActive(true);
        MovementBlockManager.Instance.isMovingBlock = true;
    }

    public void SetUpCurrentActionPointOfCurrentPlayer(int actionPointText)
    {
        currentActionPointsOfCurrentPlayerTurn.text = $"Action point : {actionPointText}";
    }

    public void UpdateCurrentTurnCount(int turnCount)
    {
        turnCountText.text = $"Round number : {turnCount}";
    }

    public void ButtonNextTurn()
    {
        GameManager.Instance.currentPlayerTurn.CurrentState.ExitState(GameManager.Instance.currentPlayerTurn);
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void SeeCondition()
    {
        Quaternion conditionEventRotZ = conditionEvent.transform.rotation;
        Quaternion conditionInventoryRotZ = conditionInventory.transform.rotation;

        if (!conditionEvent.activeSelf)
        {
            conditionEventRotZ.z = 0f;
            conditionInventoryRotZ.z = 0f;

            conditionEvent.SetActive(true);
            conditionInventory.SetActive(true);

            conditionEvent.transform.rotation = conditionEventRotZ;
            conditionInventory.transform.rotation = conditionInventoryRotZ;

            if (GameManager.Instance.actualCamPreset.presetNumber == 3 ||
                GameManager.Instance.actualCamPreset.presetNumber == 4)
            {
                conditionEventRotZ.z = 180f;
                conditionInventoryRotZ.z = 180f;
                conditionEvent.transform.rotation = conditionEventRotZ;
                conditionInventory.transform.rotation = conditionInventoryRotZ;
            }
        }
        else
        {
            conditionEvent.SetActive(false);
            conditionInventory.SetActive(false);
        }
    }

    public void WinSetUp(Player.PlayerTeam playerTeam)
    {
        winPanel.SetActive(true);
        winText.text = $"{playerTeam} WIN";
    }

    void SmoothResetCamPosAndRot()
    {
        GameObject cam = Camera.main.gameObject;
        Vector3 targetPos = GameManager.Instance.actualCamPreset.camPos;
        Quaternion targetRot = Quaternion.Euler(GameManager.Instance.actualCamPreset.camRot);
        
        cam.transform.DOMove(targetPos, 0.3f);
        cam.transform.DORotateQuaternion(targetRot, 0.3f);

    }

    public void ButtonChangeCamMoveUi()
    {
        FingersPanOrbitComponentScript cameraTouchMovement =
            Camera.main.gameObject.GetComponent<FingersPanOrbitComponentScript>();


        if (!CameraButtonManager.Instance.enabled)
        {
            CameraButtonManager.Instance.enabled = true;
            cameraTouchMovement.enabled = false;

            CameraButtonManager.Instance.ResetBaseViewButton();
            SmoothResetCamPosAndRot();
            int presetCamNum = GameManager.Instance.actualCamPreset.presetNumber;

            if (presetCamNum == 1 || presetCamNum == 2)
            {
                uiCamGameObject[1].SetActive(false);
                uiCamGameObject[0].SetActive(true);
            }
            else if (presetCamNum == 3 || presetCamNum == 4)
            {
                uiCamGameObject[0].SetActive(false);
                uiCamGameObject[1].SetActive(true);
            }
        }
        else
        {
            CameraButtonManager.Instance.enabled = false;
            cameraTouchMovement.enabled = true;
            SmoothResetCamPosAndRot();
        }
    }

    public void PlayerChangeCamButton()
    {
        int presetCamNum = GameManager.Instance.actualCamPreset.presetNumber;

        if (presetCamNum == 1 || presetCamNum == 2)
        {
            uiCamGameObject[1].SetActive(false);
            uiCamGameObject[0].SetActive(true);
        }
        else if (presetCamNum == 3 || presetCamNum == 4)
        {
            uiCamGameObject[0].SetActive(false);
            uiCamGameObject[1].SetActive(true);
        }
    }
}