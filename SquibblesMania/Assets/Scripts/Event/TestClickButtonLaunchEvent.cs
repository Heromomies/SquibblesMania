using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestClickButtonLaunchEvent : MonoBehaviour
{
   public List<GameObject> eventToTest;

   private void Start()
   {
      InvokeRepeating("OnClicked", 0, 1.1f);
   }

   public void OnClicked()
   {
      Debug.Log("Je passe par là");
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
