using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerManager : MonoBehaviour
{
	public List<GameObject> powers;
	[HideInInspector] public bool jumpOrSwapActivated;
	
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
		if (EndZoneManager.Instance != null)
		{
			EndZoneManager.Instance.CheckPlayersTeam();
			EndZoneManager.Instance.PlayersIsOnEndZone();
		}
		switch (powerIndex)
		{
			case 0: powers[0].gameObject.SetActive(activePower); jumpOrSwapActivated = true; break;
			case 1: powers[1].gameObject.SetActive(activePower); jumpOrSwapActivated = false; break;
			case 2: powers[2].gameObject.SetActive(activePower); jumpOrSwapActivated = true; break;
			case 3: powers[3].gameObject.SetActive(activePower); jumpOrSwapActivated = false; break;
		}
	}

	public void CyclePassed()
	{
	}

	public void ChangeTurnPlayer()
	{
		if (NFCManager.Instance.powerActivated)
		{
			jumpOrSwapActivated = false;
			UiManager.Instance.buttonNextTurn.SetActive(true);
		}
	}
}