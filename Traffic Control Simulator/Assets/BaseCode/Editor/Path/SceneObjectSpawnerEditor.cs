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

        [MenuItem("Window/Object Spawner")]
        public static void ShowWindow()
        {
            _pathGenerationController = FindObjectOfType<PathGenerationController>();

            if (_pathGenerationController == null)
            {
                var objects = new GameObject
                {
                    name = "----- Created Objects -----"
                };
                objects.transform.SetAsFirstSibling();
                
                _pathGenerationController = objects.AddComponent<PathGenerationController>();
                _pathGenerationController.roadsSo = Resources.Load<RoadsScriptableObject>("RoadsSo");
                
                AllWaysContainer waysContainer = FindObjectOfType<AllWaysContainer>();
                _pathGenerationController.allWaysContainer = waysContainer;
            }

            _pathGenerationController.clickedSelectedObject = _selectedObject;           
            RoadBase roadBase = _pathGenerationController.clickedSelectedObject.GetComponent<RoadBase>();
            if (roadBase == null)
            {
                _pathGenerationController.selectedRoad = _pathGenerationController.roadsSo.prefabs[0];
            }

            if(_pathGenerationController.roadsSo.canOpenWindow == false)
                return;

            var window = GetWindow<SceneObjectSpawnerEditor>("Object Spawner");
            window.minSize = new Vector2(400, 800); // Minimum width and height
        }
        
        private void OnGUI()
        {
            // SetDirection();
            if (_pathGenerationController == null || _pathGenerationController.clickedSelectedObject == null)
            {
                EditorGUILayout.LabelField("Select an object in the scene.");
                return;
            }

            EditorGUILayout.LabelField($"Selected Object: {_pathGenerationController.clickedSelectedObject.name}");

            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            foreach (var roadBase in _pathGenerationController.roadsSo.prefabs)
            {
                GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);

                // Highlight the button of the selected road
                if (roadBase == _pathGenerationController.selectedRoad)
                {
                    buttonStyle.normal.textColor = Color.green; // Highlighted text color
                }

                if (GUILayout.Button($"{roadBase.name} Select", buttonStyle))
                {
                    _pathGenerationController.selectedRoad = roadBase;
                }
            }
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField($"Change Rotation Of Selected Object");
            ChangeRotation();
            EditorGUILayout.Space();

            GenerateRoad();

            EditorGUILayout.EndScrollView();
        }
        private void GenerateRoad()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
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
            var selectedObjects = Selection.gameObjects;

            if (selectedObjects.Length <= 2)
            {
                Debug.LogError("Select more than 2 Roads");
                return false;
            }
            
            _pathGenerationController.selectedRoads.Clear();
            
            if (selectedObjects[0].GetComponent<RoadBase>() is (TripleRoadIntersection or ForthRoadIntersection)
                || selectedObjects[^1].GetComponent<RoadBase>() is (TripleRoadIntersection or ForthRoadIntersection))
            {
                Debug.LogError("First or End of Path Cant Be Intersection");
                return false;
            }

            CleanVisuals();

            RoadBase oldRoadBase = selectedObjects[0].GetComponent<RoadBase>();
            foreach (var obj in selectedObjects)
            {
                RoadBase road = obj.GetComponent<RoadBase>();

                if (road is TripleRoadIntersection && oldRoadBase is TripleRoadIntersection)
                {
                    Debug.LogError($"Two Intersection cannot be use in same neighbor index");
                    _pathGenerationController.selectedRoads.Clear();
                    return false; // Skip adding intersections to the path
                }

                if (road != null && !_pathGenerationController.selectedRoads.Contains(road))
                {
                    _pathGenerationController.selectedRoads.Add(road);
                    Debug.Log($"Added road: {road.name}");
                }

                oldRoadBase = road;
            }

            return true;
        }
        
        private void ChangeRotation()
        {
            if (_pathGenerationController.clickedSelectedObject == null)
            {
                EditorGUILayout.LabelField("No object selected.");
                return;
            }

            var currentRotation = _pathGenerationController.clickedSelectedObject.transform.rotation;

            float currentY = Mathf.Round(currentRotation.eulerAngles.y / 90f) * 90f;

            float newRotationY = EditorGUILayout.Slider("Rotation Y", currentY, 0f, 270f);

            if (!Mathf.Approximately(newRotationY, currentRotation.eulerAngles.y))
            {
                _pathGenerationController.clickedSelectedObject.transform.rotation = Quaternion.Euler(currentRotation.eulerAngles.x, newRotationY, currentRotation.eulerAngles.z);
                EditorUtility.SetDirty(_pathGenerationController.clickedSelectedObject);
            }
        }

        public void GenerateNewPath()
        {
            var createNewContainer = new GameObject
            {
                transform =
                {
                    parent = _pathGenerationController.allWaysContainer.transform
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
        }

        
        [InitializeOnLoadMethod]
        private static void EnableSceneClick()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            Event e = Event.current;

            if (e.type == EventType.MouseDown && e.button == 0 && e.clickCount == 2 && !e.alt) // Detect double-click
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    _selectedObject = hit.collider.gameObject;
                   
                    ShowWindow(); // Show the custom window on double-click
                    e.Use(); // Mark the event as used to prevent propagation
                }
            }

            if (e.modifiers != EventModifiers.None)
            {
                if(_pathGenerationController != null)
                    _pathGenerationController.selectingNewRoad = true;
            }
            else
            {
                if (_pathGenerationController != null)
                    _pathGenerationController.selectingNewRoad = false;
            }

            if (e.type == EventType.MouseDown && e.button == 0 && e.modifiers == EventModifiers.None)
            {
                if (_pathGenerationController == null || _pathGenerationController.currentDirection == Vector3.zero)
                {
                    if(_pathGenerationController != null)
                        _pathGenerationController.clickedSelectedObject = null;  
                    return;
                }
                
                if (_pathGenerationController.clickedSelectedObject != null && _pathGenerationController.selectedRoad != null && _pathGenerationController.selectingNewRoad == false)
                {
                    _pathGenerationController.SpawnNow();
                }
            }
            
        }
    }
}
