using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
  /*  public Rigidbody bulletPrefab;
    public Transform transformPoint;

    public void OnClick()
    {
        Vector3 vo = CalculateVelocity(transformPoint.position, transform.position, 2);
        transform.rotation = Quaternion.LookRotation(vo);
        
        Rigidbody obj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        obj.velocity = vo;

        Destroy(obj, 10f);
    }
    
    Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float time)
    {
        //define the distance x and y first
        Vector3 distance = target - origin;
        Vector3 distanceXZ = distance;
        distanceXZ.Normalize();
        distanceXZ.y = 0;

        //creating a float that represents our distance 
        float sy = distance.y;
        float sxz = distance.magnitude;
        
        //calculating initial x velocity
        //Vx = x / t

        float vxz = sxz / time;
        
        ////calculating initial y velocity
        //Vy0 = y/t + 1/2 * g * t

        float vy = sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;
        Vector3 result = distanceXZ * vxz;
        result.y = vy;
        
        return result;
    }  */ 
}
