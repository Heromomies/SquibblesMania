using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class RotateCaméra : MonoBehaviour
{
    public bool canRotate;
    public Transform anchorPoint;

    public float rotationAmount = 90f;

    //public float damping = 2f;

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
            canRotate = false;
            anchorPoint.localEulerAngles = new Vector3(0, anchorPoint.localEulerAngles.y + rotationAmount, 0);
        }
    }

    public void RotateCamLeftOnstarted()
    {
        canRotate = true;
        if (canRotate)
        {
            canRotate = false;
            anchorPoint.localEulerAngles = new Vector3(0, anchorPoint.localEulerAngles.y - rotationAmount, 0);
        }
    }
}