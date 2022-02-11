using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class RotateCaméra : MonoBehaviour
{
    [SerializeField] private Transform cam;

    [SerializeField] private GameObject interactionParentObject, buttonParentUpDown;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (interactionParentObject.activeSelf || buttonParentUpDown.activeSelf)
        {
            transform.LookAt(transform.position + cam.forward);
        }
        
    }

   
}