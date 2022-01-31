using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wizama.Hardware.Antenna;  
public class NFCManager : MonoBehaviour
{
	public NFC_DEVICE_ID[] nfc;
	public char[] _charCards;
	public int numberOfTheCard;
	[HideInInspector] public int colorInt;
	
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
		NFCController.StartPolling(nfc);
	}   
    
	void OnDisable()  {  
		NFCController.StopPolling();  
	}   
	
	void Update()
	{
		List<NFCTag> antenna = NFCController.GetTags(nfc);
		foreach (NFCTag tag in antenna)
		{
			//Debug.Log("the tag data is " + tag.Data);
			_charCards = tag.Data.ToCharArray();
			
			DetectedCard(_charCards[0] - '0', _charCards[1]);
		}
	}

	public void DetectedCard(int valueCard, char colorCard)
	{
		numberOfTheCard = valueCard;
		
		Debug.Log(numberOfTheCard);
		
		switch (colorCard)
		{
			case 'B' :
				colorInt = 0;
				Debug.Log("Blue + " + valueCard);
				break;
			case 'R' :
				colorInt = 1;
				Debug.Log("Red");
				break;
			case 'G' :
				colorInt = 2;
				Debug.Log("Green");
				break;
			case 'Y' :
				colorInt = 3;
				Debug.Log("Yellow");
				break;
		}
	}
}