using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class RotateCaméra : MonoBehaviour
{
    public bool canRotate;
    public Transform anchorPoint;

    public float rotationAmount = 90f;

    public float rotateTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void RotateCamRightOncanceled()
    {
        canRotate = true;
        if (canRotate)
        {
            anchorPoint.transform.DORotate(new Vector3(0, anchorPoint.transform.eulerAngles.y + rotationAmount, 0),
                rotateTime);
            canRotate = false;
        }
    }


    public void RotateCamLeftOnstarted()
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