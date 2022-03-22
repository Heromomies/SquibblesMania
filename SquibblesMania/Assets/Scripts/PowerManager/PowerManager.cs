using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerManager : MonoBehaviour
{
    public List<GameObject> powers;
   

    #region Singleton

    private static PowerManager powerManager;

    public static PowerManager Instance => powerManager;
    // Start is called before the first frame update

    private void Awake()
    {
        powerManager = this;
    }

    #endregion

    public void ActivateDeactivatePower(int powerIndex, bool activePower)
    {
        switch (powerIndex)
        {
            case 0: powers[0].gameObject.SetActive(activePower); break;
            case 1: powers[1].gameObject.SetActive(activePower); break;
            case 2: powers[2].gameObject.SetActive(activePower); break;
            case 3: powers[3].gameObject.SetActive(activePower); break;
        }
    }

    public void CyclePassed()
    {
       
    }

    public void ChangeTurnPlayer()
    {
        if (NFCManager.Instance.hasRemovedCard)
        {
            UiManager.Instance.buttonNextTurn.SetActive(true);
        }
        else
        {
            switch (GameManager.Instance.actualCamPreset.presetNumber)
            {
                case 1: NFCManager.Instance.actionPlayerPreset[0].textTakeOffCard.gameObject.SetActive(true); break;
                case 2: NFCManager.Instance.actionPlayerPreset[0].textTakeOffCard.gameObject.SetActive(true); break;
                case 3: NFCManager.Instance.actionPlayerPreset[1].textTakeOffCard.gameObject.SetActive(true); break;
                case 4: NFCManager.Instance.actionPlayerPreset[1].textTakeOffCard.gameObject.SetActive(true); break;
            }
        }
    }
}