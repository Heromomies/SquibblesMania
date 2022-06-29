using System;
using System.Collections;
using UnityEngine;

public class DetectionSnowGun : MonoBehaviour
{
   [Header("SETTINGS")]
   public SnowGun snowGun;
   public LayerMask layer;
   
   [Header("MATERIALS")]
   public Material matToChange;
   
   [HideInInspector] public Animator animator;
   [HideInInspector] public Collider[] players;
   private GameObject _playerOne;
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
         
         // ReSharper disable once Unity.PreferNonAllocApi
         players = Physics.OverlapSphere(transform.position, Mathf.Infinity, layer);
         _playerOne = GameManager.Instance.currentPlayerTurn.gameObject;
         
         for (int i = 0; i < players.Length; i++)
         {
            if (players[i].name != _playerOne.name && players.Length > 1)
            {
               players[i].GetComponent<PlayerStateManager>().playerMesh.GetComponent<Renderer>().material = matToChange;
            }
         }
         
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
         snowGun.animatorSnowGun.SetBool("onHatche", false);
         OnAntennaRemove();
      }
   }

   public void RemoveAntenna()
   {
      Debug.Log("here");
      animator.SetBool("isTrigger", false);
      snowGun.animatorSnowGun.SetBool("onHatche", false);
   }
   
   public void OnAntennaRemove()
   {
      for (int i = 0; i < players.Length; i++)
      {
         players[i].TryGetComponent(out PlayerStateManager playerStateManager);
         GameManager.Instance.SetUpPlayerMaterial(playerStateManager, playerStateManager.playerNumber);
      }
      snowGun.canClick = false;

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
