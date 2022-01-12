using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node : MonoBehaviour
{
    public float blockSizeOffset = 0.5f;
    public Transform previousBlock;
    public bool isActive;

    public List<GamePath> possiblePath;

    [HideInInspector] public GroupBlockDetection groupBlockParent;

    public float radius = 1f;

    // Start is called before the first frame update
    void Awake()
    {
        foreach (var path in possiblePath)
        {
            path.currentBlock = gameObject;
        }

        groupBlockParent = gameObject.transform.parent.GetComponent<GroupBlockDetection>();
        isActive = true;
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

    public void SetUpPossiblePath()
    {
        radius = 1f;
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, radius);

        int count = 0;
        foreach (var collider in colliders)
        {
            
            if (Vector3.Distance(collider.transform.position, transform.position) <= radius && collider.transform != transform)
            {
                //Create a new element and set possiblePath value
                possiblePath.Add(new GamePath());
                possiblePath[count].nextPath = collider.transform;
                possiblePath[count].isActive = true;
                count++;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.grey;
        Gizmos.DrawSphere(transform.position, radius);
    }
}

[Serializable]
public class GamePath
{
    [HideInInspector] public GameObject currentBlock;
    public Transform nextPath;
    public bool isActive;
}