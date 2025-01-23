using System.Collections.Generic;
using System.Linq;
using BaseCode.Logic.Npcs.Npc;
using BaseCode.Logic.PathData;
using BaseCode.Logic.Roads;
using BaseCode.Logic.Ways;
using UnityEditor;
using UnityEngine;

namespace BaseCode.Editor.Path
{
    public class SceneObjectSpawnerEditor : EditorWindow
    {
        private Vector2 _scrollPosition;
        private static GameObject _selectedObject;
        private static SceneRoadGenerationController _sceneRoadGenerationController;
        
        
        #region Window

        [MenuItem("Window/Road Spawner")]
        public static void ShowWindow()
        {
            if (_selectedObject == null)
            {
                Debug.Log("To Open Window, Create A Cube Double Click On It!");
                return;
            }
            
            _sceneRoadGenerationController = FindObjectOfType<SceneRoadGenerationController>();

            if (_sceneRoadGenerationController == null)
            {
                var objects = new GameObject
                {
                    name = "----- Created Objects -----"
                };
                objects.transform.SetAsFirstSibling();
                
                _sceneRoadGenerationController = objects.AddComponent<SceneRoadGenerationController>();
            }
            
            _sceneRoadGenerationController.ClickedSelectedObject = _selectedObject;           
            RoadBase roadBase = _sceneRoadGenerationController.ClickedSelectedObject.GetComponent<RoadBase>();
            if (roadBase == null)
            {
                _sceneRoadGenerationController.SelectedRoad = _sceneRoadGenerationController.RoadsSo.roadPrefabs[0];
            }
            
            _sceneRoadGenerationController.RoadsSo.canDrawRoadGizmo = true;

            if(_sceneRoadGenerationController.RoadsSo.canOpenWindow == false)
                return;

            var window = GetWindow<SceneObjectSpawnerEditor>("Object Spawner");
            window.minSize = new Vector2(400, 800); // Minimum width and height
        }
        
        #endregion

        #region On GUI Part
        
        private void OnGUI()
        {
            if (!ValidatePathGenerationController())
                return;

            DisplaySelectedObjectInfo();
            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            DisplayRoadSelectionButtons();

            EditorGUILayout.Space();

            DisplayRotationControls();

            EditorGUILayout.Space();

            GenerateNpcControls();
            
            EditorGUILayout.Space();
            
            GenerateRoad();

            EditorGUILayout.EndScrollView();

        }

        private bool ValidatePathGenerationController()
        {
            if (_sceneRoadGenerationController == null || _sceneRoadGenerationController.ClickedSelectedObject == null)
            {
                EditorGUILayout.LabelField("Select an object in the scene.");
                _sceneRoadGenerationController.RoadsSo.canDrawRoadGizmo = false;
                return false;
            }
            return true;
        }

        private void DisplaySelectedObjectInfo()
        {
            EditorGUILayout.LabelField($"Selected Object: {_sceneRoadGenerationController.ClickedSelectedObject.name}");
        }

        private void DisplayRoadSelectionButtons()
        {
            foreach (var roadBase in _sceneRoadGenerationController.RoadsSo.roadPrefabs)
            {
                GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
                {
                    normal = 
                        { textColor = 
                            roadBase == _sceneRoadGenerationController.SelectedRoad ? 
                                Color.green : GUI.skin.button.normal.textColor }
                };

                if (GUILayout.Button($"{roadBase.name} Select", buttonStyle))
                {
                    _sceneRoadGenerationController.SelectedRoad = roadBase;
                }
            }
        }

        private void GenerateNpcControls()
        {
            if (IsInvalidStartOrEnd(_sceneRoadGenerationController.ClickedSelectedObject) == false)
                return;

            EditorGUILayout.LabelField("NPC Generation Controls");
            
            if (GUILayout.Button("Create NPC"))
            {
                bool result = false;
                var roadBase = GetRoadBase(_sceneRoadGenerationController.ClickedSelectedObject);
                
                if (roadBase.GetType() == typeof(ForthRoadIntersection))
                {
                    result = ((ForthRoadIntersection)roadBase).GenerateNpc<BasicNpc>(_sceneRoadGenerationController.RoadsSo);
                }
                else if (roadBase.GetType() == typeof(TripleRoadIntersection))
                {
                    result = ((TripleRoadIntersection)roadBase)
                        .GenerateNpc<BasicNpc>(_sceneRoadGenerationController.RoadsSo);
                }

                Debug.Log(result ? "NPC Created" : "Cannot Create NPC!");
            }

            if (GUILayout.Button("Destroy NPC"))
            {
                bool result = false;

                var roadBase = GetRoadBase(_sceneRoadGenerationController.ClickedSelectedObject);
                
                if (roadBase.GetType() == typeof(ForthRoadIntersection))
                {
                    result = ((ForthRoadIntersection)roadBase).DestroyNpc();
                }
                else if (roadBase.GetType() == typeof(TripleRoadIntersection))
                {
                    result = ((TripleRoadIntersection)roadBase).DestroyNpc();
                }

                Debug.Log(result ? "NPC Destroyed" : "Cannot Destroy NPC!");
            }
        }

        
        private void DisplayRotationControls()
        {
            EditorGUILayout.LabelField("Change Rotation Of Selected Object");
            ChangeRotation();
        }
        
        // change rotation of road
        private void ChangeRotation()
        {
            if (_sceneRoadGenerationController.ClickedSelectedObject == null)
            {
                EditorGUILayout.LabelField("No object selected.");
                return;
            }

            var currentRotation = _sceneRoadGenerationController.ClickedSelectedObject.transform.rotation;

            float currentY = Mathf.Round(currentRotation.eulerAngles.y / 90f) * 90f;

            float newRotationY = EditorGUILayout.Slider("Rotation Y", currentY, 0f, 270f);

            if (!Mathf.Approximately(newRotationY, currentRotation.eulerAngles.y))
            {
                _sceneRoadGenerationController.ClickedSelectedObject.transform.rotation = Quaternion.Euler(currentRotation.eulerAngles.x, newRotationY, currentRotation.eulerAngles.z);
                EditorUtility.SetDirty(_sceneRoadGenerationController.ClickedSelectedObject);
            }
        }
        #endregion
        
        #region Generate Road Part
        private void GenerateRoad()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            _sceneRoadGenerationController.selectedRoads ??= new List<RoadBase>();
            
            foreach (var road in _sceneRoadGenerationController.selectedRoads)
            {
                if (road != null)
                {
                    EditorGUILayout.LabelField(road.name);
                }
            }

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Add Selected Generate Path Roads From Road"))
            {
                if(AddSelectedRoads())
                    GeneratePathRoad();
            }
            
            if (GUILayout.Button("Confirm Current Path Generate"))
            {
                GenerateNewPath();
            }
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Clean Visuals"))
            {
                CleanVisuals();
            }
        }
        
        #endregion

        #region  Generate Path
        
        private void GeneratePathRoad()
        {
            var roads = _sceneRoadGenerationController.selectedRoads;
            var size = roads.Count;

            foreach (var roadBase in _sceneRoadGenerationController.selectedRoads)
            {
                roadBase.startPoint = null;
                roadBase.endPoint = null;
            }
            for (int i = 0; i < size; i++)
            {
                if (i == size - 1) // Check if it's the last road
                {
                    roads[i].ConnectPath(null);
                }
                else
                {
                    roads[i].ConnectPath(roads[i + 1]);
                }
            }
        }
        private bool AddSelectedRoads()
        {
            if (ValidateSelectedObjects() == false)
                return false;

            CleanVisuals();

            RoadBase previousRoad = GetRoadBase(Selection.gameObjects[0]);

            foreach (var obj in Selection.gameObjects)
            {
                RoadBase currentRoad = GetRoadBase(obj);

                if (AreConsecutiveIntersectionsInvalid(previousRoad, currentRoad))
                {
                    _sceneRoadGenerationController.selectedRoads.Clear();
                    return false;
                }

                AddRoadToPath(currentRoad);

                previousRoad = currentRoad;
            }

            return true;
        }

        private bool ValidateSelectedObjects()
        {
            var selectedObjects = Selection.gameObjects;

            if (selectedObjects.Length <= 2)
            {
                Debug.LogError("Select more than 2 Roads");
                return false;
            }

            _sceneRoadGenerationController.selectedRoads.Clear();

            if (IsInvalidStartOrEnd(selectedObjects[0]) || IsInvalidStartOrEnd(selectedObjects[^1]))
            {
                Debug.LogError("First or End of Path Can't Be Intersection");
                return false;
            }

            return true;
        }

        private RoadBase GetRoadBase(GameObject obj) => obj.GetComponent<RoadBase>();

        private bool IsInvalidStartOrEnd(GameObject obj)
        {
            return GetRoadBase(obj) is (TripleRoadIntersection or ForthRoadIntersection);
        }

        private bool AreConsecutiveIntersectionsInvalid(RoadBase previousRoad, RoadBase currentRoad)
        {
            return previousRoad is TripleRoadIntersection && currentRoad is TripleRoadIntersection;
        }

        private void AddRoadToPath(RoadBase road)
        {
            if (road != null && !_sceneRoadGenerationController.selectedRoads.Contains(road))
            {
                _sceneRoadGenerationController.selectedRoads.Add(road);
            }
        }
        public void GenerateNewPath()
        {
            var createNewContainer = new GameObject
            {
                transform =
                {
                    parent = _sceneRoadGenerationController.AllWaysContainer.transform
                },
                name = "PathContainer"
            };
            var waypointContainer = createNewContainer.AddComponent<WaypointContainer>();

            foreach (var objectsCreatedRoadBase in _sceneRoadGenerationController.createdRoadBases.ToList())
            {
                if(objectsCreatedRoadBase == null)
                    _sceneRoadGenerationController.createdRoadBases.Remove(objectsCreatedRoadBase);
            }
            
            foreach (var roadBase in _sceneRoadGenerationController.selectedRoads)
            {
                roadBase.startPoint = null;
                roadBase.endPoint = null;
                
                waypointContainer.SetRoadPoints(roadBase.path,roadBase.decelerationPoints,roadBase.accelerationPoints);
                
                roadBase.path.Clear();
                roadBase.decelerationPoints.Clear();
                roadBase.accelerationPoints.Clear();
            }
            
            Selection.activeGameObject = createNewContainer;
        }
        
        #endregion
        
        #region Editor Visual
        [InitializeOnLoadMethod]
        private static void EnableSceneClick()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }
        
        private static void OnSceneGUI(SceneView sceneView)
        {
            Event e = Event.current;

            if (IsDoubleClick(e))
            {
                HandleDoubleClick(e);
                return;
            }

            UpdatePathGenerationControllerState(e);

            if (IsSingleClick(e))
            {
                HandleSingleClick();
            }
        }

        private static bool IsDoubleClick(Event e) =>
            e.type == EventType.MouseDown && e.button == 0 && e.clickCount == 2 && !e.alt;

        private static bool IsSingleClick(Event e) =>
            e.type == EventType.MouseDown && e.button == 0 && e.modifiers == EventModifiers.None;

        private static void HandleDoubleClick(Event e)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                _selectedObject = hit.collider.gameObject;
                //ShowWindow();
                e.Use();
            }
        }

        private static void UpdatePathGenerationControllerState(Event e)
        {
            if (_sceneRoadGenerationController == null)
                return;

            _sceneRoadGenerationController.SelectingNewRoad = e.modifiers != EventModifiers.None;
        }

        private static void HandleSingleClick()
        {
            if (_sceneRoadGenerationController == null || _sceneRoadGenerationController.CurrentDirection == Vector3.zero)
            {
                if (_sceneRoadGenerationController != null) _sceneRoadGenerationController.ClickedSelectedObject = null;

                return;
            }

            if (_sceneRoadGenerationController.ClickedSelectedObject != null && 
                _sceneRoadGenerationController.SelectedRoad != null &&
                !_sceneRoadGenerationController.SelectingNewRoad)
            {
                _sceneRoadGenerationController.SpawnNow();
            }
        }
        
        private void CleanVisuals()
        {
            foreach (var roadBase in _sceneRoadGenerationController.createdRoadBases)
            {
                if(roadBase == null)
                    continue;
                roadBase.startPoint = null;
                roadBase.endPoint = null;
                
                roadBase.path.Clear();
                roadBase.decelerationPoints.Clear();
                roadBase.accelerationPoints.Clear();
            }
            _sceneRoadGenerationController.SelectedRoad = null;
        }
        #endregion
    }
}
