using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerShowPath : MonoBehaviour
{
   private void OnTriggerEnter(Collider other)
   {
      
      if (other.gameObject.GetComponent<PlayerStateManager>())
      {
         gameObject.SetActive(false);
      }
   }
}
