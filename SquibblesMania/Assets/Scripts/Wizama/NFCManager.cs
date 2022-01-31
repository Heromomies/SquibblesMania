using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine; 
using Wizama.Hardware.Antenna;  
public class NFCManager : MonoBehaviour
{
	public enum Color
	{
		Red = 0,
		Blue = 1, 
		Yellow = 2, 
		Green = 3
	}
	
	public NFC_DEVICE_ID[] nfc;
	public char[] charCards;
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
			charCards = tag.Data.ToCharArray();
			if (charCards[1].Equals('B'))
			{
				Debug.Log("The color card is blue");
			}
		}
		
		
		
	} 
}