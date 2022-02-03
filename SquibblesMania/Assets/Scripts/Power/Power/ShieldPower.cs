using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPower : MonoBehaviour, IManagePower
{
    private int _savePlayerTurn;
    private int _actualTurn;
    
    private PlayerStateManager _player;
    [HideInInspector] public bool activated;

    public void Start()
    {
	    activated = true;
	    _player = GameManager.Instance.currentPlayerTurn;
	    _savePlayerTurn = GameManager.Instance.turnCount;
	    _player.gameObject.layer = 0;
	    //GameManager.Instance.currentPlayerTurn.CurrentState.ExitState(GameManager.Instance.currentPlayerTurn);
	    
	    PowerManager.Instance.ChangeTurnPlayer();
	    
	    Debug.Log(_player.name);
	    Debug.Log(_savePlayerTurn);
    }

    public void ShowPower()
    {
	   
    }

    public void LaunchPower()
    {
	    
	    
    }

    public void ChangeTurn()
    {
	    if (GameManager.Instance.turnCount == _savePlayerTurn + 2)
	    {
		    Debug.Log("Player regain his layer");
		    _player.gameObject.layer = 6;
	    }
	    if (GameManager.Instance.turnCount == _savePlayerTurn + 4)
	    {
		    Debug.Log("Player gain two action point");
		    _player.PlayerActionPointCardState.actionPoint += 2;
	    }
    }
}
