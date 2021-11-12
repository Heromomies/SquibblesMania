using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class RotateCaméra : MonoBehaviour
{
    [SerializeField] private bool canRotate;
    [SerializeField] private Transform anchorPoint;
    [SerializeField] private float rotationAmount = 90f;
    [SerializeField] private float rotateTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void RotateCamRight()
    {
        canRotate = true;
        if (canRotate)
        {
            anchorPoint.transform.DORotate(new Vector3(0, anchorPoint.transform.eulerAngles.y + rotationAmount, 0),
                rotateTime);
            canRotate = false;
        }
    }


    public void RotateCamLeft()
    {
        canRotate = true;
        if (canRotate)
        {
            canRotate = false;

            anchorPoint.transform.DORotate(new Vector3(0, anchorPoint.transform.eulerAngles.y - rotationAmount, 0),
                rotateTime);
            canRotate = false;
        }
    }
}