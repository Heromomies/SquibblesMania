using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CinematicBars : MonoBehaviour
{
    // Start is called before the first frame update
    private RectTransform topBar, bottomBar;
    public Text mapTitle;

    Color32 startColor = new Color(255, 255, 255, 0);
    Color32 endColor = new Color(255, 255, 255, 255);


    public void Start()
    {
        topBar = transform.GetChild(0).GetComponent<RectTransform>();
        bottomBar = transform.GetChild(1).GetComponent<RectTransform>();
    }

    public IEnumerator ShowBar()
    {
        float t = 0;
        
        LeanTween.move(topBar, new Vector3(0f, 645f, 0f), 0.2f);
        LeanTween.move(bottomBar, new Vector3(0f, -645f, 0f), 0.2f);

        yield return new WaitForSeconds(0.5f);

        mapTitle.color = startColor;

        while (mapTitle.color.a < 1.0f)
        {

            mapTitle.color = new Color(255, 255, 255, 0 + (Time.deltaTime / t));
        }

        mapTitle.canvasRenderer.SetAlpha(0);
        mapTitle.CrossFadeAlpha(1.0f, 2.0f, false);

        yield return null;
    }

    public IEnumerator HideBar()
    {
        float t = 0;

        LeanTween.move(topBar, new Vector3(0f, 900f, 0f), 0.4f);
        LeanTween.move(bottomBar, new Vector3(0f, -900f, 0f), 0.4f);

        mapTitle.color = endColor;

        mapTitle.canvasRenderer.SetAlpha(0);
        mapTitle.CrossFadeAlpha(0f, 1.5f, false);



        yield return null;
    }
}
