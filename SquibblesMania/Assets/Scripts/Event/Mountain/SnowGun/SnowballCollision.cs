using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
        }
    }
}
