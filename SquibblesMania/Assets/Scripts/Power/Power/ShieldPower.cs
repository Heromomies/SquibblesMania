using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPower : MonoBehaviour, IManagePower
{
    private PlayerStateManager _player;
    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPower()
    {
        //TODO instantiate VFX
    }

    public void LaunchPower()
    {
        _player = GameManager.Instance.currentPlayerTurn;	
        
        
    }
}
