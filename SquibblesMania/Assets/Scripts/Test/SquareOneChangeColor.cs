using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Wizama.Hardware.Antenna;
using Wizama.Hardware.Light;

public class SquareOneChangeColor : MonoBehaviour
{
    void Start()
    {
        var lightIndex = LIGHT_INDEX.LIGHT_1;
        lightIndex = (LIGHT_INDEX) LIGHT_COLOR.COLOR_RED;
    }
}

