using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoutainManager : MonoBehaviour
{
    public GameObject hatchDetectPlayer;
    public List<Transform> transformHatches;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var hatch in transformHatches)
        {
            var hatchPossiblePath = hatch.GetComponent<GamePath>().nextPath;
            
            for (int i = 0; i <  hatch.GetComponent<Node>().possiblePath.Count; i++)
            {
                Instantiate(hatchDetectPlayer, hatchPossiblePath.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            }
        }
    }
}
