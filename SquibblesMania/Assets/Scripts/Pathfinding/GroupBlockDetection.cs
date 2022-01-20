using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GroupBlockDetection : MonoBehaviour
{
    public List<Transform> playersOnGroupBlock;
    
    public void SetUpBlocsColor()
    {
        Node.ColorBloc randomColor = (Node.ColorBloc)Random.Range(0, 4);
        for (int i = 0; i < GetComponentsInChildren<Node>().Length ; i++)
        {
            GetComponentsInChildren<Node>()[i].colorBloc = randomColor;
            GetComponentsInChildren<Renderer>()[i].material = GetComponentInParent<MapColor>().materials[(int)randomColor];
        }
            
    }
}