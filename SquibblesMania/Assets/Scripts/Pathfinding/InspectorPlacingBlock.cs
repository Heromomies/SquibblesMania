using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Node))]

public class InspectorPlacingBlock : Editor
{
   public override void OnInspectorGUI()
   {
      base.OnInspectorGUI();

      Node node = (Node)target;

      if (GUILayout.Button("Set Up PossiblePath"))
      {
         node.SetUpPossiblePath();
      }
   }
}
