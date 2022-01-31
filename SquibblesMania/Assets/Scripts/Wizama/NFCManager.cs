using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wizama.Hardware.Antenna;  
public class NFCManager : MonoBehaviour
{
	public NFC_DEVICE_ID[] antennaPlayerOne; [Space]
	public NFC_DEVICE_ID[] antennaPlayerTwo; [Space]
	public NFC_DEVICE_ID[] antennaPlayerThree; [Space]
	public NFC_DEVICE_ID[] antennaPlayerFour; [Space]
	public int numberOfTheCard;
	private char[] _charCards;
	
	[HideInInspector] public int colorInt;
	[HideInInspector] public bool cardDetected;
	
	#region Singleton

	private static NFCManager nfcManager;

	public static NFCManager Instance => nfcManager;
	// Start is called before the first frame update

	private void Awake()
	{
		nfcManager = this;
	}

	#endregion
	
	void Start()  {  
		NFCController.StartPolling(antennaPlayerOne);
	}   
    
	void OnDisable()  {  
		NFCController.StopPolling();  
	}   
	
	void FixedUpdate()
	{
		if (!cardDetected)
		{
			switch (GameManager.Instance.currentPlayerTurn.playerNumber)
			{
				case 0 : GetTagOfTheCard(antennaPlayerOne);
					break;
				case 1 : GetTagOfTheCard(antennaPlayerTwo);
					break;
				case 2 : GetTagOfTheCard(antennaPlayerThree);
					break;
				case 3 : GetTagOfTheCard(antennaPlayerFour);
					break;
			}
		}
	}

	void GetTagOfTheCard(NFC_DEVICE_ID[] nfcDeviceIds)
	{
		List<NFCTag> detectedCardTag = NFCController.GetTags(nfcDeviceIds);
		
		foreach (NFCTag nfcTag in detectedCardTag)
		{
			Debug.Log("I'm in the foreach");
			_charCards = nfcTag.Data.ToCharArray();
			
			DetectedCard(_charCards[0] - '0', _charCards[1]);
		}
	}
	private void DetectedCard(int valueCard, char colorCard)
	{
		numberOfTheCard = valueCard;
		
		GameManager.Instance.currentPlayerTurn.StartState();
		
		switch (colorCard)
		{
			case 'B' : colorInt = 0;
				break;
			case 'R' : colorInt = 1;
				break;
			case 'G' : colorInt = 2;
				break;
			case 'Y' : colorInt = 3;
				break;
		}
		
		cardDetected = true;
	}
}