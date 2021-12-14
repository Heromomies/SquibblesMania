using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wizama;
using Wizama.Hardware.Light;

public class SquareOneChangeColor : MonoBehaviour
{
    public LIGHT_COLOR id;
    public LIGHT_INDEX li;
    
    public void OnClick()
    {
        LightController.ColorizeOne(li, id, true);
    }

   /* public static void ColorizeOne(LIGHT_INDEX lightIndexes, LIGHT_COLOR lightColors, bool keepOthersColorized = true)
    {
        Debug.Log(lightIndexes);
        Debug.Log(lightColors);
    }*/
}
