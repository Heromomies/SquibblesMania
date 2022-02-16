using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestClickButtonLaunchEvent : MonoBehaviour
{
   public List<GameObject> eventToTest;
   public void OnClick()
   {
      foreach (var eventTested in eventToTest)
      {
         if (!eventTested.activeSelf)
         {
            Debug.Log("Je passe par la aussi ");
            eventTested.SetActive(true);
         }
         else
         {
            eventTested.SetActive(false);
         }
      }
   }
}
