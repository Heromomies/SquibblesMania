using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SnowGun : MonoBehaviour, IManageEvent
{
    public Rigidbody bulletPrefab;

    [Range(0.0f, 3.0f)] public float speed;
    public GameObject hatchDetectPlayerNearSnowGun;

    public GameObject snowGun;
    
    private void Start()
    {
        ShowEvent();
    }

    public void ShowEvent()
    {
        var hatchPossiblePath = GetComponentInParent<Node>().possiblePath;
        
        for (int i = 0; i <  hatchPossiblePath.Count; i++)
        {
            Instantiate(hatchDetectPlayerNearSnowGun, hatchPossiblePath[i].nextPath.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        }
    }

    public void LaunchEvent() // Check the distance between each players launch a bullet to the nearest player
    {
        Vector3 vo = CalculateVelocity(transform.position, transform.position,
                speed); // Add the velocity to make an effect of parabola for the bullets
        transform.rotation = Quaternion.LookRotation(vo);

        Rigidbody obj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        obj.velocity = vo;
    }
    
    #region CalculateVelocity

    Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float velocity) // Function to make a parabola
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

        float vxz = sxz / velocity;

        ////calculating initial y velocity
        //Vy0 = y/t + 1/2 * g * t

        float vy = sy / velocity + 0.6f * Mathf.Abs(Physics.gravity.y + 0.7f) * velocity;
        Vector3 result = distanceXZ * vxz;
        result.y = vy;

        return result;
    }

    #endregion
}
