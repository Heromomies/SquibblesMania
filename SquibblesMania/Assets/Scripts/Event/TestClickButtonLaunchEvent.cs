using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestClickButtonLaunchEvent : MonoBehaviour
{
   public List<GameObject> eventToTest;
  
   #region Singleton

   private static TestClickButtonLaunchEvent clickButton;

   public static TestClickButtonLaunchEvent Instance => clickButton;

   // Start is called before the first frame update

   private void Awake()
   {
      clickButton = this;
   }

   #endregion

   public void LaunchEvent()
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

   public void LaunchMeteoriteOnPlayer()
   {
      var secondPlayer = GameManager.Instance.players[1];
      var posSp = secondPlayer.transform.position;
      PoolManager.Instance.SpawnObjectFromPool("Meteorite", new Vector3(posSp.x, posSp.y + 3f, posSp.z), Quaternion.identity, null);
   }
   
   private void Update()
   {
      if (Input.GetKeyDown(KeyCode.E))
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
   
}
