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
    public class Objects : MonoBehaviour
    {
        public RoadsScriptableObject roadsSo;
        public AllWaysContainer allWaysContainer;
        
        [Header("Access from SceneObjectSpawner")]
        public List<RoadBase> createdRoadBases = new();
        public List<RoadBase> selectedRoads = new List<RoadBase>();

        public GameObject selectedObject;
        public Vector3 currentDirection = Vector3.forward;

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

                foreach (var direction in roadsSo.directions)
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