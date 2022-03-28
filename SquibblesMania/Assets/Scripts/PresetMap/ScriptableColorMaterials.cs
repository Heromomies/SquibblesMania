using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Material Color Bloc", menuName = "Scriptable Object/Material Color Bloc Tool")]
public class ScriptableColorMaterials : ScriptableObject
{
    public List<Material> colorMaterials = new List<Material>();
}
