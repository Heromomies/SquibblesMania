using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public GameObject panelPause;


    [Header("ANIM UI PARAMETERS")] [Range(1f, 3f)] [SerializeField]
    private float scaleAmount = 1.2f;

    [SerializeField] private float animScaleUITimeInSeconds = 0.3f;
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

    public void RotateUi()
    {
	    AudioManager.Instance.Play("Button");
        panelPause.transform.rotation *= Quaternion.Euler(0, 0, 180);
    }

    
    /// <summary>
    /// Anim to scale up/down ui gameObject
    /// </summary>
    /// <param name="uiGameObject"></param>
    public void AnimScaleUpUi(GameObject uiGameObject)
    {
	    LeanTween.scale(uiGameObject, Vector3.one * scaleAmount, animScaleUITimeInSeconds);
    }
    public void AnimScaleDownUi(GameObject uiGameObject)
    {
	    LeanTween.scale(uiGameObject, Vector3.one, animScaleUITimeInSeconds);
    }
}
