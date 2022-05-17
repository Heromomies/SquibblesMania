using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject startManager;
    public GameObject mapManager;
    public GameObject characterManager;

    public GameObject panelManager;
    public PlayerData playerData;

    public TextMeshProUGUI textToPulse;
    public GameObject panelLaunch;

    public GameObject uiMenuParent;
    // Start is called before the first frame update

    void Start()
    {
        textToPulse.LeanAlphaTextMeshPro(0f, 1f).setFrom(1f).setLoopPingPong();

        startManager.SetActive(false);
        mapManager.SetActive(false);
        characterManager.SetActive(false);
    }

    public void LaunchGame()
    {
        startManager.SetActive(true);
        panelLaunch.SetActive(false);
    }
    
    public void Play()
    {
        startManager.SetActive(false);
        mapManager.SetActive(true);
    }

    public void MapPlay()
    {
        
        playerData.MapID = panelManager.GetComponent<PageSwiper>().currentPage;
        panelManager.transform.position = panelManager.GetComponent<PageSwiper>().panelLocation;
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

    public void RotateStartMenuUi()
    {
        AudioManager.Instance.Play("Button");
        uiMenuParent.transform.rotation *= Quaternion.Euler(0, 0, 180);
    }
}
