using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wizama.Hardware.Antenna;
using Wizama.Hardware.Light;  

public class NFCManager : MonoBehaviour
{
	#region ANTENNA SETTINGS
	[Header("ANTENNA SETTINGS")]
	public NFC_DEVICE_ID[] antennaPlayerOne; [Space]
	public NFC_DEVICE_ID[] antennaPlayerTwo; [Space]
	public NFC_DEVICE_ID[] antennaPlayerThree; [Space]
	public NFC_DEVICE_ID[] antennaPlayerFour;
	#endregion

	#region UI SETTINGS

	[Space]
	[Header("UI SETTINGS")]
	public Button chosePower;
	public Button choseToMove;
	public TextMeshProUGUI textTakeOffCard;

	#endregion

	#region LIGHT SETTINGS

	[Space]
	[Header("LIGHT SETTINGS")]
	public LIGHT_INDEX[] lightIndexesPlayerOne;
	public LIGHT_INDEX[] lightIndexesPlayerTwo;
	public LIGHT_INDEX[] lightIndexesPlayerThree;
	public LIGHT_INDEX[] lightIndexesPlayerFour;
	public List<LIGHT_COLOR> lightColor;

	public List<LIGHT_INDEX> fullIndex;
	
	#endregion

	#region PRIVATE VAR

	[HideInInspector] public int colorInt;
	[HideInInspector] public int numberOfTheCard;
	[HideInInspector] public char[] charCards;
	[HideInInspector] public bool hasRemovedCard;
	[HideInInspector] public bool clicked;
	[HideInInspector] public int changeColor;
	#endregion

	#region Singleton

	private static NFCManager nfcManager;

	public static NFCManager Instance => nfcManager;
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
			case 0 : NFCController.StartPollingAsync(antennaPlayerOne);
				StartCoroutine(ColorOneRange(lightIndexesPlayerOne,0.5f));
				break;
			case 1 : NFCController.StartPollingAsync(antennaPlayerTwo);
				StartCoroutine(ColorOneRange(lightIndexesPlayerTwo,0.5f));
				break;
			case 2 : NFCController.StartPollingAsync(antennaPlayerThree);
				StartCoroutine(ColorOneRange(lightIndexesPlayerThree,0.5f));
				break;
			case 3 : NFCController.StartPollingAsync(antennaPlayerFour);
				StartCoroutine(ColorOneRange(lightIndexesPlayerFour,0.5f));
				break;
		}
	}

	private IEnumerator ColorOneRange(LIGHT_INDEX[] lightIndex, float timeBetweenTwoLight)
	{
		for (int i = 0; i < lightColor.Capacity; i++)
		{
			if (i == lightColor.Capacity-1)
			{
				i = 0;
			}
			LightController.Colorize(lightIndex, lightColor[i],false);
			yield return new WaitForSeconds(timeBetweenTwoLight);
		}
	}

	private IEnumerator ColorOneByOneAllTheAntennas()
	{
		for (int i = 0; i < fullIndex.Count; i++)
		{
			changeColor++;
			if (i == fullIndex.Count-1)
			{
				i = 0;
			}
			
			if (changeColor == lightColor.Count)
			{
				changeColor = 0;
			}

			LightController.ColorizeOne(fullIndex[i], lightColor[changeColor], false);
			yield return new WaitForSeconds(0.2f);
		}
	}
	
	private void OnDisable()  // Stop polling on disable, can't detect card
	{  
		NFCController.StopPolling();  
	}

	public void ChoseToLaunchPower() // If the player chose to launch a power 
	{
		clicked = true;
		switch (charCards[1]) // Check the letter of the card for the color and launch the appropriate power
		{
			case 'B' : colorInt = 0;
				PowerManager.Instance.ActivateDeactivatePower(colorInt,true);
				break;
			case 'R' : colorInt = 1;
				PowerManager.Instance.ActivateDeactivatePower(colorInt,true);
				break;
			case 'G' : colorInt = 2;
				PowerManager.Instance.ActivateDeactivatePower(colorInt,true);
				break;
			case 'Y' : colorInt = 3;
				PowerManager.Instance.ActivateDeactivatePower(colorInt,true);
				break;
		}
		SetActiveButton(false);
	}

	public void ChoseToMove() // If the player chose to move, his displacements are equals to the value of the card
	{
		clicked = true;
		numberOfTheCard = charCards[0] - '0';
		GameManager.Instance.currentPlayerTurn.StartState();
		
		SetActiveButton(false);
	}
	
	public void SetActiveButton(bool setActive) // Can activate / deactivate button from everywhere in the script
	{
		choseToMove.gameObject.SetActive(setActive);
		chosePower.gameObject.SetActive(setActive);
	}
}