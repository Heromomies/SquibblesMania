using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wizama.Hardware.Antenna;
using Wizama.Hardware.Light;

public class PlayerStateEventManager : MonoBehaviour
{
    
    private static PlayerStateEventManager _playerStateEventManager;

    public static PlayerStateEventManager Instance => _playerStateEventManager;

    public event Action<int, bool> ONPlayerStunTextTriggerEnter;
    public event Action<PlayerStateManager, int> ONPlayerStunTriggerEnter;
    void Awake()
    {
        _playerStateEventManager = this;   
    }


    public void PlayerStunTextTriggerEnter(int actualCamPreset, bool isSetActive)
    {
        if (ONPlayerStunTextTriggerEnter != null)
        {
            ONPlayerStunTextTriggerEnter(actualCamPreset, isSetActive);
        }
    }

    public void PlayerStunTriggerEnter(PlayerStateManager player, int stunCount)
    {
        if (ONPlayerStunTriggerEnter != null)
        {
            ONPlayerStunTriggerEnter(player, stunCount);
        }

        if (player == GameManager.Instance.currentPlayerTurn)
        {
            NFCController.StopPolling();
            LightController.ShutdownAllLights();
        }
    }
}
