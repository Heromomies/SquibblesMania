using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject startManager;
    public GameObject mapManager;
    public GameObject characterManager;

    public GameObject panelManager;
    public PlayerData playerData;


    // Start is called before the first frame update

    void Start()
    {
        startManager.SetActive(true);
        mapManager.SetActive(false);
        characterManager.SetActive(false);
    }
    public void Play()
    {
        startManager.SetActive(false);
        mapManager.SetActive(true);
    }

    public void MapPlay()
    {
        playerData.MapID = panelManager.GetComponent<PageSwiper>().currentPage;
        mapManager.SetActive(false);
        characterManager.SetActive(true);
    }

    // Update is called once per frame
    public void QuitApp()
    {
        Application.Quit();
    }

    public void BackMap()
    {
        characterManager.SetActive(false);
        mapManager.SetActive(true);
    }

    public void BackMainMenu()
    {
        mapManager.SetActive(false);
        startManager.SetActive(true);
    }
}
