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
    private static List<GameObject> removedBlocObjectsList = new List<GameObject>();
    private static List<GameObject> allObjectsCreatedOnScene = new List<GameObject>();

   
    private static int _planeMapSizeX;

    private static int _planeMapSizeZ;
    
   

    private static Colors colors;
    private static Node currentBlocNode;
    private static GameObject planeGo;

    private static Theme theme;
    private static GameObject mapParent;
    private static GameObject environementParent;
    private static List<ScriptableObjectTheme> scriptableObjectThemes = new List<ScriptableObjectTheme>();
    private static ScriptableColorMaterials scriptableColorMaterials;

    private static GameObject mainThemeObject;
    // private static Material material


    [MenuItem("Window/Editor Map/Custom Map Editor")]
    public static void ShowWindow()
    {
        GetWindow<EditorMapWindow>("Map Editor");
        onMapEditor = true;
        LoadScriptableObject();
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
                removedBlocObjectsList.Add(spawnObj);
                allObjectsCreatedOnScene.Add(spawnObj);
                Vector3 targetPos = SnapOffset(hitInfo.point, new Vector3(spawnObj.transform.position.x + 0.5f, spawnObj.transform.position.y,
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
        _planeMapSizeX = EditorGUILayout.IntSlider("Size map X", _planeMapSizeX, 1,100);
        _planeMapSizeZ = EditorGUILayout.IntSlider("Size map Z", _planeMapSizeZ, 1,100);
        EditorGUILayout.Space(20);
        
        if (GUILayout.Button("Create plane"))
        {
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.position = new Vector3(12,-1,0);
            plane.transform.localScale = new Vector3((float)_planeMapSizeX/10, 1, (float)_planeMapSizeZ/10);
            planeGo = plane;
            allObjectsCreatedOnScene.Add(plane);
            isCreating = true;
            SpawnMainObjectTheme(theme);
        }
    }

    void SpawnMainObjectTheme(Theme theme)
    {
        if (!mainThemeObject)
        {
            DestroyImmediate(mainThemeObject);
            switch (theme)
            {
                case Theme.Volcano:
                    mainThemeObject = scriptableObjectThemes[0].mainThemeObject;
                    mainThemeObject = Instantiate(mainThemeObject, Vector3.zero, Quaternion.identity);
                    break;
                case Theme.Mountain:
                    mainThemeObject = scriptableObjectThemes[1].mainThemeObject;
                    mainThemeObject = Instantiate(mainThemeObject, Vector3.zero, Quaternion.identity);
                    break;
            } 
        }
       
    }

    private static void LoadScriptableObject()
    {
        scriptableColorMaterials =
            (ScriptableColorMaterials)AssetDatabase.LoadAssetAtPath("Assets/Scripts/PresetMap/MaterialColorBloc.asset",
                typeof(ScriptableObject));
        scriptableObjectThemes.Add(
            (ScriptableObjectTheme)AssetDatabase.LoadAssetAtPath("Assets/Scripts/PresetMap/VolcanoPreset.asset",
                typeof(ScriptableObject)));
        scriptableObjectThemes.Add(
            (ScriptableObjectTheme)AssetDatabase.LoadAssetAtPath("Assets/Scripts/PresetMap/MountainPreset.asset",
                typeof(ScriptableObject)));
    }

    private void OnLoadScriptableObject()
    {
        scriptableColorMaterials = null;
        scriptableObjectThemes.Clear();
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
        if (removedBlocObjectsList.Count > 0)
        {
            if (GUILayout.Button("Remove last bloc created"))
            {
                DestroyImmediate(removedBlocObjectsList[removedBlocObjectsList.Count - 1]);
                removedBlocObjectsList.Remove(removedBlocObjectsList[removedBlocObjectsList.Count - 1]);
                currentBlocObjectsCreated.Remove(removedBlocObjectsList[removedBlocObjectsList.Count - 1]);
            }

            if (isBlocSelected)
            {
                if (GUILayout.Button("Remove selected bloc"))
                {
                    DestroyImmediate(currentBlocSelected);
                    removedBlocObjectsList.Remove(currentBlocSelected);
                    currentBlocObjectsCreated.Remove(currentBlocSelected);
                }
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

    private void ChangeThemeObject()
    {
        //Editor window for changing mat of object
        EditorGUILayout.Space(20);
        GUILayout.Label("Choose a material for the bloc and press the button to apply the material");
        ShowingPreset(theme);
        EditorGUILayout.Space(20);
    }

    private void ShowingPreset(Theme mapTheme)
    {
        switch (mapTheme)
        {
            case Theme.Volcano:

                if (colors != Colors.None)
                {
                    for (int i = 0; i < scriptableObjectThemes[0].materials.Count; i++)
                    {
                        PickUpMaterialFromAsset(scriptableObjectThemes[0].materials[i]);
                    }
                }

                break;

            case Theme.Mountain:

                if (colors != Colors.None)
                {
                    for (int i = 0; i < scriptableObjectThemes[1].materials.Count; i++)
                    {
                        PickUpMaterialFromAsset(scriptableObjectThemes[1].materials[i]);
                    }
                }

                break;
        }
    }

    private void PickUpMaterialFromAsset(Material mat)
    {
        Material material = EditorGUILayout.ObjectField("Material", mat, typeof(Material)) as Material;
        if (GUILayout.Button("Select material"))
        {
            ChangeBlocMaterial(material);
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
            ChooseMapTheme();
            CreatePlane();
            
        }
        else
        {
            CreateObject();
        }


        if (isBlocSelected)
        {
            ChangeThemeObject();
            ColorizeObject();
        }

        if (isCreating && removedBlocObjectsList.Count >= 1)
        {
            EditorGUILayout.Space(20);
            if (GUILayout.Button("Create map"))
            {
                CreateMap();
            }
        }

        EditorGUILayout.Space(20);
        if (allObjectsCreatedOnScene.Count >= 1 && isCreating)
        {
            //Reset the window 
            if (GUILayout.Button("Reset"))
            {
                foreach (var obj in allObjectsCreatedOnScene)
                {
                    DestroyImmediate(obj);
                }

                allObjectsCreatedOnScene.Clear();
                DestroyImmediate(mapParent);
                DestroyImmediate(environementParent);
                OnDestroy();
                ShowWindow();
            }
        }
    }

    #region MapCreation

    private void CreateMap()
    {
        foreach (var block in removedBlocObjectsList)
        {
            if (block.GetComponent<Node>())
            {
                block.GetComponent<Node>().SetUpPossiblePath();
            }
        }
        
        foreach (var currentBloc in currentBlocObjectsCreated.ToList())
        {
            var neighborsBlocks = new List<GameObject>();
            DetectBlocs(currentBloc, neighborsBlocks);
            ParentingNeighborBlocs(neighborsBlocks);
        }

        //Destroys remaining block parent with no childs
        GameObject[] parents = GameObject.FindGameObjectsWithTag("BlockParent");
        GameObject[] parentsNone = GameObject.FindGameObjectsWithTag("BlackBlock");
        if (!mapParent)
        {
            mapParent = new GameObject("MapParent")
            {
                transform =
                {
                    position = Vector3.zero
                }
            };
            mapParent.tag = "Map";
        }

        for (int i = 0; i < parentsNone.Length; i++)
        {
            parentsNone[i].transform.parent = mapParent.transform;
            parentsNone[i].transform.gameObject.AddComponent<GroupBlockDetection>();
        }
        for (int i = 0; i < parents.Length; i++)
        {
            parents[i].transform.parent = mapParent.transform;
            parents[i].transform.gameObject.AddComponent<GroupBlockDetection>();
        }

        foreach (var parentNone in parentsNone)
        {
            if (parentNone.transform.childCount <= 0)
            {
                DestroyImmediate(parentNone);
            }
        }
        foreach (var parent in parents)
        {
            if (parent.transform.childCount <= 0)
            {
                DestroyImmediate(parent);
            }
        }

       
     
        
        DestroyImmediate(planeGo);
        ResetVars();
        SetupScriptsManager();
        SetUpEnvironment();
        
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorUtility.SetDirty(this);
    }

    void SetUpEnvironment()
    {
        if (!environementParent)
        {
            environementParent = new GameObject("Environement")
            {
                transform =
                {
                    position = Vector3.zero
                }
            };
        }
        
        mainThemeObject.transform.parent = environementParent.transform;
        environementParent.transform.parent = mapParent.transform;
    }

    void SetupScriptsManager()
    {
        GameObject[] parents = GameObject.FindGameObjectsWithTag("BlockParent");
        GameManager gameManager = FindObjectOfType<GameManager>();
        
        if (gameManager != null)
        {
            foreach (var parent in parents)
            {
                gameManager.allBlocParents.Add(parent);
            }

            Node[] nodeBlocs = FindObjectsOfType<Node>();

            foreach (var bloc in nodeBlocs)
            {
                gameManager.cleanList.Add(bloc.gameObject);
            }
        }
        
        
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
            if (block.CompareTag("Platform"))
            {
                block.transform.parent = blockParent.transform;
                blockParent.layer = 3;
                blockParent.tag = "BlockParent";
            } 
            else if (block.CompareTag("Untagged"))
            {
                block.transform.parent = blockParent.transform;
                blockParent.name = "BlocParent_None";
                blockParent.layer = 0;
                blockParent.tag = "BlackBlock";
            }
        }
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
        Material boxMat = scriptableColorMaterials.colorMaterials[scriptableColorMaterials.colorMaterials.Count - 1];
        switch (colors)
        {
            case Colors.Blue:
                Material blueMat = scriptableColorMaterials.colorMaterials[2];
                InitializeTemporarySharedMat(tempSharedMat, middleMatNumber, blueMat, lastMatNumber, boxMat);
                ColorBloc(tempSharedMat, Node.ColorBloc.Blue);
                ChangePrefabBaseBloc(lastBlocCreated, tempSharedMat, Node.ColorBloc.Blue);
                break;
            case Colors.Green:
                Material greenMat = scriptableColorMaterials.colorMaterials[3];
                InitializeTemporarySharedMat(tempSharedMat, middleMatNumber, greenMat, lastMatNumber, boxMat);
                ColorBloc(tempSharedMat, Node.ColorBloc.Green);
                ChangePrefabBaseBloc(lastBlocCreated, tempSharedMat, Node.ColorBloc.Green);
                break;
            case Colors.Red:
                Material redMat = scriptableColorMaterials.colorMaterials[1];
                InitializeTemporarySharedMat(tempSharedMat, middleMatNumber, redMat, lastMatNumber, boxMat);
                ColorBloc(tempSharedMat, Node.ColorBloc.Red);
                ChangePrefabBaseBloc(lastBlocCreated, tempSharedMat, Node.ColorBloc.Red);
                break;
            case Colors.Yellow:
                Material yellowMat = scriptableColorMaterials.colorMaterials[0];
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
                baseMat = scriptableObjectThemes[0].materials[scriptableObjectThemes[0].materials.Count - 1];
                break;
            case Theme.Mountain:
                baseMat = scriptableObjectThemes[1].materials[scriptableObjectThemes[1].materials.Count - 1];
                break;
        }

        for (int i = 0; i < materialBloc.Length-1; i++)
        {
            materialBloc[i] = baseMat;
        }

        currentBlocSelected.layer = 3;
        currentBlocSelected.tag = "Untagged";
        currentBlocSelected.gameObject.name = "Bloc_prefab_" + Colors.None;
        currentBlocSelected.GetComponent<Renderer>().sharedMaterials = materialBloc;
        ColorBloc(currentBlocSelected.GetComponent<Renderer>().sharedMaterials, Node.ColorBloc.None);
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
            for (int i = 0; i < tempSharedMat.Length - 1; i++)
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
        if (currentBlocSelected.layer != 3) currentBlocSelected.layer = 3;
        if (!currentBlocSelected.CompareTag("Platform")) currentBlocSelected.tag = "Platform";
    }

    #endregion


    private void OnDestroy()
    {
        //Call when close the window or clicked on reset button
        ResetVars();
        removedBlocObjectsList.Clear();
        OnLoadScriptableObject();
        onMapEditor = false;
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorUtility.SetDirty(this);
    }


    private void ResetVars()
    {
        isCreating = false;
        isBlocSelected = false;
        currentBlocSelected = null;
        DestroyImmediate(planeGo);
        currentBlocObjectsCreated.Clear();
        
    }
}