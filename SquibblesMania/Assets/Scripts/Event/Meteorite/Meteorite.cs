using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteorite : MonoBehaviour
{
   private Rigidbody _rb;

   private void Start()
   {
      _rb = GetComponent<Rigidbody>();
   }

   private void OnCollisionEnter(Collision other)
   {
      if (other.gameObject.CompareTag("Black Block"))
      {
         Debug.Log("I'm colliding with the blocks");
         _rb.constraints = RigidbodyConstraints.FreezeAll;
      }
   }
}
