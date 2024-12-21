using System.Collections.Generic;
using System.Linq;
using BaseCode.Logic;
using BaseCode.Logic.PathData;
using BaseCode.Logic.Ways;
using Script.Roads;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BaseCode.Editor.Path
{
    public class SceneObjectSpawner : EditorWindow
    {
        private static GameObject _selectedObject;
        private Vector2 _scrollPosition;
        private static Objects _objects;
        private RoadBase _currentRoadBase;

        [MenuItem("Window/Object Spawner")]
        public static void ShowWindow()
        {
            _objects = FindObjectOfType<Objects>();
            _objects.selectedObject = _selectedObject;

            if(_objects.canOopenWindow == false)
                return;

            var window = GetWindow<SceneObjectSpawner>("Object Spawner");
            window.minSize = new Vector2(400, 800); // Minimum width and height
        }
        
        private void OnGUI()
        {
            SetDirection();
            
            if (_selectedObject == null)
            {
                EditorGUILayout.LabelField("Select an object in the scene.");
                return;
            }

            EditorGUILayout.LabelField($"Selected Object: {_selectedObject.name}");

            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            foreach (var roadBase in _objects.prefabs)
            {
                if (GUILayout.Button($"Spawn {roadBase.name}"))
                {
                    SpawnObject(roadBase);
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
            foreach (var road in _objects.selectedRoads)
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
            var roads = _objects.selectedRoads;
            var size = roads.Count;

            foreach (var roadBase in _objects.selectedRoads)
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
            
            _objects.selectedRoads.Clear();
            
            if (selectedObjects[0].GetComponent<RoadBase>() is TripleRoadIntersection 
                || selectedObjects[^1].GetComponent<RoadBase>() is TripleRoadIntersection )
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
                    _objects.selectedRoads.Clear();
                    return false; // Skip adding intersections to the path
                }

                if (road != null && !_objects.selectedRoads.Contains(road))
                {
                    _objects.selectedRoads.Add(road);
                    Debug.Log($"Added road: {road.name}");
                }

                oldRoadBase = road;
            }

            return true;
        }
        
        private void ChangeRotation()
        {
            if (_selectedObject == null)
            {
                EditorGUILayout.LabelField("No object selected.");
                return;
            }

            var currentRotation = _selectedObject.transform.rotation;

            float currentY = Mathf.Round(currentRotation.eulerAngles.y / 90f) * 90f;

            float newRotationY = EditorGUILayout.Slider("Rotation Y", currentY, 0f, 270f);

            if (!Mathf.Approximately(newRotationY, currentRotation.eulerAngles.y))
            {
                _selectedObject.transform.rotation = Quaternion.Euler(currentRotation.eulerAngles.x, newRotationY, currentRotation.eulerAngles.z);
                EditorUtility.SetDirty(_selectedObject);
            }
        }

        private void SetDirection()
        {
            List<Vector3> directions = _objects.directions;
            
            foreach (var direction in directions)
            {
                if (GUILayout.Button(GetVectorName(direction)))
                {
                    Debug.Log("Direction set to: " + direction);
                    _objects.currentDirection = direction;
                }
            }
        }
        
        private string GetVectorName(Vector3 direction)
        {
            if (direction == Vector3.forward)
                return "Vector3.forward";
            if (direction == Vector3.right)
                return "Vector3.right";
            if (direction == Vector3.back)
                return "Vector3.back";
            if (direction == Vector3.left)
                return "Vector3.left";
        
            return "Unknown direction"; // For other cases
        }
        
        private void SpawnObject(RoadBase objectName)
        {
            if (_selectedObject == null)
                return;
            
            Vector3 currentDirection = _objects.currentDirection;
             
            GameObject prefab = (GameObject)PrefabUtility.InstantiatePrefab(objectName.gameObject,_objects.transform);
            prefab.transform.position = _selectedObject.transform.position + currentDirection * _objects.offset;
            prefab.transform.rotation = Quaternion.LookRotation(currentDirection);
            _selectedObject = prefab;
            
            _objects.selectedObject = _selectedObject;
            _objects.currentDirection = currentDirection;
            
            _currentRoadBase = prefab.GetComponent<RoadBase>();
            _objects.createdRoadBases.Add(_currentRoadBase);
                
            Undo.RegisterCreatedObjectUndo(prefab, $"Spawn {objectName}");
            Selection.activeGameObject = prefab;
        }

        public void GenerateNewPath()
        {
            var createNewContainer = new GameObject
            {
                transform =
                {
                    parent = _objects.allWaysContainer.transform
                },
                name = "PathGenerator"
            };
            var waypointContainer = createNewContainer.AddComponent<WaypointContainer>();

            foreach (var objectsCreatedRoadBase in _objects.createdRoadBases.ToList())
            {
                if(objectsCreatedRoadBase == null)
                    _objects.createdRoadBases.Remove(objectsCreatedRoadBase);
            }
            
            foreach (var roadBase in _objects.selectedRoads)
            {
                roadBase.startPoint = null;
                roadBase.endPoint = null;
                
                waypointContainer.waypoints.AddRange(roadBase.path);
                roadBase.path.Clear();
            }          
        }
        private void CleanVisuals()
        {
            foreach (var roadBase in _objects.createdRoadBases)
            {
                if(roadBase == null)
                    continue;
                roadBase.startPoint = null;
                roadBase.endPoint = null;
                roadBase.path.Clear();
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
        }
    }
}
