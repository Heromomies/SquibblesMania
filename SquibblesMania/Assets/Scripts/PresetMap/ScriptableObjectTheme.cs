using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Material Theme Tool", menuName = "Scriptable Object/Material Editor Tool")]
public class ScriptableObjectTheme : ScriptableObject
{

    public List<Material> materials = new List<Material>();
    public GameObject mainThemeObject;
}


