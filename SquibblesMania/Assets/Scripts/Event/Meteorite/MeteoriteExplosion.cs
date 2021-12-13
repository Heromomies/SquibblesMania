using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoriteExplosion : MonoBehaviour
{
    [Header("EVENT")]
    private List<GameObject> _cubeOnMap;
    private List<GameObject> _cubeTouched;

    private int _numberOfMeteorite;

    public Rigidbody bulletPrefab;
    public void OnClick()
    {
        _numberOfMeteorite = MapGenerator.Instance.numberOfMeteorite;
       
        #region MeteoriteRandomization

        _cubeOnMap = MapGenerator.Instance.cubeOnMap;

        for (int i = 0; i <= _numberOfMeteorite; i++)
        {
            int placeOfCube = Random.Range(0, 100);
            RandomEvent(placeOfCube);
        }

        #endregion
    }

    private void RandomEvent(int placeOfCube)
    {
        if (_cubeOnMap[placeOfCube].GetComponent<Renderer>().material.color != Color.black)
        {
            _cubeOnMap[placeOfCube].GetComponent<Renderer>().material.color = Color.black;
            _cubeTouched.Add(_cubeOnMap[placeOfCube]);
            MapGenerator.Instance.cubeOnMap.Remove(_cubeOnMap[placeOfCube]);
        }
        #region ExplosionFromTheCenter

        for (int i = 0; i < _cubeTouched.Capacity; i++)
        {
            Vector3 vo = CalculateVelocity(_cubeTouched[i].transform.position, transform.position, 2);
            transform.rotation = Quaternion.LookRotation(vo);
        
            Rigidbody obj = Instantiate(bulletPrefab, _cubeTouched[i].transform.position, Quaternion.identity);
            obj.velocity = vo;

            Destroy(obj, 10f);
        }
        #endregion
    }

    #region CalculateVelocity

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
    }   

    #endregion
}
