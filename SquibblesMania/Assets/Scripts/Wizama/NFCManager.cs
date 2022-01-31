using System.Collections.Generic;
using TMPro;
using UnityEngine; 
using Wizama.Hardware.Antenna;  
public class NFCManager : MonoBehaviour
{
	void Start()  {  
		NFCController.StartPolling( NFC_DEVICE_ID.ANTENNA_1, NFC_DEVICE_ID.ANTENNA_8, NFC_DEVICE_ID.ANTENNA_14, NFC_DEVICE_ID.ANTENNA_21);
	}   
    
	void OnDisable()  {  
		NFCController.StopPolling();  
	}   
    
	void FixedUpdate()
	{
		List<NFCTag> antenna1Tags = NFCController.GetTags(NFC_DEVICE_ID.ANTENNA_1);
		foreach (NFCTag tag in antenna1Tags)  
			Debug.Log(tag.Data + " placed on antenna 1");
        
		List<NFCTag>[] allAntennaTags = NFCController.GetTags(); 
		int count = 0;  
		foreach (List<NFCTag> Tags in allAntennaTags)  
			count += Tags.Count;   
            
		//one.text = "There is " + count + " tags detected in total";  
		Debug.Log("There is " + count + " tags detected in total");
	} 
}