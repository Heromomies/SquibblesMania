using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestClickButtonLaunchEvent : MonoBehaviour
{
   public List<GameObject> eventToTest;

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
