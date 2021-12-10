using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody bulletPrefab;
    public Transform transformPoint;
    
    
    // Start is called before the first frame update
    void Update()
    {
        LaunchProjectile();
    }

    void LaunchProjectile()
    {
        Vector3 vo = CalculateVelocity(transformPoint.position, transform.position, 2);
        transform.rotation = Quaternion.LookRotation(vo);

        if (Input.GetMouseButtonDown(0))
        {
            Rigidbody obj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            obj.velocity = vo;
        }
    }
    
    Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float time) // Calculate the velocity of the bullet for the parabola
    {
        // Define the distance of x and y first

        Vector3 distance = target - origin;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0f;
        
        // Create a float that represent our distance

        float sDistanceY = distance.y;
        float sDistanceXZ = distanceXZ.magnitude;

        float velocityXZ = sDistanceXZ / time;
        float velocityY = sDistanceY / time * 0.5f * Mathf.Abs(Physics.gravity.y) * time;

        Vector3 result = distanceXZ.normalized;
        result *= velocityXZ;
        result.y = velocityY;
        
        return result;
    }
}
