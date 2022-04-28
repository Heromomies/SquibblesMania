using System;
using UnityEngine;

public class DetectionSnowGun : MonoBehaviour
{
   public SnowGun snowGun;
   
   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         snowGun.canClick = true;
         snowGun.animatorSnowGun.SetBool("onHatche", true);
      }
   }

   private void OnTriggerExit(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         snowGun.canClick = false;
         snowGun.animatorSnowGun.SetBool("onHatche", false);
      }
   }
}
