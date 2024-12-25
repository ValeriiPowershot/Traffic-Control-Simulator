using System;
using System.Collections.Generic;
using BaseCode.Logic.Ways;
using Script.Roads;
using Script.ScriptableObject;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace BaseCode.Logic.PathData
{
    [ExecuteInEditMode]
    public class Objects : MonoBehaviour
    {
        public RoadsScriptableObject roadsSo;
        public AllWaysContainer allWaysContainer;
        
        [Header("Access from SceneObjectSpawner")]
        public List<RoadBase> createdRoadBases = new();
        public List<RoadBase> selectedRoads = new List<RoadBase>();

        public GameObject clickedSelectedObject; // select it by double click
        public RoadBase selectedRoad; // Selected road  
        
        // draw
        public Color gizmoColor = new Color(0, 1, 0, 0.3f); // Transparent green
        public float offsetDistance = 2f; // Distance to offset in each direction
        private bool isSelected = false; // Track if the object is selected
        
        public Vector3 currentDirection = Vector3.forward;
        public bool selectingNewRoad = false;
        
        private void OnDrawGizmos()
        {
            if (selectingNewRoad)
                return;
            
            if(clickedSelectedObject == null || selectedRoad == null) 
                return;
            
            Gizmos.color = gizmoColor;
            Vector3 closestPosition = Vector3.zero;
            string closestLabel = "";
            float closestDistance = float.MaxValue;

            Event currentEvent = Event.current;
            Vector2 mousePosition = currentEvent.mousePosition;

            foreach (var offset in roadsSo.directions)
            {
                Vector3 offsetPosition = clickedSelectedObject.transform.position + offset.Item1.normalized * offsetDistance;
                Vector3 screenPosition = HandleUtility.WorldToGUIPoint(offsetPosition);
                float distance = Vector2.Distance(mousePosition, new Vector2(screenPosition.x, screenPosition.y));
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPosition = offsetPosition;
                    currentDirection = offset.Item1;
                }
                DrawMeshWithLabel(offsetPosition, offset.Item2);
                Gizmos.DrawMesh(selectedRoad.roadMesh, 
                    offsetPosition, transform.rotation, transform.localScale);
            }
            
            if (closestDistance < 60f) // Set a threshold for detection
            {
                DrawMeshWithLabel(closestPosition + Vector3.up * 10, $"Spawn Me");
            }
            else
            {
                currentDirection = Vector3.zero;
            }
            
        }

        private void DrawMeshWithLabel(Vector3 position, string label)
        {
            Handles.Label(position, label);
        }
        public void SpawnNow()
        {
            if(currentDirection == Vector3.zero)
                return;
            
            SpawnObject(selectedRoad);
            currentDirection = Vector3.zero;
        }
        
        private void SpawnObject(RoadBase objectName)
        {
            GameObject prefab = Instantiate(objectName.gameObject, transform);
            prefab.SetActive(true);
            prefab.transform.position = clickedSelectedObject.transform.position + currentDirection * roadsSo.offset;
            prefab.transform.rotation = Quaternion.LookRotation(currentDirection);

            var roadBase = prefab.GetComponent<RoadBase>();
            clickedSelectedObject = prefab;

            createdRoadBases.Add(roadBase);

            Undo.RegisterCreatedObjectUndo(prefab, $"Spawn {objectName}");
            Selection.activeGameObject = prefab; // Optionally select the new object in the editor.
        }

      
    }
}