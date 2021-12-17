using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public float blockSizeOffset = 0.5f;
    
    public Transform previousBlock;
   
    public List<GamePath> possiblePath;
    
    [HideInInspector]
    public GroupBlockDetection groupBlockParent;
    
    // Start is called before the first frame update
    void Awake()
    {
        foreach (var path in possiblePath)
        {
            path.currentBlock = gameObject;
        }
        groupBlockParent = gameObject.transform.parent.GetComponent<GroupBlockDetection>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    public Vector3 GetWalkPoint()
    {
       
        //The position in the center of the block
        return transform.position + (transform.up * blockSizeOffset);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.grey;
        Gizmos.DrawSphere(GetWalkPoint(), 0.1f);
    }
    
}

[Serializable]
public class GamePath
{
    [HideInInspector]
    public GameObject currentBlock;
    public Transform nextPath;
    public bool isActive;
}