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
    public RectTransform panelLaunch;

    public GameObject uiMenuParent;

    [SerializeField] private float timeInSecondsLeanAlpha = 0.7f;
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
        AudioManager.Instance.Play("Button");

        panelLaunch.SetActive(false);
        
        startManager.SetActive(true);
        
    }
    public void Play()
    {
        AudioManager.Instance.Play("Button");
        startManager.SetActive(false);
        mapManager.SetActive(true);
    }

    public void MapPlay()
    {
        AudioManager.Instance.Play("Button");
        playerData.MapID = panelManager.GetComponent<PageSwiper>().currentPage;
        panelManager.transform.position = panelManager.GetComponent<PageSwiper>().panelLocation;
        mapManager.SetActive(false);
        characterManager.SetActive(true);
    }

    // Update is called once per frame
    public void QuitApp()
    {
        AudioManager.Instance.Play("Button");
        Application.Quit();
    }

    public void BackMap()
    {
        AudioManager.Instance.Play("Button");
        characterManager.SetActive(false);
        mapManager.SetActive(true);
    }

    public void BackMainMenu()
    {
        AudioManager.Instance.Play("Button");
        mapManager.SetActive(false);
        startManager.SetActive(true);
    }

    public void RotateStartMenuUi()
    {
        AudioManager.Instance.Play("Button");
        uiMenuParent.transform.rotation *= Quaternion.Euler(0, 0, 180);
    }
}
