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
			case 0 : powers[0].SetActive(activePower);
				break;
			case 1 : powers[1].SetActive(activePower);
				break;
			case 2 : powers[2].SetActive(activePower);
				break;
			case 3 : powers[3].SetActive(activePower);
				break;
		}
	}

	public void ChangeTurnPlayer()
	{
		UiManager.Instance.buttonNextTurn.SetActive(true);
	}
}