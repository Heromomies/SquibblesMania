using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestClickButtonLaunchEvent : MonoBehaviour
{
   public GameObject eventToTest;
   public void OnClick()
   {
      eventToTest.SetActive(true);
   }
}
