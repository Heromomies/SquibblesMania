using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [Serializable]
    public class Pool
    {
        public string poolTag;
        public GameObject prefab;
        public int poolSize;
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionnary = new Dictionary<string, Queue<GameObject>>();

    private static PoolManager poolManager;
    public static PoolManager Instance => poolManager;


    private void Awake()
    {
        poolManager = this;
    }

    void Start()
    {
        InitializePools();
    }

    void InitializePools()
    {
        //Initialize the different pools of our list

        foreach (var pool in pools)
        {
            //Create a queue for each pool
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.poolSize; i++)
            {
                GameObject prefabObject = Instantiate(pool.prefab);
                prefabObject.SetActive(false);
                objectPool.Enqueue(prefabObject);
            }
            
            poolDictionnary.Add(pool.poolTag, objectPool);
        }
    }

    public GameObject SpawnObjectFromPool(string tag, Vector3 position, Quaternion rotation, Transform parent)
    {
        //If we dont have a pool with tag we return 
        if (!poolDictionnary.ContainsKey(tag))
        {
            Debug.LogError($"Pool with tag {tag} doesn't exist !");
            return null;
        }
        //Spawn object from pool using our dictionnary
        GameObject objectToSpawn =  poolDictionnary[tag].Dequeue();
        if (objectToSpawn != null)
        {
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            objectToSpawn.transform.parent = parent;
            
            //Add back to the queue so we can use the object later
            poolDictionnary[tag].Enqueue(objectToSpawn);
        }

        return objectToSpawn;
    }
  

    // Update is called once per frame
    void Update()
    {
        
    }
}
