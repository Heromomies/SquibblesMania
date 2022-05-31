using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCamera : MonoBehaviour
{

    private bool cinematic;
    public GameObject cinematicbars;
    public GameObject CanvasUI;
    public Transform target;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("g"))
        {
            StartCoroutine(CameraMovement());
        }

        if(cinematic == true)
        {
            transform.LookAt(target);
            transform.Translate(Vector3.right * Time.deltaTime * 2);
            
        }
    }

    IEnumerator CameraMovement()
    {
        CanvasUI.SetActive(false);
        StartCoroutine(cinematicbars.GetComponent<CinematicBars>().ShowBar());
        cinematic = true;

        yield return new WaitForSeconds(7f);

        CanvasUI.SetActive(true);
        StartCoroutine(cinematicbars.GetComponent<CinematicBars>().HideBar());
        cinematic = false;

        transform.LookAt(CameraButtonManager.Instance.target);
    
    }
}
