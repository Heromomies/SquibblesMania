using System;
using UnityEngine;

public class DetectionSnowGun : MonoBehaviour
{
   public SnowGun snowGun;

   [HideInInspector] public Animator animator;

   private void Start()
   {
      animator = GetComponent<Animator>();
   }

   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         snowGun.canClick = true;
         animator.SetBool("isTrigger", true);
         snowGun.animatorSnowGun.SetBool("onHatche", true);
         
         snowGun.shootPlayerTxt.SetActive(true);
         snowGun.goToAntennaTxt.SetActive(false);
      }
   }

   private void OnTriggerExit(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         snowGun.canClick = false;
         animator.SetBool("isTrigger", false);
         snowGun.animatorSnowGun.SetBool("onHatche", false);
         
         snowGun.shootPlayerTxt.SetActive(false);
         snowGun.goToAntennaTxt.SetActive(true);
      }
   }
}
