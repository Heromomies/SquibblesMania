using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    public GameObject panelPause;
    
    // Start is called before the first frame update
	public void Pause()
	{
		if (!panelPause.activeSelf)
		{
			AudioManager.Instance.Stop("MainSound");
			panelPause.SetActive(true);
		}
		else 
		{
			AudioManager.Instance.Play("MainSound");
			panelPause.SetActive(false);
		}
		AudioManager.Instance.Play("Button");
	}
}
