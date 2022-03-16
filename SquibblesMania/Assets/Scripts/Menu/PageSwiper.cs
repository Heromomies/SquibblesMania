using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PageSwiper : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private Vector3 panelLocation;
    public float percentThreshold = 0.2f;
    public float easing = 0.5f;
    public int totalPages;
    private int currentPage = 1;
    // Start is called before the first frame update
    void Start()
    {
        panelLocation = transform.position;
    }

    // Update is called once per frame
    public void OnDrag(PointerEventData data)
    {
        float difference = data.pressPosition.x - data.position.x;
        transform.position = panelLocation - new Vector3(difference, 0, 0);
    }
    public void OnEndDrag(PointerEventData data)
    {
        float percentage = (data.pressPosition.x - data.position.x) / Screen.width;
        if(Mathf.Abs(percentage) >= percentThreshold)
        {
            Vector3 newLocation = panelLocation;
            if(percentage > 0 && currentPage < totalPages)
            {
                newLocation += new Vector3(-Screen.width, 0, 0);
                currentPage++;
            }else if(percentage < 0 && currentPage > 1)
            {
                newLocation += new Vector3(Screen.width, 0, 0);
                currentPage--;
            }
            StartCoroutine(SmoothMove(transform.position, newLocation, easing));
            panelLocation = newLocation;
        }
        else
        {
            StartCoroutine(SmoothMove(transform.position, panelLocation, easing));
        }
    }

    IEnumerator SmoothMove(Vector3 startpos, Vector3 endpos, float seconds)
    {
        float t = 0f;
        while(t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            transform.position = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
    }

    public void NextPanel()
    {
        if (currentPage < totalPages)
        {
            Vector3 newLocation = panelLocation;
            newLocation += new Vector3(-Screen.width, 0, 0);
            currentPage++;
            StartCoroutine(SmoothMove(transform.position, newLocation, easing));
            panelLocation = newLocation;
        }
    }

    public void PreviousPanel()
    {
        if (currentPage > 1)
        {
            Vector3 newLocation = panelLocation;
            newLocation += new Vector3(Screen.width, 0, 0);
            currentPage--;
            StartCoroutine(SmoothMove(transform.position, newLocation, easing));
            panelLocation = newLocation;
        }
    }
}
