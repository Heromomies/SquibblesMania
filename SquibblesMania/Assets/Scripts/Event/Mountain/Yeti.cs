using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

public class Yeti : MonoBehaviour, IManageEvent
{
    public Rigidbody bulletPrefab;

    [Range(0.0f, 3.0f)] public float speed;

    public TextMeshProUGUI textPrevention;
    
    public void ShowEvent()
    {
        textPrevention.text ="The yeti is going to shoot the nearest player !";
    }

    public void LaunchEvent() // Check the distance between each players launch a bullet to the nearest player
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (PlayerStateManager potentialTarget in GameManager.Instance.players)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget.transform;
            }
        }

        if (bestTarget != null)
        {
            Vector3 vo = CalculateVelocity(bestTarget.position, transform.position,
                speed); // Add the velocity to make an effect of parabola for the bullets
            transform.rotation = Quaternion.LookRotation(vo);

            Rigidbody obj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            obj.velocity = vo;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        //TODO Make the bullet stuns the player touched
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

        float vy = sy / velocity + 0.6f * Mathf.Abs(Physics.gravity.y) * velocity;
        Vector3 result = distanceXZ * vxz;
        result.y = vy;

        return result;
    }

    #endregion

  
}
