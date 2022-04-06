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
			panelPause.SetActive(true);
		}
		else 
		{
			panelPause.SetActive(false);
		}
		AudioManager.Instance.Play("Button");
	}

    public void Return()
    {
	    AudioManager.Instance.Play("Button");
        panelPause.transform.rotation *= Quaternion.Euler(0, 0, 180);
    }
}
