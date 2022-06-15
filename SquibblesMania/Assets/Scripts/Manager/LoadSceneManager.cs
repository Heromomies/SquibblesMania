using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{

    private static LoadSceneManager _loadSceneManager;
    public static LoadSceneManager Instance => _loadSceneManager;

    [SerializeField]
    private GameObject loadingScreen;
    void Awake()
    {
        _loadSceneManager = this;
    }

    public void LoadScene(int sceneIndex)
    {
        loadingScreen.SetActive(true);
        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);
        StartCoroutine(SceneLoadingCoroutine(loadSceneAsync));
    }

    IEnumerator SceneLoadingCoroutine(AsyncOperation sceneAsync)
    {
        while (!sceneAsync.isDone)
        {
            yield return null;
        }
        loadingScreen.SetActive(false);
    }
}
