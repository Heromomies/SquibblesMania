using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Wizama.Hardware.Antenna;
using Wizama.Hardware.Light;

public class NFCManager : MonoBehaviour
{
    #region ANTENNA SETTINGS

    [Header("ANTENNA SETTINGS")] public NFC_DEVICE_ID[] antennaPlayerOne;
    [Space] public NFC_DEVICE_ID[] antennaPlayerTwo;
    [Space] public NFC_DEVICE_ID[] antennaPlayerThree;
    [Space] public NFC_DEVICE_ID[] antennaPlayerFour;

    #endregion

    #region UI SETTINGS

    [Space] [Header("UI SETTINGS")] public ActionPlayerPreset[] actionPlayerPreset;
    [Serializable]
    public struct ActionPlayerPreset
    {
        public GameObject playerActionButton;
        public GameObject playerPowerButton;
        public TextMeshProUGUI textTakeOffCard;
    }

    #endregion

    #region LIGHT SETTINGS

    [Space] [Header("LIGHT SETTINGS")] public LIGHT_INDEX[] lightIndexesPlayerOne;
    public LIGHT_INDEX[] lightIndexesPlayerTwo;
    public LIGHT_INDEX[] lightIndexesPlayerThree;
    public LIGHT_INDEX[] lightIndexesPlayerFour;
    public List<LIGHT_COLOR> lightColor;

    public List<LIGHT_INDEX> fullIndex;

    #endregion

    #region PRIVATE VAR
    
    [HideInInspector] public int numberOfTheCard;
    public char[] charCards;
    [HideInInspector] public bool hasRemovedCard;
    [HideInInspector] public bool clicked;
    [HideInInspector] public int changeColor;

    #endregion

    #region Singleton

    private static NFCManager nfcManager;

    public static NFCManager Instance => nfcManager;

    private WaitForSeconds _timeBetweenTwoLight = new WaitForSeconds(0.5f);

    private WaitForSeconds _timeAntennaOneByOne = new WaitForSeconds(0.2f);
    // Start is called before the first frame update

    private void Awake()
    {
        nfcManager = this;
    }

    #endregion

    public void PlayerChangeTurn() // When we change the turn of the player, the color and the antenna who can detect change too
    {
        StopAllCoroutines();
        NFCController.StopPolling();
        clicked = false;
        switch (GameManager.Instance.currentPlayerTurn.playerNumber)
        {
            case 0:
                NFCController.StartPollingAsync(antennaPlayerOne);
                LightController.Colorize(lightIndexesPlayerOne, lightColor[0], false);
                break;
            case 1:
                NFCController.StartPollingAsync(antennaPlayerTwo);
                LightController.Colorize(lightIndexesPlayerTwo, lightColor[1], false);
                break;
            case 2:
                NFCController.StartPollingAsync(antennaPlayerThree);
                LightController.Colorize(lightIndexesPlayerThree, lightColor[0], false);
                break;
            case 3:
                NFCController.StartPollingAsync(antennaPlayerFour);
                LightController.Colorize(lightIndexesPlayerFour, lightColor[1], false);
                break;
        }
    }

    private IEnumerator ColorOneRange(LIGHT_INDEX[] lightIndex) // Color One range with different colors
    {
        for (int i = 0; i < lightColor.Capacity; i++)
        {
            if (i == lightColor.Capacity - 1)
            {
                i = 0;
            }

            LightController.Colorize(lightIndex, lightColor[i], false);
            yield return _timeBetweenTwoLight;
        }
    }

    public IEnumerator ColorOneByOneAllTheAntennas() // Color One by One all the antennas 
    {
        for (int i = 0; i < fullIndex.Count; i++)
        {
            changeColor++;
            if (i == fullIndex.Count - 1)
            {
                i = 0;
            }

            if (changeColor == lightColor.Count)
            {
                changeColor = 0;
            }

            LightController.ColorizeOne(fullIndex[i], lightColor[changeColor], false);
            yield return _timeAntennaOneByOne;
        }
    }

    private void OnDisable() // Stop polling on disable, can't detect card
    {
        NFCController.StopPolling();
    }

    public void ChoseToLaunchPower() // If the player chose to launch a power 
    {
        clicked = true;
        GameManager.Instance.currentPlayerTurn.SwitchState(GameManager.Instance.currentPlayerTurn.PlayerPowerCardState);
        switch (charCards[1]) // Check the letter of the card for the color and launch the appropriate power
        {
            case 'B': PowerManager.Instance.ActivateDeactivatePower(0, true); break;
            case 'R': PowerManager.Instance.ActivateDeactivatePower(1, true); break;
            case 'G': PowerManager.Instance.ActivateDeactivatePower(2, true); break;
            case 'Y': PowerManager.Instance.ActivateDeactivatePower(3, true); break;
        }

        SetActivePlayerActionButton(1,false);
    }

    public void ChoseToMove() // If the player chose to move, his displacements are equals to the value of the card
    {
        clicked = true;
        numberOfTheCard = charCards[0] - '0';
        GameManager.Instance.currentPlayerTurn.playerActionPoint = numberOfTheCard;
        
        if (GameManager.Instance.currentPlayerTurn.isPlayerShielded)
        {
            GameManager.Instance.currentPlayerTurn.playerActionPoint += 2;
        }

        UiManager.Instance.SetUpCurrentActionPointOfCurrentPlayer(GameManager.Instance.currentPlayerTurn.playerActionPoint);
        
        GameManager.Instance.currentPlayerTurn.isPlayerInActionCardState = true;
        GameManager.Instance.currentPlayerTurn.SwitchState(GameManager.Instance.currentPlayerTurn
            .PlayerActionPointCardState);

        SetActivePlayerActionButton(0,false);
    }

    public void SetActivePlayerActionButton(int index, bool setActive) // Can activate / deactivate button from everywhere in the script
    {
        if (index == 0)
        {
            switch (GameManager.Instance.actualCamPreset.presetNumber)
            {
                case 1: actionPlayerPreset[0].playerActionButton.SetActive(setActive); break;
                case 2: actionPlayerPreset[0].playerActionButton.SetActive(setActive); break;
                case 3: actionPlayerPreset[1].playerActionButton.SetActive(setActive); break;
                case 4: actionPlayerPreset[1].playerActionButton.SetActive(setActive); break;
            }
        } else if (index == 1)
        {
            switch (GameManager.Instance.actualCamPreset.presetNumber)
            { 
                case 1: actionPlayerPreset[0].playerPowerButton.SetActive(setActive); break;
                case 2: actionPlayerPreset[0].playerPowerButton.SetActive(setActive); break;
                case 3: actionPlayerPreset[1].playerPowerButton.SetActive(setActive); break;
                case 4: actionPlayerPreset[1].playerPowerButton.SetActive(setActive); break;
            }
        }
       
    }
}