using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteorite : MonoBehaviour
{
   private Rigidbody _rb;
   public GameObject particleSystem;
   private void Start()
   {
      _rb = GetComponent<Rigidbody>();
   }

   private void OnCollisionEnter(Collision other)
   {
      if (other.gameObject.CompareTag("Black Block"))
      {
         Instantiate(particleSystem, transform.position, Quaternion.identity);
         _rb.constraints = RigidbodyConstraints.FreezeAll;
         transform.position = new Vector3(other.transform.position.x, transform.position.y , other.transform.position.z);
         
         transform.rotation = other.transform.rotation;
      }
   }
}
