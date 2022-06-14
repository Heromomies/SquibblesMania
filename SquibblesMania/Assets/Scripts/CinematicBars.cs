using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CinematicBars : MonoBehaviour
{
    // Start is called before the first frame update
    private RectTransform _topBar, _bottomBar;
    public TextMeshProUGUI mapTitle;

    public void Start()
    {
        _topBar = transform.GetChild(0).GetComponent<RectTransform>();
        _bottomBar = transform.GetChild(1).GetComponent<RectTransform>();
    }

    public IEnumerator ShowBar()
    {
        var t = 0;
        
        LeanTween.move(_topBar, new Vector3(0f, 645f, 0f), 0.2f);
        LeanTween.move(_bottomBar, new Vector3(0f, -645f, 0f), 0.2f);

        yield return new WaitForSeconds(0.5f);

        mapTitle.gameObject.SetActive(true);
        mapTitle.LeanAlphaTextMeshPro(1f, 1f).setFrom(0f);
        
        yield return null;
    }

    public IEnumerator HideBar()
    {
        LeanTween.move(_topBar, new Vector3(0f, 900f, 0f), 0.4f);
        LeanTween.move(_bottomBar, new Vector3(0f, -900f, 0f), 0.4f);

        mapTitle.LeanAlphaTextMeshPro(0f, 1f).setFrom(1f).setOnComplete(SetActiveFalseObject);

        yield return null;
    }

    private void SetActiveFalseObject() => mapTitle.gameObject.SetActive(false);
}
