using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGeneratorManager : MonoBehaviour
{
    
    [SerializeField]
    private List<GameObject> mapPrefabsList;

    [SerializeField] private Vector3 mapSpawnPos = new Vector3(6, 0, -3.5f);

    [SerializeField] private int endZoneSpawnPointsCount = 5;

    private string _blocParentTag = "BlockParent";

    private static MapGeneratorManager _mapGeneratorManager;


    public static MapGeneratorManager Instance => _mapGeneratorManager;


    private void Awake()
    {
        _mapGeneratorManager = this;
    }


    public void SetupMap()
    {
        var indexMap = Random.Range(0, mapPrefabsList.Count);
        var map = Instantiate(mapPrefabsList[indexMap], mapSpawnPos, Quaternion.identity);
        SetUpBlocParents(map);
    }

    /// <summary>
    /// Set up of the bloc parents for the game manager
    /// </summary>
    /// <param name="map"></param>
    private void SetUpBlocParents(GameObject map)
    {
        foreach (Transform blocParent in map.transform)
        {
            if (blocParent.CompareTag(_blocParentTag))
            {
                if (!GameManager.Instance.allBlocParents.Contains(blocParent.gameObject))
                    GameManager.Instance.allBlocParents.Add(blocParent.gameObject);
            }

            SetUpBlocChild(blocParent.gameObject);
        }
        SetUpEndZonePossibleSpawn(GameManager.Instance.allBlocParents);
    }

    private void SetUpEndZonePossibleSpawn(List<GameObject> allBlocParents)
    {

        foreach (GameObject blocParent in allBlocParents)
        {
            foreach (Transform blocChild in blocParent.transform)
            {
                if (blocChild.TryGetComponent(out Node blocChildNode))
                {
                    if (blocChildNode.isSpawnPoint)
                    {
                        return;
                    }
                    var randomIndex = Random.Range(0, 11);
                    
                    if (randomIndex >= 4)
                    {
                        if (GameManager.Instance.conditionVictory.endZoneSpawnPoints.Count < endZoneSpawnPointsCount)
                            GameManager.Instance.conditionVictory.endZoneSpawnPoints.Add(blocParent.transform);

                    }
                }
                
            }
            
           
        }
    }
/// <summary>
/// Set up all the bloc child in the gameManager list
/// </summary>
/// <param name="blocParent"></param>
    private void SetUpBlocChild(GameObject blocParent)
    {
        foreach (Transform blocChild in blocParent.transform)
        {
            if (!GameManager.Instance.cleanList.Contains(blocChild.gameObject))
            {
                GameManager.Instance.cleanList.Add(blocChild.gameObject);
            }

            SetUpPlayerSpawnPoints(blocChild);
        }
    }
/// <summary>
/// Set up the spawn point od the players
/// </summary>
/// <param name="blocChild"></param>
    private void SetUpPlayerSpawnPoints(Transform blocChild)
    {
        if (!blocChild.gameObject.TryGetComponent(out Node blocChildNode)) return;
        if (!blocChildNode.isSpawnPoint) return;
        if (!GameManager.Instance.playersSpawnPoints.Contains(blocChild))
            GameManager.Instance.playersSpawnPoints.Add(blocChild);
    }
}
