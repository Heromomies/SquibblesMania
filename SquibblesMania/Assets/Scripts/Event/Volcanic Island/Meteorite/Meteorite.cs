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
         _rb.constraints = RigidbodyConstraints.FreezeAll;
         float localY = transform.localScale.y / 4;
         transform.position = new Vector3(other.transform.position.x, other.transform.localScale.y + localY, other.transform.position.z);
         
         transform.rotation = other.transform.rotation;
      }
   }
}
