using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PulsingBloc
{
    //Pulsing for a list of bloc
    public static void PulsingEmissiveColorSquareBlocList(Color baseSquareColorBloc, Color colorTo, List<Transform> blocList, float length)
    {
        foreach (var bloc in blocList)
        {
            Material blocSquareMat = bloc.GetComponent<Renderer>().materials[2]; 
            blocSquareMat.SetColor("_EmissionColor", Color.Lerp(colorTo, baseSquareColorBloc, Mathf.PingPong(Time.time, length))); 
        }
    }
    
    //Pulsing for one bloc
    public static void PulsingEmissiveColorSquareBloc(Color baseSquareColorBloc, Color colorTo, Transform bloc, float length)
    {
        Material blocSquareMat = bloc.GetComponent<Renderer>().materials[2];
        blocSquareMat.SetColor("_EmissionColor", Color.Lerp(colorTo, baseSquareColorBloc, Mathf.PingPong(Time.time, length))); 
    }
    
}
