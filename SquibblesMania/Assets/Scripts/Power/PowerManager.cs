using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerManager : MonoBehaviour
{
    public int maxDistance;

    private float _height;
    private bool _canTouch;
    private void Update()
    {
        RaycastHit hit;
        
        Debug.DrawRay(transform.position, Vector3.back * maxDistance, Color.yellow);
        Debug.DrawRay(transform.position, Vector3.forward * maxDistance, Color.yellow);
        Debug.DrawRay(transform.position, Vector3.left * maxDistance, Color.yellow);
        Debug.DrawRay(transform.position, Vector3.right * maxDistance, Color.yellow);
        
        if (Physics.Raycast(transform.position, Vector3.back, out hit, maxDistance) && !_canTouch)
        {
            _canTouch = true;
            hit.transform.position += Vector3.back;
            Debug.Log("I'm touching a player from the back");
        }
        if (Physics.Raycast(transform.position, Vector3.forward, out hit, maxDistance) && !_canTouch)
        {
            _canTouch = true;
            hit.transform.position += Vector3.forward;
            Debug.Log("I'm touching a player from forward");
        }
        if (Physics.Raycast(transform.position, Vector3.left, out hit, maxDistance) && !_canTouch)
        {
            _canTouch = true;
            hit.transform.position += Vector3.left;
            Debug.Log("I'm touching a player from the left");
        }
        if (Physics.Raycast(transform.position, Vector3.right, out hit, maxDistance) && !_canTouch)
        {
            _canTouch = true;
            hit.transform.position += Vector3.right;
            Debug.Log("I'm touching a player from the right");
        }
    }

    public void AttractPlayer()
    {
        var transformPosition = transform.position;
        transform.position += Vector3.back;
        transformPosition.y += transformPosition.y + _height;
    }
}
