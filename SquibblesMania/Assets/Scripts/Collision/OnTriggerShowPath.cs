using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerShowPath : MonoBehaviour
{
   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         PlayerMovementManager.Instance.sphereList.Remove(gameObject);
         gameObject.SetActive(false);
      }
   }
}
