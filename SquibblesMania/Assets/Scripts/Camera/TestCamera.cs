using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCamera : MonoBehaviour
{
    public GameObject target;
    private bool cinematic;
    public GameObject cinematicbars;
    // Start is called before the first frame update
    void Start()
    {
  
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("g"))
        {
            StartCoroutine(CameraMovement());
        }

        if(cinematic == true)
        {
            transform.LookAt(target.transform);
            transform.Translate(Vector3.right * Time.deltaTime * 2);
        }
    }

    IEnumerator CameraMovement()
    {
        StartCoroutine(cinematicbars.GetComponent<CinematicBars>().ShowBar());
        cinematic = true;

        yield return new WaitForSeconds(5f);

        StartCoroutine(cinematicbars.GetComponent<CinematicBars>().HideBar());
        cinematic = false;
    }
}
