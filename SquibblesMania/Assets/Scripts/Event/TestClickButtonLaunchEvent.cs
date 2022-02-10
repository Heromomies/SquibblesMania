using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestClickButtonLaunchEvent : MonoBehaviour
{
   public GameObject eventToTest;
   public void OnClick()
   {
      if (!eventToTest.activeSelf)
      {
         eventToTest.SetActive(true);
      }
      else
      {
         eventToTest.SetActive(false);
      }
     
   }
}
