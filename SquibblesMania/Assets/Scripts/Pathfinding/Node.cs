using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node : MonoBehaviour
{
    public float blockSizeOffset = 0.5f;
    public Transform previousBlock;
    public bool isActive = true;

    public List<GamePath> possiblePath = new List<GamePath>();

    [HideInInspector] public GroupBlockDetection groupBlockParent;

    private float radius = 1f;
    public enum ColorBloc
    {
        Red = 0,
        Yellow = 1,
        Blue = 2,
        Green = 3,
        None = 4
    }

    public ColorBloc colorBloc;
    // Start is called before the first frame update
    void Awake()
    {
      
        groupBlockParent = gameObject.transform.parent.GetComponent<GroupBlockDetection>();
        isActive = true;
    }

    public Vector3 GetWalkPoint()
    {
        //The position in the center of the block
        return transform.position + (transform.up * blockSizeOffset);
    }
    

    
    public void SetUpPossiblePath()
    {
        radius = 1f;
        
        
        
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, radius);
        possiblePath.Clear();
        int count = 0;
        foreach (var collider in colliders)
        {
            if (Vector3.Distance(collider.transform.position, transform.position) <= (radius + 0.1f) &&
                collider.transform != transform && collider.gameObject.GetComponent<Node>())
            {
                //Create a new element and set possiblePath value
                possiblePath.Add(new GamePath());
                possiblePath[count].nextPath = collider.transform;
                possiblePath[count].isActive = true;
                count++;
            }
        }
    }
    
}

[Serializable]
public class GamePath
{
    public Transform nextPath;
    public bool isActive;
}