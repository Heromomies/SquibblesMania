using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorMapWindow : EditorWindow
{
    private static GameObject lastObjectCreated;
    private static bool onMapEditor;
    private static bool isCreating;

    private static List<GameObject> currentObjectsCreated = new List<GameObject>();
    private static Vector2 planeMapSize;

    [MenuItem("Window/Editor Map/Custom Map Editor")]
    // Start is called before the first frame update
    public static void ShowWindow()
    {
        GetWindow<EditorMapWindow>("Map Editor");
        onMapEditor = true;

        SceneView.duringSceneGui -= UpdateSceneView;
        SceneView.duringSceneGui += UpdateSceneView;
    }


    private static Vector3 SnapOffset(Vector3 vector3, Vector3 offset, float gridSize = 1.0f)
    {
        Vector3 snapped = vector3 + offset;
        snapped = new Vector3(
            Mathf.Round(snapped.x / gridSize) * gridSize,
            Mathf.Round(snapped.y / gridSize + 1f) * gridSize,
            Mathf.Round(snapped.z / gridSize) * gridSize);
        return snapped - offset;
    }

    static void SpawnObjectOnPlane(UnityEngine.Event e)
    {
        // Shoot a ray from the mouse position into the world
        Ray worldRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(worldRay, out hitInfo, Mathf.Infinity))
        {
            if (hitInfo.collider.GetComponent<MeshCollider>())
            {
                GameObject spawnObj = Instantiate(lastObjectCreated);
                currentObjectsCreated.Add(spawnObj);
                Vector3 targetPos = SnapOffset(hitInfo.point,
                    new Vector3(spawnObj.transform.position.x + 0.5f, spawnObj.transform.position.y,
                        spawnObj.transform.position.z + 0.5f));
                spawnObj.transform.position = targetPos;
            }
        }
    }


    private static void UpdateSceneView(SceneView sceneView)
    {
        UnityEngine.Event e = UnityEngine.Event.current;


        if (e.type == EventType.MouseDown && e.button == 0 && onMapEditor)
        {
            SpawnObjectOnPlane(e);
        }

        UnityEngine.Event.current = null;
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnGUI()
    {
        if (!isCreating)
        {
            GUILayout.Label("MAP EDITOR TOOL", EditorStyles.boldLabel);
            GUILayout.Label("Create a plane on the scene to place objects, you can choose it's size");
            planeMapSize = EditorGUILayout.Vector2Field("Size map (Y axis correspond at Z axis)", planeMapSize);


            if (GUILayout.Button("Create plane"))
            {
                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.transform.position = Vector3.zero;
                plane.transform.localScale = new Vector3(planeMapSize.x, 1, planeMapSize.y);
                isCreating = true;
            }
        }


        if (isCreating)
        {
            GUILayout.Label("Left click on the scene to create a object", EditorStyles.largeLabel);
            lastObjectCreated =
                EditorGUILayout.ObjectField("Block", lastObjectCreated, typeof(GameObject), true) as GameObject;
            if (GUILayout.Button("Remove last object created"))
            {
                if (currentObjectsCreated.Count > 0)
                {
                    DestroyImmediate(currentObjectsCreated[currentObjectsCreated.Count - 1]);
                    currentObjectsCreated.Remove(currentObjectsCreated[currentObjectsCreated.Count - 1]);
                }
            }
        }
    }


    private void OnDestroy()
    {
        //Call when close the window
        isCreating = false;
        onMapEditor = false;
        lastObjectCreated = null;
    }
}