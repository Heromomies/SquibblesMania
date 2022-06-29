using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [Header("LAUNCH GAME")] public GameObject startManager;
    [SerializeField] private GameObject[] launchGameObjects;
    
    public GameObject mapManager;
    public GameObject characterManager;
    public GameObject creditsManager;
    
    [SerializeField]
    private PageSwiper panelManager;
    [SerializeField] private Button buttonRotateUi;
    public PlayerData playerData;

    public TextMeshProUGUI textToPulse;
    public RectTransform panelLaunch;

    public GameObject uiMenuParent;
    public Image imageMenuTitle;

    public bool rotated = false;
    public GameObject swipeManager;
    public Vector3 firstPanelPosition;

    //[HideInInspector]
    public bool blockRotate = false;
    
    
    [SerializeField] private float titleScaleDownTime = 0.3f;
    
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
        panelLaunch.gameObject.SetActive(false);
        startManager.SetActive(true);
        foreach (var launchGameObject in launchGameObjects) launchGameObject.SetActive(true);

    }

    public void Play()
    {
        AudioManager.Instance.Play("Button");
        startManager.SetActive(false);
        mapManager.SetActive(true);
        LeanTween.scale(imageMenuTitle.gameObject, Vector3.zero, titleScaleDownTime);
    }

    public void MapPlay()
    {
        AudioManager.Instance.Play("Button");
        playerData.MapID = panelManager.currentPage;
        panelManager.transform.position = panelManager.panelLocation;
        mapManager.SetActive(false);
        characterManager.SetActive(true);
        buttonRotateUi.gameObject.SetActive(false);
        uiMenuParent.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void ActiveCredits()
    {
        AudioManager.Instance.Play("Button");
        LeanTween.scale(imageMenuTitle.gameObject, Vector3.zero, titleScaleDownTime);
        creditsManager.SetActive(true);
    }
 
    public void QuitApp()
    {
        AudioManager.Instance.Play("Button");
        Application.Quit();
    }

    public void BackMap()
    {
        AudioManager.Instance.Play("Button");
        characterManager.SetActive(false);
        buttonRotateUi.gameObject.SetActive(true);
        mapManager.SetActive(true);
    }

    public void BackMainMenu()
    {
        AudioManager.Instance.Play("Button");
        mapManager.SetActive(false);
        startManager.SetActive(true);
        creditsManager.SetActive(false);
        LeanTween.scale(imageMenuTitle.gameObject, Vector3.one, titleScaleDownTime);
    }

    public void RotateStartMenuUi()
    {
        AudioManager.Instance.Play("Button");

        if(blockRotate == false)
        {
            uiMenuParent.transform.rotation *= Quaternion.Euler(0, 0, 180);
            firstPanelPosition = new Vector3(640, 512, 0);
            swipeManager.transform.position = firstPanelPosition;
            panelManager.currentPage = 1;
            panelManager.panelLocation = firstPanelPosition;
        }
        

        if(rotated == false)
        {
            rotated = true;
        }
        else
        {
            rotated = false;
        }
        
    }

}
