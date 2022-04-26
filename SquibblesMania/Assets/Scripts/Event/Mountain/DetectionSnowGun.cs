using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionSnowGun : MonoBehaviour
{
   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         //TODO Select player and launch a bullet
      }
   }
}
