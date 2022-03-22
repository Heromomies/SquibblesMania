using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData" , menuName = "Scriptable Object/Player Data")]
public class PlayerData : ScriptableObject
{
    // Start is called before the first frame update
    public int P1colorID;
    public int P1hatID;

    public int P2colorID;
    public int P2hatID;

    public int P3colorID;
    public int P3hatID;

    public int P4colorID;
    public int P4hatID;

    public int MapID;
}
