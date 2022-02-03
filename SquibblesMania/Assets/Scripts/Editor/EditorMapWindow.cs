using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum Colors
{
    Red,
    Blue,
    Yellow,
    Green
};

public class EditorMapWindow : EditorWindow
{
    private static GameObject lastObjectCreated, currentObjectSelected;
    private static bool onMapEditor;
    private static bool isCreating;
    private static bool isBlockSelected;
    private static List<GameObject> currentObjectsCreated = new List<GameObject>();
    private static List<GameObject> allObjectsCreatedOnScene = new List<GameObject>();
    private static Vector2 planeMapSize;

    private static Colors colors;
    private static Node currentBlockNode;
    private static GameObject planeGo;

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
            if (hitInfo.collider.GetComponent<MeshCollider>() && lastObjectCreated != null)
            {
                GameObject spawnObj = Instantiate(lastObjectCreated);
                currentObjectsCreated.Add(spawnObj);
                allObjectsCreatedOnScene.Add(spawnObj);
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

        if (Selection.activeGameObject && isCreating && !isBlockSelected)
        {
            if (Selection.activeGameObject.GetComponent<BoxCollider>())
            {
                currentObjectSelected = Selection.activeGameObject;
                currentBlockNode = Selection.activeGameObject.GetComponent<Node>();
                isBlockSelected = true;
            }

            Selection.selectionChanged -= SelectionChanged;
            Selection.selectionChanged += SelectionChanged;
        }

        UnityEngine.Event.current = null;
    }

    private static void SelectionChanged()
    {
        isBlockSelected = false;
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    #region GUI

    private void CreatePlane()
    {
        GUILayout.Label("MAP EDITOR TOOL", EditorStyles.boldLabel);
        EditorGUILayout.Space(20);
        GUILayout.Label("Create a plane on the scene to place objects, you can choose it's size");
        EditorGUILayout.Space(10);
        planeMapSize = EditorGUILayout.Vector2Field("Size map (Y axis correspond at Z axis)", planeMapSize);
        EditorGUILayout.Space(20);
        if (GUILayout.Button("Create plane") && planeMapSize.x >= 1 && planeMapSize.y >= 1)
        {
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.position = Vector3.zero;
            plane.transform.localScale = new Vector3(planeMapSize.x, 1, planeMapSize.y);
            planeGo = plane;
            allObjectsCreatedOnScene.Add(plane);
            isCreating = true;
        }
    }

    private void CreateObject()
    {
        EditorGUILayout.Space(20);
        GUILayout.Label("Left click on the scene to create a object", EditorStyles.largeLabel);
        lastObjectCreated =
            EditorGUILayout.ObjectField("Block", lastObjectCreated, typeof(GameObject), true) as GameObject;
        EditorGUILayout.Space(20);
        if (currentObjectsCreated.Count > 0)
        {
            if (GUILayout.Button("Remove last object created"))
            {
                DestroyImmediate(currentObjectsCreated[currentObjectsCreated.Count - 1]);
                currentObjectsCreated.Remove(currentObjectsCreated[currentObjectsCreated.Count - 1]);
            }
        }
    }

    private void ColorizeObject()
    {
        EditorGUILayout.Space(20);
        GUILayout.Label("Choose a color for the block bellow");
        colors = (Colors)EditorGUILayout.EnumPopup("Choose a color", colors);
        EditorGUILayout.Space(20);
        if (GUILayout.Button("Colorize"))
        {
            ChangeBlockColor();
        }
    }

    #endregion

   

    private void OnGUI()
    {
        if (!isCreating)
        {
            CreatePlane();
        }
        else
        {
            CreateObject();
        }


        if (isBlockSelected)
        {
            ColorizeObject();
        }

        if (isCreating && currentObjectsCreated.Count >= 1)
        {
            EditorGUILayout.Space(20);
            if (GUILayout.Button("Create map"))
            {
                CreateMap();
            }
        }

        EditorGUILayout.Space(20);
        if (GUILayout.Button("Reset"))
        {
            foreach (var obj in allObjectsCreatedOnScene)
            {
                DestroyImmediate(obj);
            }

            allObjectsCreatedOnScene.Clear();
            OnDestroy();
            ShowWindow();
        }
    }

    private void CreateMap()
    {
        var parents = new List<GameObject>();

        foreach (var currentObj in currentObjectsCreated.ToList())
        {
            var rays = new Ray[4];
            var raycastHits = new RaycastHit[4];
            var vectorRaycast = new List<Vector3> { Vector3.back, Vector3.forward, Vector3.right, Vector3.left };
            var listGoForParent = new List<GameObject>();

            for (int i = 0; i < rays.Length; i++)
            {
                rays[i] = InitializeRay(rays[i], currentObj.transform.position, vectorRaycast[i]);

                if (Physics.Raycast(rays[i], out raycastHits[i], 1.2f))
                {
                    HitReturn(raycastHits[i], currentObj, listGoForParent);
                }
            }

            if (!listGoForParent.Contains(currentObj))
            {
                listGoForParent.Add(currentObj);
            }

            currentObjectsCreated.Remove(currentObj);

            parents.Add(ParentingNeighboorObjects(listGoForParent));
        }

        foreach (var parent in parents)
        {
            if (parent.transform.childCount <= 0)
            {
                DestroyImmediate(parent);
            }
        }


        if (currentObjectsCreated.Any())
        {
            CreateMap();
        }
        else
        {
            DestroyImmediate(planeGo);
            OnDestroy();
        }
    }

    private GameObject ParentingNeighboorObjects(List<GameObject> listForParenting)
    {
        GameObject blockParent = new GameObject("Block_Parent");
        
        foreach (var block in listForParenting)
        {
            block.transform.parent = blockParent.transform;
        }
        allObjectsCreatedOnScene.Add(blockParent);
        return blockParent;
    }

    private Ray InitializeRay(Ray ray, Vector3 origin, Vector3 direction)
    {
        ray.origin = origin;
        ray.direction = direction;

        return ray;
    }

    private void HitReturn(RaycastHit hit, GameObject currentObject, List<GameObject> listGoParent)
    {
        Node hitNode = hit.collider.gameObject.GetComponent<Node>();
        Node currentObjNode = currentObject.GetComponent<Node>();

        if (hitNode.colorBloc == currentObjNode.colorBloc)
        {
            listGoParent.Add(hit.collider.gameObject);
        }
    }

    private void ChangeBlockColor()
    {
        int lastMatNumber = currentObjectSelected.GetComponent<Renderer>().sharedMaterials.Length - 2;
        Material[] tempSharedMat = currentObjectSelected.GetComponent<Renderer>().sharedMaterials;

        switch (colors)
        {
            case Colors.Blue:
                Material blueMat =
                    AssetDatabase.LoadAssetAtPath("Assets/Materials/CubeMat/M_PiqueCube.mat", typeof(Material)) as
                        Material;
                tempSharedMat[lastMatNumber] = blueMat;
                currentObjectSelected.GetComponent<Renderer>().sharedMaterials = tempSharedMat;
                currentBlockNode.colorBloc = Node.ColorBloc.Blue;
                break;
            case Colors.Green:
                Material greenMat =
                    AssetDatabase.LoadAssetAtPath("Assets/Materials/CubeMat/M_TrefleCube.mat", typeof(Material)) as
                        Material;
                tempSharedMat[lastMatNumber] = greenMat;
                currentObjectSelected.GetComponent<Renderer>().sharedMaterials = tempSharedMat;
                currentBlockNode.colorBloc = Node.ColorBloc.Green;
                break;
            case Colors.Red:
                Material redMat =
                    AssetDatabase.LoadAssetAtPath("Assets/Materials/CubeMat/M_CoeurCube.mat", typeof(Material)) as
                        Material;
                tempSharedMat[lastMatNumber] = redMat;
                currentObjectSelected.GetComponent<Renderer>().sharedMaterials = tempSharedMat;
                currentBlockNode.colorBloc = Node.ColorBloc.Red;
                break;
            case Colors.Yellow:
                Material yellowMat =
                    AssetDatabase.LoadAssetAtPath("Assets/Materials/CubeMat/M_CarreauCube.mat", typeof(Material)) as
                        Material;
                tempSharedMat[lastMatNumber] = yellowMat;
                currentObjectSelected.GetComponent<Renderer>().sharedMaterials = tempSharedMat;
                currentBlockNode.colorBloc = Node.ColorBloc.Yellow;
                break;
        }
    }

    private void OnDestroy()
    {
        //Call when close the window
        ResetVars();
    }


    void ResetVars()
    {
        isCreating = false;
        isBlockSelected = false;
        onMapEditor = false;
        lastObjectCreated = null;
        currentObjectSelected = null;
        planeGo = null;
        currentObjectsCreated.Clear();
    }
}