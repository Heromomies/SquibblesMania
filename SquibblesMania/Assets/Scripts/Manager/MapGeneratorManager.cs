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
    private string _noneTag = "Untagged";
    private static MapGeneratorManager _mapGeneratorManager;


    public static MapGeneratorManager Instance => _mapGeneratorManager;


    [SerializeField] private int minNumberBlocParentChildEndZone = 3;
    private void Awake()
    {
        _mapGeneratorManager = this;
    }


    public void SetupMap()
    {
        var indexMap = Random.Range(0, mapPrefabsList.Count);
        var map = Instantiate(mapPrefabsList[indexMap], mapSpawnPos, Quaternion.identity);
        SetUpBlocParents(map);
        if (CameraButtonManager.Instance != null) CameraButtonManager.Instance.target = map.transform;
        if (GameManager.Instance != null) GameManager.Instance.cameraViewModeGesture.mapTarget = map.transform;
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

            if (!blocParent.CompareTag(_noneTag))
            {
                SetUpBlocChild(blocParent.gameObject);
            }
        }
        SetUpEndZonePossibleSpawn(GameManager.Instance.allBlocParents);
    }

    private void SetUpEndZonePossibleSpawn(List<GameObject> allBlocParents)
    {

        foreach (GameObject blocParent in allBlocParents)
        {
            bool isHasBlocChildSpawnPoint = false;
            for (int i = 0; i < blocParent.transform.childCount; i++)
            {
                if (blocParent.transform.GetChild(i).TryGetComponent(out Node blocChildNode))
                {
                    if (blocChildNode.isSpawnPoint)
                    {
                        i = blocParent.transform.childCount;
                        isHasBlocChildSpawnPoint = true;
                        break;
                    }
                }
               
            }

            if (!isHasBlocChildSpawnPoint)
            {
                if (Random.value <= 0.7)// 70% chance rate
                {
                    if (GameManager.Instance.conditionVictory.endZoneSpawnPoints.Count < endZoneSpawnPointsCount)
                    {
                        if (!GameManager.Instance.conditionVictory.endZoneSpawnPoints.Contains(blocParent.transform) && blocParent.transform.childCount >= minNumberBlocParentChildEndZone)
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
