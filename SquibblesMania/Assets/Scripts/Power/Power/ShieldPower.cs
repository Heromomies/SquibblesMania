using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPower : MonoBehaviour
{
	public int durationShield;
	public GameObject vfxShield;

	public int numberOfCycleBeforeDeactivateShield;
	[HideInInspector] public GameObject shieldGameObject;
	
    public void OnEnable()
    {
	    var cPlayerTurn = GameManager.Instance.currentPlayerTurn;
	    
	    cPlayerTurn.isPlayerShielded = true;
	    cPlayerTurn.shieldCount = durationShield;
	    cPlayerTurn.gameObject.layer = 3;

	    GameObject s = Instantiate(vfxShield, cPlayerTurn.transform.position, Quaternion.identity);

		shieldGameObject = s;
	    StartCoroutine(CoroutineShield());
    }

	IEnumerator CoroutineShield()
	{
		yield return new WaitForSeconds(0.2f);

		PowerManager.Instance.ActivateDeactivatePower(3, false);
	    PowerManager.Instance.ChangeTurnPlayer();
    }
}
