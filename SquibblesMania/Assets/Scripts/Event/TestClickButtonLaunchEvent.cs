using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestClickButtonLaunchEvent : MonoBehaviour
{
   public List<GameObject> eventToTest;
   public GameObject powerToTest;
   private void Update()
   {
      if (Input.GetKeyDown(KeyCode.E))
      {
         powerToTest.SetActive(true);
      }
   }

   public void OnClicked()
   {
      foreach (var eventTested in eventToTest)
      {
         if (!eventTested.activeInHierarchy)
         {
            eventTested.SetActive(true);
         }
         else
         {
            eventTested.SetActive(false);
         }
      }
   }
}
