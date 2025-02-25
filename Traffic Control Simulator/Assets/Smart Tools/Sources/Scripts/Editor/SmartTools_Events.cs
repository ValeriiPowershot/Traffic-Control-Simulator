using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
using UnityEditor.Events;
using UnityEditor.ShortcutManagement;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace SmartTools
{
    public partial class SmartTools
    {
        static void RegisterEvents()
        {
            SceneView.duringSceneGui += OnScene;
            Selection.selectionChanged += OnSelectionChanged;
            Undo.postprocessModifications += OnPostProcessModifications;
            EditorSceneManager.activeSceneChangedInEditMode += OnSceneChanged;
            EditorSceneManager.sceneOpened += OnEditorSceneOpened;
            //EditorSceneManager.newSceneCreated += OnNewSceneCreated;
            //EditorSceneManager.activeSceneChangedInEditMode += OnActiveSceneChanged;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            EditorApplication.update += OnEditorUpdate;
            //EditorApplication.hierarchyWindowItemOnGUI += OnHierarchy;
            //EditorApplication.projectWindowItemOnGUI += OnProject;
            //EditorApplication.playModeStateChanged += OnPlaymodeStateChanged;
        }
        static void UnregisterEvents()
        {
            SceneView.duringSceneGui -= OnScene;
            Selection.selectionChanged -= OnSelectionChanged;
            Undo.postprocessModifications -= OnPostProcessModifications;
            EditorSceneManager.activeSceneChangedInEditMode -= OnSceneChanged;
            EditorSceneManager.sceneOpened -= OnEditorSceneOpened;
            //EditorSceneManager.newSceneCreated -= OnNewSceneCreated;
            //EditorSceneManager.activeSceneChangedInEditMode -= OnActiveSceneChanged;
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
            EditorApplication.update -= OnEditorUpdate;
            //EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchy;
            //EditorApplication.projectWindowItemOnGUI -= OnProject;
            //EditorApplication.playModeStateChanged -= OnPlaymodeStateChanged;
        }







        static void OnScene(SceneView sv)
        {
            if (!IsDisplayed()) return;

            sceneView = sv;

            sceneCamera = Camera.current;

            Event e = Event.current;
            int controlID = GUIUtility.hotControl;

            if (focus)
            {
                if (Selection.activeTransform != null)
                {
                    if (controlID == 0) sceneView.pivot = Selection.activeTransform.position;
                    if (e.type == EventType.MouseDown) { }
                    else if (e.button == 1) { sceneView.pivot = Selection.activeTransform.position; }
                }
                if (e.button == 2)
                {
                    focus = false;
                    tgl_Focus.UpdateToggle(focus);
                }
            }

            // Display Scene Notification
            if (notify.Length > 0)
            {
                sceneView.ShowNotification(new GUIContent(notify));
                notify = "";
            }

            AutoSnapping();

            //Child Compensation
            ChildCompensationUpdate();

            if (gridMesh == null || gridMaterial == null)
            {
                //string meshPath = SmartTools_Settings.installPath + "/Sources/SmartTools_Mesh.fbx";
                //string materialPath = SmartTools_Settings.installPath + "/Sources/SmartToolsGridMaterial.mat";
                string texturePath = SmartTools_Settings.installPath + "/Sources/SmartTools_Grid.psd";

                //gridMesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);

                //GameObject tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //gridMesh = tempCube.GetComponent<MeshFilter>().sharedMesh;
                //GameObject.DestroyImmediate(tempCube); // Destroy the temporary cube as we only need its mesh

                gridMesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

                if (gridMesh == null) Debug.LogError("Mesh not found!");//Debug.LogError("Mesh not found at: " + meshPath);

                if (gridMaterial == null)
                {
                    var shader = Shader.Find("Hidden/SmartTools_Shader");
                    if (shader == null) Debug.LogError("SmartTools Shader not found!");
                    else
                    {
                        var gridTex = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
                        if (gridTex == null) Debug.LogError(texturePath + " not found!");

                        //gridTex = SmartTools_Settings.grid;
                        gridTex.mipMapBias = -0.5f;


                        gridMaterial = new Material(shader);
                        gridMaterial.SetTexture("_Grid", gridTex);
                        gridMaterial.SetColor("_StreakColor", Color.white);//settings._StreakColor);
                        gridMaterial.SetColor("_GridColor", new Color(1, 1, 1, 0.5f));// settings._GridColor);
                    }
                }
            
            }


            if (Selection.activeTransform != null && Event.current.type == EventType.Repaint && showGrid)
            {
                float dist = Vector3.Distance(Selection.activeTransform.position, sceneCamera.transform.position) * 8;
                var matrix = Matrix4x4.TRS(Selection.activeTransform.position, Quaternion.identity, Vector3.one * dist);


                gridMaterial.SetFloat("_SnapTransparency", moveAreaSize * 0.75f);
                gridMaterial.SetFloat("_Fresnel", 1 + (viewDependency * viewDependency) * 15);
                gridMaterial.SetFloat("_Scale", dist / (dist / moveDistance));
                gridMaterial.SetFloat("_SnapAreaSize", dist / (dist * moveDistance));
                gridMaterial.SetFloat("_ObjectScale", dist);
                gridMaterial.SetFloat("_GridSteps", gridSize);
                Vector3 gridOffset2 = -(sceneData.gridOffset / gridSize) + Vector3.one * (offsetHalfGrid ? 0.0f : 0.5f);
                gridMaterial.SetVector("_SnapOffset", gridOffset2);
                gridMaterial.SetVector("_GridOffset", gridOffset2);
                gridMaterial.SetFloat("_VertexPush", settings._VertexPush * 1f + 0.6f);

                gridMaterial.SetPass(0);
                Graphics.DrawMeshNow(gridMesh, matrix);
            }
            if (sceneData == null)
            {
                GetSceneData();
            }

            if (e.type == EventType.MouseUp && e.button == 0)
            {
                // Would like to call StoreSelection(); directly here
                // but Selection.objects is not updated yet so we need
                // to delay the call into the next EditorUpdate
                storeSelection = true;
            }

            sld_HorizontalMode.Q("unity-dragger").style.backgroundColor = GetHorizontalMode() ? Color.green : new Color(0.6f, 0.6f, 0.6f, 1);
        }




        static UndoPropertyModification[] OnPostProcessModifications(UndoPropertyModification[] modifications)
        {
            ChildCompensationUpdate();
            return modifications;
        }



        static void OnEditorUpdate()
        {
            if (!IsDisplayed()) return;

            if (storeSelection)
            {
                StoreSelection();
                storeSelection = false;
            }
        }



        static void OnSelectionChanged()
        {
            if (!IsDisplayed()) return;
            tgl_CComp.UpdateToggle(cComp = false);

            var focusedWindow = EditorWindow.focusedWindow;
            if (focusedWindow != null && focusedWindow.titleContent.text == "Hierarchy")
            {
                StoreSelection();
            }
        }


        static void OnEditorSceneOpened(Scene scene, OpenSceneMode mode)
        {
            if (!IsDisplayed()) return;
            //Debug.Log("OnEditorSceneOpened");
            if (scene != EditorSceneManager.GetActiveScene()) return;
            GetSceneData();
            sceneData.ClearHistory();
            UpdateAllPresetButtons();

        }
        static void OnSceneChanged(Scene previousScene, Scene newScene)
        {
            if (!IsDisplayed()) return;
            //Debug.Log("OnSceneChanged");
            GetSceneData();
            sceneData.ClearHistory();
            UpdateAllPresetButtons();

        }
        //static void OnNewSceneCreated(Scene scene, NewSceneSetup setup, NewSceneMode mode)
        //{
        //    if (!IsDisplayed()) return;
        //    Debug.Log("OnNewSceneCreated");
        //    GetSceneData();
        //    sceneData.ClearHistory();
        //    UpdateAllPresetButtons();

        //}
        //static void OnActiveSceneChanged(Scene previousScene, Scene newScene)
        //{
        //    if (!IsDisplayed()) return;
        //    Debug.Log("OnActiveSceneChanged");
        //    GetSceneData();
        //    sceneData.ClearHistory();
        //    UpdateAllPresetButtons();
        //}
        


        static void OnHierarchyChanged()
        {
            if (sceneData == null) return;
            //Debug.Log("OnHierarchyChanged");
            sceneData.CheckAllSelections();
            UpdateAllPresetButtons();

        }
    }
}
