using System;
using System.Collections;
using UnityEngine;

public class DetectionSnowGun : MonoBehaviour
{
   public SnowGun snowGun;

   [HideInInspector] public Animator animator;

   private static float _waitTimeBeforeSliderFalse = 0.5f;
   private WaitForSeconds _waitForSecondsTime = new WaitForSeconds(_waitTimeBeforeSliderFalse);
   
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

         StartCoroutine(WaitBeforeDeactivateSlider());
         
         snowGun.shootPlayerTxt.SetActive(true);
         snowGun.goToAntennaTxt.SetActive(false);
      }
   }

   IEnumerator WaitBeforeDeactivateSlider()
   {
      yield return _waitForSecondsTime;
    
      UiManager.Instance.sliderNextTurn.interactable = false;
   }
   
   private void OnTriggerExit(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         OnAntennaRemove();
      }
   }

   public void OnAntennaRemove()
   {
      snowGun.canClick = false;
      animator.SetBool("isTrigger", false);
      snowGun.animatorSnowGun.SetBool("onHatche", false);
         
      AudioManager.Instance.Play("CanonOnOff");
        
      UiManager.Instance.sliderNextTurn.interactable = true;

      snowGun.shootPlayerTxt.SetActive(false);
      
      if (snowGun.activated)
      {
         snowGun.goToAntennaTxt.SetActive(false);
         snowGun.activated = false;
      }
      else
      {
         snowGun.goToAntennaTxt.SetActive(true);
      }
     
   }
   
}
