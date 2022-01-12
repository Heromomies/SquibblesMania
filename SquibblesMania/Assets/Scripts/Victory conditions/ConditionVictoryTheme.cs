using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition Victory Theme", menuName = "Scriptable Object/Victory Conditions")]
public class ConditionVictoryTheme : ScriptableObject
{
    public ConditionVictory conditionVictory;
}
[Serializable]
public class ConditionVictory
{
    public enum Theme
    {
        Volcano,
        Mountain,
        Submarine,
        Temple
    };

    public Theme mapTheme;
    
    public int objectCount;
    public GameObject[] items;
    public GameObject endZone;
    public Transform[] endZoneSpawnPoints;
    
    public Transform[] fireCheckPoints;
}