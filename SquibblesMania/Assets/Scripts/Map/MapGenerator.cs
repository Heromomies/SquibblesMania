using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    private Transform[,] gridMap;
    [SerializeField] private Transform cubePrefab;
    

    [SerializeField] private Vector2Int mapSize;
    [SerializeField] private float cubeSize;
    [SerializeField] private int minYScale, maxYScale;

    [SerializeField] private Transform ground;

    [SerializeField] private Material[] cubeMaterials;
    // Start is called before the first frame update
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
                Vector3 cubePosition = new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y) * cubeSize;
                Transform cube = Instantiate(cubePrefab, cubePosition, cubePrefab.rotation);
                cube.localScale = GetRandomScale(cube);
                
                int randomMaterial = Random.Range(0, 3 + 1);
                cube.GetComponent<Renderer>().material = GetRandomMaterial(randomMaterial);
              
                gridMap[x, y] = cube;
                cube.parent = mapHolder;
            }
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
}