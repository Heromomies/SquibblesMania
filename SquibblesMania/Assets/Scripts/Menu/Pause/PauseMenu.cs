using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

 
    [Header("ANIM UI PARAMETERS")] [Range(1f, 3f)] [SerializeField]
    private float scaleAmount = 1.2f;

    [SerializeField] private float animScaleUITimeInSeconds = 0.3f;

    private Vector3 _panelPauseAnchorPos;
    [Header("ANIM UI PAUSE PANEL")]
    public RectTransform panelPause;
	[SerializeField]
    private LeanTweenType easeTypePausePanel;

    [SerializeField] private float animPanelPauseMoveUITimeInSeconds = 0.3f;
    
    public Image imageMenuTitle;
    
    [SerializeField] private float titleScaleDownTime = 0.3f;
    [SerializeField] private float titleScaleUpTime = 0.6f;

    private Menu _menu;
    
    private void Start()
    {
	    _menu = GetComponent<Menu>();
	    _panelPauseAnchorPos = panelPause.anchoredPosition;
    }

    public void Pause()
	{
		if (!panelPause.gameObject.activeSelf)
		{
			AnimMovePanelPauseXUi(panelPause,0);
			if (GameManager.Instance)
			{
				foreach (var winGameObject in GameManager.Instance.winConditonsList) winGameObject.SetActive(false);
				    
				if (GameManager.Instance.volume.profile.TryGet(out DepthOfField depthOfField))
				{
					depthOfField.active = true;
				}

			}

			if (imageMenuTitle != null)
			{
				LeanTween.scale(imageMenuTitle.gameObject, Vector3.zero, titleScaleDownTime);
			}
			
			SetPanelPauseStateTrue();
		}
		else
		{
			AnimMovePanelPauseXUi(panelPause, _panelPauseAnchorPos.x);
			if (GameManager.Instance)
			{
				foreach (var winGameObject in GameManager.Instance.winConditonsList) winGameObject.SetActive(true);
				if (GameManager.Instance.volume.profile.TryGet(out DepthOfField depthOfField))
				{
					depthOfField.active = false;
				}
			}

			if (_menu != null)
			{
				if (imageMenuTitle != null && !_menu.characterManager.activeSelf && !_menu.mapManager.activeSelf && !_menu.creditsManager.activeSelf)
				{
					LeanTween.scale(imageMenuTitle.gameObject, Vector3.one, titleScaleUpTime);	
				}
			}
			else
			{
				if (imageMenuTitle != null)
				{
					LeanTween.scale(imageMenuTitle.gameObject, Vector3.one, titleScaleUpTime);	
				}
			}
		}
		AudioManager.Instance.Play("Button");
	}
	
	/// <summary>
	/// Load scene with its index
	/// </summary>
	/// <param name="sceneIndex"></param>
	public void LoadScene(int sceneIndex)
	{
		LoadSceneManager.Instance.LoadScene(sceneIndex);
	}

    public void RotateUi()
    {
	    AudioManager.Instance.Play("Button");
	    
        panelPause.transform.rotation *= Quaternion.Euler(0, 0, 180);
    }

    private void AnimMovePanelPauseXUi(RectTransform uiGameObject, float xValue)
    {
	    if (!panelPause.gameObject.activeSelf)
	    {
		    LeanTween.moveX(uiGameObject, xValue, animPanelPauseMoveUITimeInSeconds).setEase(easeTypePausePanel);
	    }
	    else
	    {
		    LeanTween.moveX(uiGameObject, xValue, animPanelPauseMoveUITimeInSeconds).setEase(easeTypePausePanel).setOnComplete(SetPanelPauseStateFalse);
	    }
	   
    }
	private void SetPanelPauseStateTrue() => panelPause.gameObject.SetActive(true);
    private void SetPanelPauseStateFalse() => panelPause.gameObject.SetActive(false);
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
    public void PlayButtonSound()
    {
	    AudioManager.Instance.Play("Button");
    }
}
