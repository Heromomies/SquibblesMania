using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerManager : MonoBehaviour
{
	public GrabPower grab;
	public DashPower dash;
	public SwapPower swap;
	public ShieldPower shield;

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
			case 0: grab.gameObject.SetActive(activePower); break;
			case 1: dash.gameObject.SetActive(activePower); break;
			case 2: swap.gameObject.SetActive(activePower); break;
			case 3: shield.gameObject.SetActive(activePower); break;
		}
	}

	public void CyclePassed()
	{
		if (shield.numberOfCycleBeforeDeactivateShield < 1)
		{
			shield.numberOfCycleBeforeDeactivateShield++;
		}
		else
		{
			Destroy(shield.shield);
		}
	}

	public void ChangeTurnPlayer()
	{
		UiManager.Instance.buttonNextTurn.SetActive(true);
	}
}