using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerManager : MonoBehaviour
{
    public GameObject player;
    public int maxDistance;

    private float _height;
    
    private void Update()
    {
        RaycastHit hit;
        
        Debug.DrawRay(player.transform.position, Vector3.back * maxDistance, Color.yellow);
        
        if (Physics.Raycast(player.transform.position, Vector3.back, out hit, maxDistance))
        {
            var position = hit.transform.position;
            Debug.Log(position.y);
            _height = position.y;
        }
    }

    public void AttractPlayer()
    {
        var transformPosition = player.transform.position;
        transformPosition.y += transformPosition.y + _height; 
        player.transform.position += Vector3.back;
    }
}
