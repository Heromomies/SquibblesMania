using UnityEngine;

public class DetectionSnowGun : MonoBehaviour
{
   public SnowGun snowGun;
   
   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         snowGun.canClick = true;
      }
   }
}
