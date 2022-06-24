using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PageSwiper : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public Vector3 panelLocation;
    public float percentThreshold = 0.2f;
    public float easing = 0.5f;
    public int totalPages;
    public int currentPage = 1;
    public bool transitionf;

    public Menu menu;

    [SerializeField]
    private Transform fakeFirstPanel;
    [SerializeField]
    private Transform fakeLastPanel;

    
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
        if (Mathf.Abs(percentage) >= percentThreshold && menu.rotated == false)
        {
            Vector3 newLocation = panelLocation;
            if (percentage > 0 && currentPage < totalPages)
            {
                newLocation += new Vector3(-Screen.width, 0, 0);
                currentPage++;
            } else if (percentage > 0 && currentPage == totalPages)
            {
                int returnToFirst;
                returnToFirst = totalPages - 1;
                newLocation -= new Vector3(returnToFirst * -Screen.width, 0, 0);
                currentPage = 1;
                transform.position = fakeFirstPanel.position;
                StartCoroutine(SmoothMove(transform.position, newLocation, easing));

                panelLocation = newLocation;
            }

            if (percentage < 0 && currentPage > 1)
            {
                newLocation += new Vector3(Screen.width, 0, 0);
                currentPage--;
            } else if (percentage < 0 && currentPage == 1)
            {
                int returnToLast;
                returnToLast = totalPages - 1;
                newLocation += new Vector3(returnToLast * -Screen.width, 0, 0);
                currentPage = totalPages;
                transform.position = new Vector3(-4480, 512, 0);
                StartCoroutine(SmoothMove(transform.position, newLocation, easing));
                panelLocation = newLocation;
            }
            StartCoroutine(SmoothMove(transform.position, newLocation, easing));
            panelLocation = newLocation;
        }
        else if (menu.rotated == false)
        {
            StartCoroutine(SmoothMove(transform.position, panelLocation, easing));
        }


        if (Mathf.Abs(percentage) >= percentThreshold && menu.rotated == true)
        {
            Vector3 newLocation = panelLocation;
            if (percentage < 0 && currentPage < totalPages)
            {
                newLocation += new Vector3(Screen.width, 0, 0);
                currentPage++;
            }

            

            else if (percentage < 0 && currentPage == totalPages)
            {
                int returnToFirst;
                returnToFirst = totalPages - 1;
                newLocation -= new Vector3(returnToFirst * Screen.width, 0, 0);
                currentPage = 1;
                transform.position = fakeFirstPanel.position;
                StartCoroutine(SmoothMove(transform.position, newLocation, easing));

                panelLocation = newLocation;
            }


            if (percentage > 0 && currentPage > 1)
            {
                newLocation += new Vector3(-Screen.width, 0, 0);
                currentPage--;

            }

            else if (percentage > 0 && currentPage == 1)
            {
                Debug.Log("test");
                     int returnToLast;
                     returnToLast = totalPages - 1;
                     newLocation += new Vector3(returnToLast * Screen.width, 0, 0);
                     currentPage = totalPages;
                     transform.position = new Vector3(4480, 512, 0) + new Vector3(Screen.width, 0, 0);
                     StartCoroutine(SmoothMove(transform.position, newLocation, easing));
                     panelLocation = newLocation;


            }
                StartCoroutine(SmoothMove(transform.position, newLocation, easing));
            panelLocation = newLocation;
        }
        else if(menu.rotated == true)
        {
            StartCoroutine(SmoothMove(transform.position, panelLocation, easing));
        }
    }

    IEnumerator SmoothMove(Vector3 startpos, Vector3 endpos, float seconds)
    {
        menu.blockRotate = true;
        float t = 0f;
        
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            transform.position = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
            
            yield return null;
        }

        menu.blockRotate = false;

        
    }

    public void NextPanel()
    {
        AudioManager.Instance.Play("UI_Button_Other");
        if (currentPage < totalPages && menu.rotated == false)
        {
            Vector3 newLocation = panelLocation;
            newLocation += new Vector3(-Screen.width, 0, 0);
            currentPage++;
            StartCoroutine(SmoothMove(transform.position, newLocation, easing));
            panelLocation = newLocation;
        }
        else if (menu.rotated == false)
        {
            int returnToFirst;
            returnToFirst = totalPages - 1;
            Vector3 newLocation = panelLocation;
            newLocation -= new Vector3(returnToFirst * -Screen.width, 0, 0);
            currentPage = 1;
            transform.position = fakeFirstPanel.position;
            StartCoroutine(SmoothMove(transform.position, newLocation, easing));
            panelLocation = newLocation;
            
        }
        

        if (currentPage < totalPages && menu.rotated == true)
        {
            Vector3 newLocation = panelLocation;
            newLocation += new Vector3(Screen.width, 0, 0);
            currentPage++;
            StartCoroutine(SmoothMove(transform.position, newLocation, easing));
            panelLocation = newLocation;
           
        }
        else if (menu.rotated == true)
        {
            int returnToFirst;
            returnToFirst = totalPages - 1;
            Vector3 newLocation = panelLocation;
            newLocation -= new Vector3(returnToFirst * Screen.width, 0, 0);
            currentPage = 1;
            transform.position = fakeFirstPanel.position;
            StartCoroutine(SmoothMove(transform.position, newLocation, easing));
            panelLocation = newLocation;
        }
    }

    public void PreviousPanel()
    {
        AudioManager.Instance.Play("UI_Button_Other");
        if (currentPage > 1 && menu.rotated == false)
        {
            Vector3 newLocation = panelLocation;
            newLocation += new Vector3(Screen.width, 0, 0);
            currentPage--;
            StartCoroutine(SmoothMove(transform.position, newLocation, easing));
            panelLocation = newLocation;
        }
        else if(menu.rotated == false)
        {
            int returnToLast;
            returnToLast = totalPages - 1;
            Vector3 newLocation = panelLocation;
            newLocation += new Vector3(returnToLast * -Screen.width, 0, 0);
            currentPage = totalPages;
            transform.position = new Vector3(-4480, 512, 0);
            StartCoroutine(SmoothMove(transform.position, newLocation, easing));
            panelLocation = newLocation;
        }



        if (currentPage > 1 && menu.rotated == true)
        {
            Vector3 newLocation = panelLocation;
            newLocation += new Vector3(-Screen.width, 0, 0);
            currentPage--;
            StartCoroutine(SmoothMove(transform.position, newLocation, easing));
            panelLocation = newLocation;
            
        }
        else if (menu.rotated == true)
        {
            int returnToLast;
            returnToLast = totalPages - 1;
            Vector3 newLocation = panelLocation;
            newLocation += new Vector3(returnToLast * Screen.width, 0, 0);
            currentPage = totalPages;
            transform.position = new Vector3(4480, 512, 0) + new Vector3(Screen.width, 0, 0);
            StartCoroutine(SmoothMove(transform.position, newLocation, easing));
            panelLocation = newLocation;
        }
    }
}
