
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public enum Colors
{
    Red,
    Blue,
    Yellow,
    Green,
    None
};

public enum Theme
{
    Volcano,
    Mountain,
    Submarine,
    Temple
};


public class EditorMapWindow : EditorWindow
{
    private static GameObject lastBlocCreated, currentBlocSelected;
    private static bool onMapEditor;
    private static bool isCreating;
    private static bool isBlocSelected;
    private static List<GameObject> currentBlocObjectsCreated = new List<GameObject>();
    private static List<GameObject> allObjectsCreatedOnScene = new List<GameObject>();
    private static Vector2 planeMapSize;

    private static Colors colors;
    private static Node currentBlocNode;
    private static GameObject planeGo;

    private static Theme theme;

    private static Material[] materials = new Material[5];
    // private static Material material


    [MenuItem("Window/Editor Map/Custom Map Editor")]
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

    private static void SpawnObjectOnPlane(UnityEngine.Event e)
    {
        // Shoot a ray from the mouse position into the world
        Ray worldRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(worldRay, out hitInfo, Mathf.Infinity))
        {
            if (hitInfo.collider.GetComponent<MeshCollider>() && lastBlocCreated != null)
            {
                GameObject spawnObj = Instantiate(lastBlocCreated);
                currentBlocObjectsCreated.Add(spawnObj);
                allObjectsCreatedOnScene.Add(spawnObj);
                Vector3 targetPos = SnapOffset(hitInfo.point,
                    new Vector3(spawnObj.transform.position.x + 0.5f, spawnObj.transform.position.y,
                        spawnObj.transform.position.z + 0.5f));
                spawnObj.transform.position = targetPos;
            }
        }
    }


    //Calling multiple time when scene window in focused
    private static void UpdateSceneView(SceneView sceneView)
    {
        UnityEngine.Event e = UnityEngine.Event.current;

        if (e.type == EventType.MouseDown && e.button == 0 && onMapEditor)
        {
            //Spawn bloc on plane

            SpawnObjectOnPlane(e);
        }

        if (Selection.activeGameObject && isCreating && !isBlocSelected)
        {
            if (Selection.activeGameObject.GetComponent<BoxCollider>())
            {
                currentBlocSelected = Selection.activeGameObject;
                currentBlocNode = Selection.activeGameObject.GetComponent<Node>();
                isBlocSelected = true;
            }

            Selection.selectionChanged -= SelectionChanged;
            Selection.selectionChanged += SelectionChanged;
        }

        UnityEngine.Event.current = null;
    }

    private static void SelectionChanged()
    {
        isBlocSelected = false;
    }

    private void OnInspectorUpdate()
    {
        //Update the window
        Repaint();
    }

    #region GUI

    private void CreatePlane()
    {
        //Editor window for creating base plane for placing object
        GUILayout.Label("MAP EDITOR TOOL", EditorStyles.boldLabel);
        EditorGUILayout.Space(20);
        GUILayout.Label("Create a plane on the scene to place blocs, you can choose it's size");
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
        //Editor Window for creating object
        EditorGUILayout.Space(20);
        GUILayout.Label("Left click on the scene to create a bloc", EditorStyles.largeLabel);
        lastBlocCreated = EditorGUILayout.ObjectField("Bloc",
            AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Bloc_prefab/Bloc_prefab.prefab", typeof(GameObject)),
            typeof(GameObject), true) as GameObject;
        EditorGUILayout.Space(20);
        if (currentBlocObjectsCreated.Count > 0)
        {
            if (GUILayout.Button("Remove last bloc created"))
            {
                DestroyImmediate(currentBlocObjectsCreated[currentBlocObjectsCreated.Count - 1]);
                currentBlocObjectsCreated.Remove(currentBlocObjectsCreated[currentBlocObjectsCreated.Count - 1]);
            }
        }
    }

    private void ChooseMapTheme()
    {
        EditorGUILayout.Space(20);
        GUILayout.Label("Choose a theme to show the materials corresponding to the theme");
        theme = (Theme)EditorGUILayout.EnumPopup("Theme", theme);
        EditorGUILayout.Space(20);
    }

    private void ChangeMatObject()
    {
        //Editor window for changing mat of object
        EditorGUILayout.Space(20);
        GUILayout.Label("Choose a material for the bloc and press the button to apply the material");
        ShowingMaterials(theme);
        EditorGUILayout.Space(20);
    }

    private void ShowingMaterials(Theme mapTheme)
    {
        switch (mapTheme)
        {
            case Theme.Volcano:
                if (colors != Colors.None)
                {
                    PickUpMaterialFromAsset(materials[0],
                        EditorGUILayout.ObjectField("Material",
                            AssetDatabase.LoadAssetAtPath("Assets/Materials/CubeMat/Volcano/Iteration1/M_grass.mat",
                                typeof(Material)), typeof(Material), true));
                    PickUpMaterialFromAsset(materials[1],
                        EditorGUILayout.ObjectField("Material",
                            AssetDatabase.LoadAssetAtPath("Assets/Materials/CubeMat/Volcano/Iteration1/M_rock.mat",
                                typeof(Material)), typeof(Material), true));
                    PickUpMaterialFromAsset(materials[2],
                        EditorGUILayout.ObjectField("Material",
                            AssetDatabase.LoadAssetAtPath("Assets/Materials/CubeMat/Volcano/Iteration1/M_sand.mat",
                                typeof(Material)), typeof(Material), true));
                    PickUpMaterialFromAsset(materials[3],
                        EditorGUILayout.ObjectField("Material",
                            AssetDatabase.LoadAssetAtPath(
                                "Assets/Materials/CubeMat/Volcano/Iteration1/M_volcanic_rock.mat", typeof(Material)),
                            typeof(Material), true));
                }

                break;

            case Theme.Mountain:
                if (colors != Colors.None)
                {
                    PickUpMaterialFromAsset(materials[0],
                        EditorGUILayout.ObjectField("Material",
                            AssetDatabase.LoadAssetAtPath("Assets/Materials/CubeMat/Mountain/Iteration1/M_ice.mat",
                                typeof(Material)), typeof(Material), true));
                    PickUpMaterialFromAsset(materials[1],
                        EditorGUILayout.ObjectField("Material",
                            AssetDatabase.LoadAssetAtPath("Assets/Materials/CubeMat/Mountain/Iteration1/M_snow.mat",
                                typeof(Material)), typeof(Material), true));
                    PickUpMaterialFromAsset(materials[2],
                        EditorGUILayout.ObjectField("Material",
                            AssetDatabase.LoadAssetAtPath(
                                "Assets/Materials/CubeMat/Mountain/Iteration1/M_mountain_stone.mat", typeof(Material)),
                            typeof(Material), true));
                }

                break;
        }
    }

    private void PickUpMaterialFromAsset(Material mat, Object objectField)
    {
        mat = objectField as Material;
        if (GUILayout.Button("Select material"))
        {
            ChangeBlocMaterial(mat);
        }
    }


    private void ColorizeObject()
    {
        //Editor window for changing colorBloc of bloc
        EditorGUILayout.Space(20);
        GUILayout.Label("Choose a color for the selected bloc");
        colors = (Colors)EditorGUILayout.EnumPopup("Choose a color", colors);
        EditorGUILayout.Space(20);
        if (currentBlocSelected != null)
        {
            ChangeBlocColor();
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


        if (isBlocSelected)
        {
            ChooseMapTheme();
            ChangeMatObject();
            ColorizeObject();
        }

        if (isCreating && currentBlocObjectsCreated.Count >= 1)
        {
            EditorGUILayout.Space(20);
            if (GUILayout.Button("Create map"))
            {
                foreach (var block in currentBlocObjectsCreated)
                {
                    if (block.GetComponent<Node>())
                    {
                        block.GetComponent<Node>().SetUpPossiblePath();
                    }
                }

                CreateMap();
            }
        }

        EditorGUILayout.Space(20);
        if (currentBlocObjectsCreated.Count >= 1 && isCreating)
        {
            //Reset the window 
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
    }

    #region MapCreation

    private void CreateMap()
    {
        foreach (var currentBloc in currentBlocObjectsCreated.ToList())
        {
            var neighborsBlocks = new List<GameObject>();
            DetectBlocs(currentBloc, neighborsBlocks);
            ParentingNeighborBlocs(neighborsBlocks);
        }

        //Destroys remaining block parent with no childs
        GameObject[] parents = GameObject.FindGameObjectsWithTag("BlockParent");

        foreach (var parent in parents)
        {
            if (parent.transform.childCount <= 0)
            {
                DestroyImmediate(parent);
            }
        }

        DestroyImmediate(planeGo);
        ResetVars();
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorUtility.SetDirty(this);
    }

    void DetectBlocs(GameObject currentBloc, List<GameObject> neighborsBlocs)
    {
        //Shoot 4 raycast int 4 different direction
        var rays = new Ray[4];
        var raycastHits = new RaycastHit[4];
        var vectorRaycast = new List<Vector3> { Vector3.back, Vector3.forward, Vector3.right, Vector3.left };


        //If the list for parenting the objects doesn't contains our currentObj we add him in the list and remove from the principal list
        if (!neighborsBlocs.Contains(currentBloc))
        {
            neighborsBlocs.Add(currentBloc);
            currentBlocObjectsCreated.Remove(currentBloc);
        }

        //For each rays if we hit 
        for (int i = 0; i < rays.Length; i++)
        {
            rays[i] = InitializeRay(rays[i], currentBloc.transform.position, vectorRaycast[i]);

            if (Physics.Raycast(rays[i], out raycastHits[i], 1.2f))
            {
                //Check what our currentObj hit 
                HitReturn(raycastHits[i], currentBloc, neighborsBlocs);
            }
        }
    }

    private void ParentingNeighborBlocs(List<GameObject> neighborsBlocs)
    {
        //Create a parent and for each gameobject of neighbors list change there parent to the create one
        var blockParent = new GameObject("Block parent");
        foreach (var block in neighborsBlocs)
        {
            block.transform.parent = blockParent.transform;
        }

        blockParent.tag = "BlockParent";

        allObjectsCreatedOnScene.Add(blockParent);
    }

    private Ray InitializeRay(Ray ray, Vector3 origin, Vector3 direction)
    {
        ray.origin = origin;
        ray.direction = direction;

        return ray;
    }

    private void HitReturn(RaycastHit hit, GameObject currentBloc, List<GameObject> neighborsBlocs)
    {
        bool isCurrentBlocNode = currentBloc.GetComponent<Node>();
        bool isHitBlocNode = hit.collider.gameObject.GetComponent<Node>();
       
        //Check the colorbloc value of hit gameobject and currentObject 
        if (isCurrentBlocNode && isHitBlocNode)
        {
            Node hitNode = hit.collider.gameObject.GetComponent<Node>();
            Node currentObjNode = currentBloc.GetComponent<Node>();

            //Add hit go in the list and Reload the function with the hit go  
            if (hitNode.colorBloc == currentObjNode.colorBloc && !neighborsBlocs.Contains(hit.collider.gameObject))
            {
                neighborsBlocs.Add(hit.collider.gameObject);
                currentBlocObjectsCreated.Remove(hit.collider.gameObject);
                DetectBlocs(hit.collider.gameObject, neighborsBlocs);
            }
        }
        else if (!neighborsBlocs.Contains(hit.collider.gameObject) && !isHitBlocNode && !isCurrentBlocNode)
        {
            neighborsBlocs.Add(hit.collider.gameObject);
            currentBlocObjectsCreated.Remove(hit.collider.gameObject);
            DetectBlocs(hit.collider.gameObject, neighborsBlocs);
        }
        
    }

    #endregion

    #region BlocModifiers

    private void ChangeBlocColor()
    {
        //Change bloc color base on the window enum selected
        int middleMatNumber = currentBlocSelected.GetComponent<Renderer>().sharedMaterials.Length - 2;
        int lastMatNumber = currentBlocSelected.GetComponent<Renderer>().sharedMaterials.Length - 1;
        Material[] tempSharedMat = currentBlocSelected.GetComponent<Renderer>().sharedMaterials;
        Material boxMat =
            AssetDatabase.LoadAssetAtPath("Assets/Materials/CubeMat/M_box.mat", typeof(Material)) as Material;
        switch (colors)
        {
            case Colors.Blue:
                Material blueMat =
                    AssetDatabase.LoadAssetAtPath("Assets/Materials/CubeMat/M_PiqueCube.mat", typeof(Material)) as
                        Material;
                InitializeTemporarySharedMat(tempSharedMat, middleMatNumber, blueMat, lastMatNumber, boxMat);
                ColorBloc(tempSharedMat, Node.ColorBloc.Blue);
                ChangePrefabBaseBloc(lastBlocCreated, tempSharedMat, Node.ColorBloc.Blue);
                break;
            case Colors.Green:
                Material greenMat =
                    AssetDatabase.LoadAssetAtPath("Assets/Materials/CubeMat/M_TrefleCube.mat", typeof(Material)) as
                        Material;
                InitializeTemporarySharedMat(tempSharedMat, middleMatNumber, greenMat, lastMatNumber, boxMat);
                ColorBloc(tempSharedMat, Node.ColorBloc.Green);

                ChangePrefabBaseBloc(lastBlocCreated, tempSharedMat, Node.ColorBloc.Green);
                break;
            case Colors.Red:
                Material redMat =
                    AssetDatabase.LoadAssetAtPath("Assets/Materials/CubeMat/M_CoeurCube.mat", typeof(Material)) as
                        Material;
                InitializeTemporarySharedMat(tempSharedMat, middleMatNumber, redMat, lastMatNumber, boxMat);
                ColorBloc(tempSharedMat, Node.ColorBloc.Red);
                ChangePrefabBaseBloc(lastBlocCreated, tempSharedMat, Node.ColorBloc.Red);
                break;
            case Colors.Yellow:
                Material yellowMat =
                    AssetDatabase.LoadAssetAtPath("Assets/Materials/CubeMat/M_CarreauCube.mat", typeof(Material)) as
                        Material;
                InitializeTemporarySharedMat(tempSharedMat, middleMatNumber, yellowMat, lastMatNumber, boxMat);
                ColorBloc(tempSharedMat, Node.ColorBloc.Yellow);
                ChangePrefabBaseBloc(lastBlocCreated, tempSharedMat, Node.ColorBloc.Yellow);
                break;
            case Colors.None:
                UnmovableBlocSetUp(tempSharedMat);
                break;
        }
    }

    private void InitializeTemporarySharedMat(Material[] sharedMaterials, int indexMiddleMatNumber,
        Material colorMaterial, int indexLastMatNumber, Material blocMaterial)
    {
        //Initialize value for sharedMaterials
        sharedMaterials[indexMiddleMatNumber] = colorMaterial;
        sharedMaterials[indexLastMatNumber] = blocMaterial;
        sharedMaterials[0] = lastBlocCreated.GetComponent<Renderer>().sharedMaterial;
    }

    private void UnmovableBlocSetUp(Material[] materialBloc)
    {
        //Set up the block for unMovable mat
        Material baseMat = null;
        switch (theme)
        {
            case Theme.Volcano:
                baseMat = AssetDatabase.LoadAssetAtPath(
                    "Assets/Materials/CubeMat/Volcano/Iteration1/M_unmovable_volcanic.mat",
                    typeof(Material)) as Material;
                break;
            case Theme.Mountain:
                baseMat = AssetDatabase.LoadAssetAtPath(
                    "Assets/Materials/CubeMat/Mountain/Iteration1/M_unmovable_mountain.mat",
                    typeof(Material)) as Material;
                break;
        }

        for (int i = 0; i < materialBloc.Length; i++)
        {
            materialBloc[i] = baseMat;
        }

        currentBlocSelected.gameObject.name = "Bloc_prefab_" + Colors.None;
        currentBlocSelected.GetComponent<Renderer>().sharedMaterials = materialBloc;
        Node node = currentBlocSelected.GetComponent<Node>();
        DestroyImmediate(node);
    }

    private void ColorBloc(Material[] tempSharedMaterial, Node.ColorBloc colorBloc)
    {
        //Change the shared bloc mats
        if (!currentBlocSelected.GetComponent<Node>())
        {
            currentBlocNode = currentBlocSelected.AddComponent<Node>();
        }

        currentBlocSelected.GetComponent<Renderer>().sharedMaterials = tempSharedMaterial;
        if (currentBlocNode != null)
        {
            currentBlocNode.colorBloc = colorBloc;
        }

        currentBlocSelected.gameObject.name = "Bloc_prefab_" + colorBloc;
    }

    private void ChangeBlocMaterial(Material material)
    {
        //Change the base material of the currentBlocSelected
        bool isBlocMovable = currentBlocSelected.GetComponent<Node>();
        Material[] tempSharedMat = currentBlocSelected.GetComponent<Renderer>().sharedMaterials;
        if (isBlocMovable)
        {
            tempSharedMat[0] = material;
            currentBlocSelected.GetComponent<Renderer>().sharedMaterials = tempSharedMat;
            lastBlocCreated.GetComponent<Renderer>().sharedMaterials = tempSharedMat;
        }
        else
        {
            for (int i = 0; i < tempSharedMat.Length; i++)
            {
                tempSharedMat[i] = material;
            }

            currentBlocSelected.GetComponent<Renderer>().sharedMaterials = tempSharedMat;
            lastBlocCreated.GetComponent<Renderer>().sharedMaterials = tempSharedMat;
        }
    }

    private void ChangePrefabBaseBloc(GameObject prefab, Material[] sharedMat, Node.ColorBloc colorBloc)
    {
        //Update the prefab bloc
        prefab.GetComponent<Renderer>().sharedMaterials = sharedMat;
        prefab.GetComponent<Node>().colorBloc = colorBloc;
    }

    #endregion


    private void OnDestroy()
    {
        //Call when close the window
        ResetVars();
        onMapEditor = false;
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorUtility.SetDirty(this);
    }


    private void ResetVars()
    {
        isCreating = false;
        isBlocSelected = false;
        currentBlocSelected = null;
        planeGo = null;
        currentBlocObjectsCreated.Clear();
        allObjectsCreatedOnScene.Clear();
    }
}