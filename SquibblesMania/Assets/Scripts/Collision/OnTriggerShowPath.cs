using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerShowPath : MonoBehaviour
{
   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Ghost"))
      {
         //GameManager.Instance.currentPlayerTurn.playerActionPoint+=2;
         PlayerMovementManager.Instance.UpdateActionPointTextPopUp(PlayerMovementManager.Instance.totalCurrentActionPoint+=2);
         UiManager.Instance.SetUpCurrentActionPointOfCurrentPlayer(GameManager.Instance.currentPlayerTurn.playerActionPoint);
         PlayerMovementManager.Instance.sphereList.Remove(gameObject);
         gameObject.SetActive(false);
      }
   }
}
