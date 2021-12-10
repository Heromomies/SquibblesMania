using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoriteExplosion : MonoBehaviour
{
    [Header("EVENT")]
    private List<GameObject> _cubeOnMap;

    private int _numberOfMeteorite;

    public GameObject meteorite;
    public Transform topOfTheVolcano;
    public void OnClick()
    {
        _numberOfMeteorite = MapGenerator.Instance.numberOfMeteorite;
        #region ExplosionFromTheCenter

        for (int i = 0; i < _numberOfMeteorite; i++)
        {
            GameObject m = Instantiate(meteorite, topOfTheVolcano.position, Quaternion.identity);
            m.GetComponent<Rigidbody>().AddForce(transform.up * 10);
        }

        #endregion


        /*#region MeteoriteRandomization

        _cubeOnMap = MapGenerator.Instance.cubeOnMap;

        for (int i = 0; i <= _numberOfMeteorite; i++)
        {
            int placeOfCube = Random.Range(0, 100);
            RandomEvent(placeOfCube);
        }

        #endregion*/
    }

    private void RandomEvent(int i)
    {
        if (_cubeOnMap[i].GetComponent<Renderer>().material.color != Color.black)
        {
            _cubeOnMap[i].GetComponent<Renderer>().material.color = Color.black;
            MapGenerator.Instance.cubeOnMap.Remove(_cubeOnMap[i]);
        }
    }
}
