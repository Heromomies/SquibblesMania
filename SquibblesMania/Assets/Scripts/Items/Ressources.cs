using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ressources : Item
{
    [Serializable]
    public enum Types
    {
        Wood,
        Rock,
        Rope
    };

    public Types ressourcesTypes;

    public int ressourcesCount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
