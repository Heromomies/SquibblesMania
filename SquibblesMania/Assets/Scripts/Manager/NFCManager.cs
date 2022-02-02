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
	private char[] _charCards;
	[HideInInspector] public bool hasRemovedCard;
	private bool _cliked;
	private int _changeColor;
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
	
	void Start() // Launch the polling function to detect card
	{  
		NFCController.OnNewTag = OnNewTagDetected;
		NFCController.OnTagRemoved = OnTagRemoveDetected;
		NFCController.StartPollingAsync(antennaPlayerOne);
	}

	public void PlayerChangeTurn() // When we change the turn of the player, the color and the antenna who can detect change too
	{
		StopAllCoroutines();
		switch (GameManager.Instance.currentPlayerTurn.playerNumber)
		{
			case 0 : NFCController.StartPollingAsync(antennaPlayerOne);
				//StartCoroutine(ColorRGB(lightIndexesPlayerOne,0.5f));
				StartCoroutine(ColorOneByOne());
				break;
			case 1 : NFCController.StartPollingAsync(antennaPlayerTwo);
				//StartCoroutine(ColorRGB(lightIndexesPlayerTwo,0.5f));
				StartCoroutine(ColorOneByOne());
				break;
			case 2 : NFCController.StartPollingAsync(antennaPlayerThree);
				StartCoroutine(ColorRGB(lightIndexesPlayerThree,0.5f));
				break;
			case 3 : NFCController.StartPollingAsync(antennaPlayerFour);
				StartCoroutine(ColorRGB(lightIndexesPlayerFour,0.5f));
				break;
		}
	}

	private IEnumerator ColorRGB(LIGHT_INDEX[] lightIndex, float timeBetweenTwoLight)
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

	private IEnumerator ColorOneByOne()
	{
		for (int i = 0; i < fullIndex.Count; i++)
		{
			_changeColor++;
			if (i == fullIndex.Count-1)
			{
				i = 0;
			}
			
			if (_changeColor == lightColor.Count)
			{
				_changeColor = 0;
			}

			LightController.ColorizeOne(fullIndex[i], lightColor[_changeColor], false);
			yield return new WaitForSeconds(0.2f);
		}
	}
	
	private void OnDisable()  // Stop polling on disable, can't detect card
	{  
		NFCController.StopPolling();  
	}    
    
	private void OnNewTagDetected(NFC_DEVICE_ID device, NFCTag nfcTag)  // When the player put a card on the tablet
	{
		if (!GameManager.Instance.currentPlayerTurn.isPlayerInActionCardState)
		{
			SetActiveButton(true);
		
			_charCards = nfcTag.Data.ToCharArray();
			UiManager.Instance.buttonNextTurn.SetActive(false);
			hasRemovedCard = false;
		}
		else
		{
			Debug.Log("Don't detect another card");
			UiManager.Instance.buttonNextTurn.SetActive(false);
		}
	}
	
	private void OnTagRemoveDetected(NFC_DEVICE_ID device, NFCTag nfcTag) // When a card is removed
	{
		if (GameManager.Instance.currentPlayerTurn.playerActionPoint == 0 && !_cliked)
		{
			SetActiveButton(false);
			UiManager.Instance.buttonNextTurn.SetActive(false);
		}
		else if(GameManager.Instance.currentPlayerTurn.playerActionPoint == 0 && _cliked)
		{
			Debug.Log("I'm in");
			SetActiveButton(false);
			textTakeOffCard.text = "";
			UiManager.Instance.buttonNextTurn.SetActive(true);
			NFCController.StopPolling();
		}
		else if(GameManager.Instance.currentPlayerTurn.playerActionPoint > 0 && !_cliked)
		{
			UiManager.Instance.buttonNextTurn.SetActive(false);
		}
		hasRemovedCard = true;
	}

	private void Update()
	{
		if (hasRemovedCard && GameManager.Instance.currentPlayerTurn.playerActionPoint == 0 && _cliked)
		{
			Debug.Log("I'm in the update");
			UiManager.Instance.buttonNextTurn.SetActive(true);
			textTakeOffCard.text = "";
			_cliked = false;
			hasRemovedCard = false;
		} 
	}

	public void ChoseToLaunchPower() // If the player chose to launch a power 
	{
		_cliked = true;
		switch (_charCards[1]) // Check the letter of the card for the color and launch the appropriate power
		{
			case 'B' : colorInt = 0;
				break;
			case 'R' : colorInt = 1;
				PowerManager.Instance.ActivateDeactivatePower(colorInt,true);
				break;
			case 'G' : colorInt = 2;
				break;
			case 'Y' : colorInt = 3;
				break;
		}
		SetActiveButton(false);
	}

	public void ChoseToMove() // If the player chose to move, his displacements are equals to the value of the card
	{
		_cliked = true;
		numberOfTheCard = _charCards[0] - '0';
		GameManager.Instance.currentPlayerTurn.StartState();
		
		SetActiveButton(false);
	}

	private void SetActiveButton(bool setActive) // Can activate / deactivate button from everywhere in the script
	{
		choseToMove.gameObject.SetActive(setActive);
		chosePower.gameObject.SetActive(setActive);
	}
}