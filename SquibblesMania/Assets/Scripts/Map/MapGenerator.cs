using System;
using System.Collections;
using System.Collections.Generic;
using DigitalRubyShared;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    private Transform[,] gridMap;
    [SerializeField] private Transform cubePrefab;


    [SerializeField] private Vector2Int mapSize;
    [SerializeField] private float cubeSize;
    [SerializeField] private int minYScale, maxYScale;

    [SerializeField] private Transform ground;

    [SerializeField] private Material[] cubeMaterials;

    [Header("EVENT")]
    public List<GameObject> cubeOnMap;

    [SerializeField] private int numberOfMeteorite;
    [SerializeField] private int placeOfMeteoriteX;
    [SerializeField] private int placeOfMeteoriteY;

    private static MapGenerator mapGenerator;

    public static MapGenerator Instance => mapGenerator;
    // Start is called before the first frame update

    private void Awake()
    {
        mapGenerator = this;
        FingersScript.Instance.ShowTouches = false;
    }

    void Start()
    {
        GenerateMap();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void GenerateMap()
    {
        //Création du parent mapHolder object
        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        gridMap = new Transform[mapSize.x, mapSize.y];

        ground.localScale = new Vector3(mapSize.x, ground.localScale.y, mapSize.y);

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Transform cube = Instantiate(cubePrefab);
                
                cubeOnMap.Add(cube.gameObject);

                cube.localScale = GetRandomScale(cube);
                cube.position = new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y) +
                                Vector3.up * cube.localScale.y / 2;
                int randomMaterial = Random.Range(0, 3 + 1);
                cube.GetComponent<Renderer>().material = GetRandomMaterial(randomMaterial);
                gridMap[x, y] = cube;
                cube.parent = mapHolder;
            }
        }

        /*for (int i = 1; i <= numberOfMeteorite; i++)
        {
            int placeOfCube = Random.Range(placeOfMeteoriteX, placeOfMeteoriteY);
            RandomEvent(placeOfCube);
        }*/
        CompactEvent();
    }

    public void RandomEvent(int i)
    {
        if(cubeOnMap[i].GetComponent<Renderer>().material.color != Color.black)
            cubeOnMap[i].GetComponent<Renderer>().material.color = Color.black;
        else
        {
            Debug.Log("This block was already black");
        }
    }

    public void CompactEvent()
    {
        for (int i = 1; i <= numberOfMeteorite; i++)
        {
            cubeOnMap[i].GetComponent<Renderer>().material.color = Color.black;
        }
    }
    Vector3Int GetRandomScale(Transform gameObj)
    {
        return new Vector3Int((int)gameObj.localScale.x, Random.Range(minYScale, maxYScale + 1),
            (int)gameObj.localScale.z);
    }

    Material GetRandomMaterial(int numberRandom)
    {
        return cubeMaterials[numberRandom];
    }

    public Transform GetPlatformFromPosition(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / cubeSize + (mapSize.x - 1) / 2f);
        int y = Mathf.RoundToInt(position.z / cubeSize + (mapSize.y - 1) / 2f);
        x = Mathf.Clamp(x, 0, gridMap.GetLength(0) - 1);
        y = Mathf.Clamp(y, 0, gridMap.GetLength(1) - 1);
        return gridMap[x, y];
    }
}