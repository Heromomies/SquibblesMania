using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class RotateCaméra : MonoBehaviour
{
    [SerializeField] private Transform cam;

    [SerializeField] private GameObject interactionParentObject;
  
    // Update is called once per frame
    void LateUpdate()
    {
        if (interactionParentObject.activeSelf)
        {
            transform.LookAt(transform.position + cam.forward);
        }
        
    }

   
}