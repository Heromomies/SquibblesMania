using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CinematicBars : MonoBehaviour
{
    [SerializeField]
    private RectTransform topBar, bottomBar;
    public TextMeshProUGUI mapTitle;


    private static float waitTimerShowBar = 0.5f;
    private WaitForSeconds _waitForSecondsShowBar = new WaitForSeconds(waitTimerShowBar);
   

    public IEnumerator ShowBar()
    {
        LeanTween.move(topBar, new Vector3(0f, 645f, 0f), 0.2f);
        LeanTween.move(bottomBar, new Vector3(0f, -645f, 0f), 0.2f);

        yield return _waitForSecondsShowBar;

        mapTitle.gameObject.SetActive(true);
        mapTitle.LeanAlphaTextMeshPro(1f, 1f).setFrom(0f);
        
        yield return null;
    }

    public IEnumerator HideBar()
    {
        LeanTween.move(topBar, new Vector3(0f, 900f, 0f), 0.4f);
        LeanTween.move(bottomBar, new Vector3(0f, -900f, 0f), 0.4f);

        mapTitle.LeanAlphaTextMeshPro(0f, 1f).setFrom(1f).setOnComplete(SetActiveFalseObject);

        yield return null;
    }

    private void SetActiveFalseObject() => mapTitle.gameObject.SetActive(false);
}
