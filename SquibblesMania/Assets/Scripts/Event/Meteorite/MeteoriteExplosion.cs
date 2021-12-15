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
    public Transform secondCam;
    
    [Range(0.0f, 3.0f)] public float speed;
    [Range(0.0f, 1.0f)] public float repeatRate;
    
    public Transform cube;
    public float durationOfScale;
    public float duration = 4;

    IEnumerator Start()
    {
        // Start after one second delay (to ignore Unity hiccups when activating Play mode in Editor)
        yield return new WaitForSeconds(0.5f);

        // Create a new Sequence.
        // We will set it so that the whole duration is 6
        Sequence s = DOTween.Sequence();
        // Add an horizontal relative move tween that will last the whole Sequence's duration
        s.Append(cube.DOScaleY(-0.5f, durationOfScale).SetRelative().SetEase(Ease.InOutQuad));
        // Insert a rotation tween which will last half the duration
        // and will loop forward and backward twice
        //s.Insert(0, cube.DORotate(new Vector3(0, 45, 0), duration / 2).SetEase(Ease.InQuad).SetLoops(2, LoopType.Yoyo));
        // Add a color tween that will start at half the duration and last until the end
        //s.Insert(duration / 2, cube.GetComponent<Renderer>().material.DOColor(Color.yellow, duration / 2));
        // Set the whole Sequence to loop infinitely forward and backwards
        //s.SetLoops(-1, LoopType.Yoyo);
    }
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
        InvokeRepeating(nameof(LaunchBullet), 0.2f, repeatRate);
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
        }

        #endregion
    }

    void LaunchBullet()
    {
        #region ExplosionFromTheCenter

        //Camera.main.DOShakePosition(1, 1f, 100, 0);
        
        cubeTouched[0].tag = "Black Block";
        
        var positionVol = volcanoTransform.position;
        Vector3 vo = CalculateVelocity(cubeTouched[0].transform.position, positionVol, speed);
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
            Camera.main.transform.position = secondCam.position;
        }
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
