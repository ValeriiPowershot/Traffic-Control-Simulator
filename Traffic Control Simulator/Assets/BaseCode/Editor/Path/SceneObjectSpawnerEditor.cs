using System.Collections.Generic;
using System.Linq;
using BaseCode.Logic;
using BaseCode.Logic.PathData;
using BaseCode.Logic.Roads;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Ways;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BaseCode.Editor.Path
{
    public class SceneObjectSpawnerEditor : EditorWindow
    {
        private Vector2 _scrollPosition;
        private static GameObject _selectedObject;
        private static PathGenerationController _pathGenerationController;
        
        #region Window

        [MenuItem("Window/Road Spawner")]
        public static void ShowWindow()
        {
            if (_selectedObject == null)
            {
                Debug.Log("To Open Window, Create A Cube Double Click On It!");
                return;
            }
            
            _pathGenerationController = FindObjectOfType<PathGenerationController>();

            if (_pathGenerationController == null)
            {
                var objects = new GameObject
                {
                    name = "----- Created Objects -----"
                };
                objects.transform.SetAsFirstSibling();
                
                _pathGenerationController = objects.AddComponent<PathGenerationController>();
            }
            
            _pathGenerationController.ClickedSelectedObject = _selectedObject;           
            RoadBase roadBase = _pathGenerationController.ClickedSelectedObject.GetComponent<RoadBase>();
            if (roadBase == null)
            {
                _pathGenerationController.SelectedRoad = _pathGenerationController.RoadsSo.prefabs[0];
            }
            
            _pathGenerationController.RoadsSo.canDrawRoadGizmo = true;

            if(_pathGenerationController.RoadsSo.canOpenWindow == false)
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

            GenerateRoad();

            EditorGUILayout.EndScrollView();
        }

        private bool ValidatePathGenerationController()
        {
            if (_pathGenerationController == null || _pathGenerationController.ClickedSelectedObject == null)
            {
                EditorGUILayout.LabelField("Select an object in the scene.");
                _pathGenerationController.RoadsSo.canDrawRoadGizmo = false;
                return false;
            }
            return true;
        }

        private void DisplaySelectedObjectInfo()
        {
            EditorGUILayout.LabelField($"Selected Object: {_pathGenerationController.ClickedSelectedObject.name}");
        }

        private void DisplayRoadSelectionButtons()
        {
            foreach (var roadBase in _pathGenerationController.RoadsSo.prefabs)
            {
                GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
                {
                    normal = 
                        { textColor = 
                            roadBase == _pathGenerationController.SelectedRoad ? 
                                Color.green : GUI.skin.button.normal.textColor }
                };

                if (GUILayout.Button($"{roadBase.name} Select", buttonStyle))
                {
                    _pathGenerationController.SelectedRoad = roadBase;
                }
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
            if (_pathGenerationController.ClickedSelectedObject == null)
            {
                EditorGUILayout.LabelField("No object selected.");
                return;
            }

            var currentRotation = _pathGenerationController.ClickedSelectedObject.transform.rotation;

            float currentY = Mathf.Round(currentRotation.eulerAngles.y / 90f) * 90f;

            float newRotationY = EditorGUILayout.Slider("Rotation Y", currentY, 0f, 270f);

            if (!Mathf.Approximately(newRotationY, currentRotation.eulerAngles.y))
            {
                _pathGenerationController.ClickedSelectedObject.transform.rotation = Quaternion.Euler(currentRotation.eulerAngles.x, newRotationY, currentRotation.eulerAngles.z);
                EditorUtility.SetDirty(_pathGenerationController.ClickedSelectedObject);
            }
        }
        #endregion
        
        #region Generate Road Part
        private void GenerateRoad()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            _pathGenerationController.selectedRoads ??= new List<RoadBase>();
            
            foreach (var road in _pathGenerationController.selectedRoads)
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
            var roads = _pathGenerationController.selectedRoads;
            var size = roads.Count;

            foreach (var roadBase in _pathGenerationController.selectedRoads)
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
                    _pathGenerationController.selectedRoads.Clear();
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

            _pathGenerationController.selectedRoads.Clear();

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
            if (road != null && !_pathGenerationController.selectedRoads.Contains(road))
            {
                _pathGenerationController.selectedRoads.Add(road);
            }
        }
        public void GenerateNewPath()
        {
            var createNewContainer = new GameObject
            {
                transform =
                {
                    parent = _pathGenerationController.AllWaysContainer.transform
                },
                name = "PathContainer"
            };
            var waypointContainer = createNewContainer.AddComponent<WaypointContainer>();

            foreach (var objectsCreatedRoadBase in _pathGenerationController.createdRoadBases.ToList())
            {
                if(objectsCreatedRoadBase == null)
                    _pathGenerationController.createdRoadBases.Remove(objectsCreatedRoadBase);
            }
            
            foreach (var roadBase in _pathGenerationController.selectedRoads)
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
                ShowWindow();
                e.Use();
            }
        }

        private static void UpdatePathGenerationControllerState(Event e)
        {
            if (_pathGenerationController == null)
                return;

            _pathGenerationController.SelectingNewRoad = e.modifiers != EventModifiers.None;
        }

        private static void HandleSingleClick()
        {
            if (_pathGenerationController == null || _pathGenerationController.CurrentDirection == Vector3.zero)
            {
                if (_pathGenerationController != null) _pathGenerationController.ClickedSelectedObject = null;

                return;
            }

            if (_pathGenerationController.ClickedSelectedObject != null && 
                _pathGenerationController.SelectedRoad != null &&
                !_pathGenerationController.SelectingNewRoad)
            {
                _pathGenerationController.SpawnNow();
            }
        }
        
        private void CleanVisuals()
        {
            foreach (var roadBase in _pathGenerationController.createdRoadBases)
            {
                if(roadBase == null)
                    continue;
                roadBase.startPoint = null;
                roadBase.endPoint = null;
                
                roadBase.path.Clear();
                roadBase.decelerationPoints.Clear();
                roadBase.accelerationPoints.Clear();
            }
            _pathGenerationController.SelectedRoad = null;
        }
        #endregion
    }
}
