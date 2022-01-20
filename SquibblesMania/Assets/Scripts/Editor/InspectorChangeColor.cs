using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GroupBlockDetection))]
public class InspectorChangeColor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		GroupBlockDetection groupBlockDetection = (GroupBlockDetection)target;
      
		if (GUILayout.Button("Set Up Blocs Color"))
		{
			groupBlockDetection.SetUpBlocsColor();
		}
	}
}
