using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    private Transform _target;
    public float speed;
    public int numberOfFrames;

    private void Start()
    {
        _target = CameraButtonManager.Instance.target;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount % numberOfFrames == 0)
        {
            transform.RotateAround(_target.position, Vector3.up, speed * Time.deltaTime);
        }
    }
}
