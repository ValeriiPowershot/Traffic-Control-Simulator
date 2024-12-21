using System;
using System.Collections.Generic;
using BaseCode.Logic.Ways;
using Script.Roads;
using UnityEditor;
using UnityEngine;

namespace BaseCode.Logic.PathData
{
    public class Objects : MonoBehaviour
    {
        public List<RoadBase> prefabs = new List<RoadBase>();
        public List<RoadBase> createdRoadBases = new();
        public List<RoadBase> selectedRoads = new List<RoadBase>();

        public bool canOopenWindow = false;
        public float offset = 10f;

        [Header("Access from SceneObjectSpawner")]
        public GameObject selectedObject;
        
        public Vector3 currentDirection = Vector3.forward;
        public List<Vector3> directions = new List<Vector3>()
        {
            Vector3.forward,
            Vector3.right,
            Vector3.back,
            Vector3.left
        };

        public AllWaysContainer allWaysContainer;
 

        private void OnDrawGizmos()
        {
            if (selectedObject != null)
            {
                Vector3 startPosition = selectedObject.transform.position;
                Handles.color = Color.yellow;
                Handles.ArrowHandleCap(
                    0, // Control ID
                    startPosition, // Start position
                    Quaternion.LookRotation(currentDirection.normalized), // Arrow direction
                    5, // Arrow size
                    EventType.Repaint // Ensure it renders in the Scene view
                );
                
            }
            else
            {
                Vector3 startPosition = transform.position;

                foreach (var direction in directions)
                {

                    Handles.color = Color.cyan;
                    Handles.ArrowHandleCap(
                        0,
                        startPosition,
                        Quaternion.LookRotation(direction.normalized),
                        5,
                        EventType.Repaint
                    );
                }
            }
 
        }
    }
}