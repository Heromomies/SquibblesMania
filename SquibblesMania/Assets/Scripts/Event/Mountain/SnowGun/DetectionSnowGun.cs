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
      }
   }

   private void OnTriggerExit(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         snowGun.canClick = false;
         snowGun.animatorSnowGun.SetBool("onHatche", false);
      }
   }
}
