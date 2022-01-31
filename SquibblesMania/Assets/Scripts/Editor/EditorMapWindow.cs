using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorMapWindow : EditorWindow
{
    private static GameObject lastObjectCreated, currentObjectSelected;
    private static bool onMapEditor;
    private static bool isCreating;

    [MenuItem("Window/Editor Map/Custom Map Editor")]
    // Start is called before the first frame update
    public static void ShowWindow()
    {
        GetWindow<EditorMapWindow>("Map Editor");
        onMapEditor = true;

        SceneView.duringSceneGui -= UpdateSceneView;
        SceneView.duringSceneGui += UpdateSceneView;
    }

    private static void UpdateSceneView(SceneView sceneView)
    {
        if (isCreating)
        {
            if (UnityEngine.Event.current.type == EventType.MouseDown && UnityEngine.Event.current.button == 0)
            {
                Vector3 pointsPos =
                    HandleUtility.GUIPointToWorldRay(UnityEngine.Event.current.mousePosition).direction * 1;

                //Todo create object here at pointsPos
                Instantiate(lastObjectCreated, pointsPos, Quaternion.identity);

                // Avoid the current event being propagated
                // I'm not sure which of both works better here
                UnityEngine.Event.current = null;


                // exit creation mode
                isCreating = false;
            }
        }
        else
        {
            UnityEngine.Event.current = null;
        }
    }

    private void OnInspectorUpdate()
    {
        if (onMapEditor && lastObjectCreated)
        {
            isCreating = true;
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("MAP EDITOR TOOL", EditorStyles.boldLabel);
        GUILayout.Label("Left click on the scene to create a object", EditorStyles.largeLabel);
        lastObjectCreated =
            EditorGUILayout.ObjectField("Select the object to create", lastObjectCreated, typeof(GameObject), true) as
                GameObject;
    }


    private void OnDestroy()
    {
        //Call when close the window
        isCreating = false;
        onMapEditor = false;
        lastObjectCreated = null;
    }
}