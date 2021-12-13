using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MeteoriteExplosion : MonoBehaviour
{
    [Header("EVENT")]
    private List<GameObject> _cubeOnMap;
    [SerializeField] private List<GameObject> cubeTouched;

    public int numberOfMeteorite;

    public Rigidbody bulletPrefab;
    public Transform volcanoTransform;
 
    public void OnClick()
    {
        #region MeteoriteRandomization

        _cubeOnMap = MapGenerator.Instance.cubeOnMap;

        while (numberOfMeteorite > 0)
        {
            numberOfMeteorite--;
            int placeOfCube = Random.Range(0, 100);
            RandomEvent(placeOfCube);
        }

        #endregion
    }

    private void RandomEvent(int placeOfCube)
    {
        #region ChangeColor

        if (_cubeOnMap[placeOfCube].GetComponent<Renderer>().material.color != Color.black)
        {
            _cubeOnMap[placeOfCube].GetComponent<Renderer>().material.color = Color.black;
           
            cubeTouched.Add(_cubeOnMap[placeOfCube]);
            
            MapGenerator.Instance.cubeOnMap.Remove(_cubeOnMap[placeOfCube]);

            InvokeRepeating(nameof(LaunchBullet), 0.2f, 0.5f);
        }

        #endregion
    }

    void LaunchBullet()
    {
        #region ExplosionFromTheCenter

        cubeTouched[0].tag = "Black Block";
        
        var positionVol = volcanoTransform.position;
        Vector3 vo = CalculateVelocity(cubeTouched[0].transform.position, positionVol, 2);
        transform.rotation = Quaternion.LookRotation(vo);

        cubeTouched.Remove(cubeTouched[0]);
        
        Rigidbody obj = Instantiate(bulletPrefab, positionVol, Quaternion.identity);
        obj.velocity = vo;

        #endregion
    }

    void Update()
    {
        if (cubeTouched.Count <= 0)
        {
            CancelInvoke();
        }
    }
    
    #region CalculateVelocity

    Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float speed) // Function to make a parabola
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

        float vxz = sxz / speed;
        
        ////calculating initial y velocity
        //Vy0 = y/t + 1/2 * g * t

        float vy = sy / speed + 0.5f * Mathf.Abs(Physics.gravity.y) * speed;
        Vector3 result = distanceXZ * vxz;
        result.y = vy;
        
        return result;
    }   

    #endregion
}
