using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerManager : MonoBehaviour
{
    public int maxDistance;

    private float _height;
    
    private void Update()
    {
        RaycastHit hit;
        
        Debug.DrawRay(transform.transform.position, Vector3.back * maxDistance, Color.yellow);
        
        if (Physics.Raycast(transform.transform.position, Vector3.back, out hit, maxDistance))
        {
            var position = hit.transform.position;
            _height = position.y;
        }
    }

    public void AttractPlayer()
    {
        var transformPosition = transform.position;
        transform.position += Vector3.back;
        transformPosition.y += transformPosition.y + _height;
    }
}
