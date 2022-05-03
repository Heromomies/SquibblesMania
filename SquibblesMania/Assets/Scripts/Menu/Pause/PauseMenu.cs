using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public GameObject panelPause;
    
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
	
	/// <summary>
	/// Load scene with its index
	/// </summary>
	/// <param name="sceneIndex"></param>
	public void LoadScene(int sceneIndex)
	{
		SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
	}

    public void Return()
    {
	    AudioManager.Instance.Play("Button");
        panelPause.transform.rotation *= Quaternion.Euler(0, 0, 180);
    }
}
