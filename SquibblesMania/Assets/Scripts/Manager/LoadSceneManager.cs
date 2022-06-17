using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{

    private static LoadSceneManager _loadSceneManager;
    public static LoadSceneManager Instance => _loadSceneManager;

    [SerializeField]
    private CanvasGroup loadingScreen;

    [SerializeField] private float timerInSecondsAlphaBlend = 0.5f;
    void Awake()
    {
        if (_loadSceneManager == null)
        {
            _loadSceneManager = this;
            DontDestroyOnLoad(loadingScreen.transform.parent);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
    

    public void LoadScene(int sceneIndex)
    {
        loadingScreen.gameObject.SetActive(true);
        LeanTween.alphaCanvas(loadingScreen, 1, timerInSecondsAlphaBlend);
        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);
        StartCoroutine(SceneLoadingCoroutine(loadSceneAsync));
    }

    IEnumerator SceneLoadingCoroutine(AsyncOperation sceneAsync)
    {
        while (!sceneAsync.isDone)
        {
            yield return null;
        }
        LeanTween.alphaCanvas(loadingScreen, 0, timerInSecondsAlphaBlend).setOnComplete(SetLoadingScreenToFalse);
    }

   private void SetLoadingScreenToFalse() => loadingScreen.gameObject.SetActive(false);
}
