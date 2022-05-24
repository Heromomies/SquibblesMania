using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    private Transform _target;
    public float speed;

    private bool _canTurn;
    private static float waitForSecondStartRotate = 0.2f;
    private WaitForSeconds _waitForSecondsRotate = new WaitForSeconds(waitForSecondStartRotate);
    private IEnumerator Start()
    {
        yield return _waitForSecondsRotate;
        _target = CameraButtonManager.Instance.target;
        _canTurn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(_canTurn)
         transform.RotateAround(_target.position, Vector3.up, speed * Time.deltaTime);
    }
}
