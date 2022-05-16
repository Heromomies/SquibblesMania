using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    private Transform _target;
    public float speed;

    private bool _canTurn;
    
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.2f);
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
