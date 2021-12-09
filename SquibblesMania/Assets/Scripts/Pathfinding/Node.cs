using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public float blockSizeOffset = 0.5f;

    public Transform previousBlock;
    public List<GamePath> possiblePath;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    public Vector3 GetWalkPoint()
    {
        //On récupère la position central du block
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
    //
    public Transform nextPath;
    public bool isActive;
}