using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPower : MonoBehaviour, IManagePower
{
	public int durationShield;
	
    public void OnEnable()
    {
	    GameManager.Instance.currentPlayerTurn.isPlayerShielded = true;
	    GameManager.Instance.currentPlayerTurn.shieldCount = durationShield;
	    GameManager.Instance.currentPlayerTurn.gameObject.layer = 3;
	    transform.position = GameManager.Instance.currentPlayerTurn.transform.position;

	    StartCoroutine(CoroutineShield());
    }

	IEnumerator CoroutineShield()
	{
		yield return new WaitForSeconds(5f);
		
	    PowerManager.Instance.ActivateDeactivatePower(3, false);
	    PowerManager.Instance.ChangeTurnPlayer();
    }
    
    public void ShowPower()
    {
	   
    }

    public void LaunchPower()
    {
    }
}
