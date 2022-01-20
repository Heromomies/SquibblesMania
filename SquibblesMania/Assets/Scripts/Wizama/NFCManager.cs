using System.Collections.Generic;
using TMPro;
using UnityEngine; 
using Wizama.Hardware.Antenna;  
public class NFCManager : MonoBehaviour
{
	public NFC_DEVICE_ID[] nfc;
	void Start()  {  
		NFCController.StartPolling(nfc);
	}   
    
	void OnDisable()  {  
		NFCController.StopPolling();  
	}   
    
	void FixedUpdate()  {  
		/*Debug.Log(NFC_DEVICE_ID.ANTENNA_1);
		Debug.Log(NFCController.CanReadNFC);
		Debug.Log(NFCController.GetTags(NFC_DEVICE_ID.ANTENNA_1));*/
		
		/*List<NFCTag> antenna1Tags = NFCController.GetTags(NFC_DEVICE_ID.ANTENNA_1);
		foreach (NFCTag tag in antenna1Tags)  
			Debug.Log(tag.Data + " placed on antenna 1");*/
        
		List<NFCTag>[] allAntennaTags = NFCController.GetTags(); 
		int count = 0;  
		foreach (List<NFCTag> Tags in allAntennaTags)  
			count += Tags.Count;   
		
		Debug.Log("There is " + count + " tags detected in total");
		
	} 
}