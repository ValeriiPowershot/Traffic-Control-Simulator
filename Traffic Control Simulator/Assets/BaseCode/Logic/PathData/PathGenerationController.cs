using System;
using System.Collections.Generic;
using BaseCode.Logic.Roads;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Ways;
using UnityEditor;
using UnityEngine;

namespace BaseCode.Logic.PathData
{
    [ExecuteInEditMode]
    public class PathGenerationController : MonoBehaviour
    {
        public RoadsScriptableObject RoadsSo { get; private set; }
        public AllWaysContainer AllWaysContainer { get; private set; }

        [Header("Access from SceneObjectSpawner")]
        public List<RoadBase> createdRoadBases = new List<RoadBase>();
        public List<RoadBase> selectedRoads = new List<RoadBase>();
        
        public Vector3 CurrentDirection  { get; private set; }
        public bool SelectingNewRoad { get; set; }
        public GameObject ClickedSelectedObject{ get; set; } // Selected by double-click
        public RoadBase SelectedRoad{ get; set; } // Currently selected road
        
        public PathGenerationController(AllWaysContainer allWaysContainer)
        {
            AllWaysContainer = allWaysContainer;
        }

        public void OnEnable()
        {
            if (RoadsSo == null)
            {
                RoadsSo = Resources.Load<RoadsScriptableObject>("RoadsSo");
            }

            if (AllWaysContainer == null)
            {
                AllWaysContainer = FindObjectOfType<AllWaysContainer>();
            }
        }

        private void OnDrawGizmos()
        {
            if (IsThereProblem()) // very cool name
                return;

            Gizmos.color = RoadsSo.gizmoColor;

            Event currentEvent = Event.current;
            Vector2 mousePosition = currentEvent.mousePosition;

            Vector3 closestPosition = Vector3.zero;
            string closestLabel = "";
            float closestDistance = float.MaxValue;

            foreach (var offset in RoadsSo.directions)
            {
                Vector3 offsetPosition = CalculateOffsetPosition(offset.Item1);
                float distance = CalculateScreenDistance(offsetPosition, mousePosition);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPosition = offsetPosition;
                    CurrentDirection = offset.Item1;
                }

                DrawOffsetGizmo(offsetPosition, offset.Item2);
            }

            DrawClosestGizmo(closestDistance, closestPosition);
        }

        private bool IsThereProblem()
        {
            return SelectingNewRoad || ClickedSelectedObject == null || SelectedRoad == null;
        }

        private Vector3 CalculateOffsetPosition(Vector3 direction)
        {
            return ClickedSelectedObject.transform.position + direction.normalized * RoadsSo.offsetDistance;
        }

        private float CalculateScreenDistance(Vector3 worldPosition, Vector2 mousePosition)
        {
            Vector2 screenPosition = HandleUtility.WorldToGUIPoint(worldPosition);
            return Vector2.Distance(mousePosition, screenPosition);
        }

        private void DrawOffsetGizmo(Vector3 position, string label)
        {
            DrawMeshWithLabel(position, label);
            
            Gizmos.DrawMesh(
                SelectedRoad.roadMesh,
                position,
                transform.rotation,
                transform.localScale
            );
        }

        private void DrawClosestGizmo(float closestDistance, Vector3 closestPosition)
        {
            const float detectionThreshold = 60f;

            if (closestDistance < detectionThreshold)
            {
                DrawMeshWithLabel(closestPosition + Vector3.up * 10, "Spawn Me");
            }
            else
            {
                CurrentDirection = Vector3.zero;
            }
        }

        private void DrawMeshWithLabel(Vector3 position, string label)
        {
            Handles.Label(position, label);
        }

        public void SpawnNow()
        {
            if (CurrentDirection == Vector3.zero)
                return;

            SpawnObject(SelectedRoad);
            CurrentDirection = Vector3.zero;
        }

        private void SpawnObject(RoadBase roadBase)
        {
            GameObject prefab = (GameObject)PrefabUtility.InstantiatePrefab(roadBase.gameObject, transform);

            prefab.SetActive(true);
            prefab.transform.position = ClickedSelectedObject.transform.position + CurrentDirection * RoadsSo.offset;
            prefab.transform.rotation = Quaternion.LookRotation(CurrentDirection);

            RegisterSpawnedObject(prefab);
        }

        private void RegisterSpawnedObject(GameObject prefab)
        {
            var roadBase = prefab.GetComponent<RoadBase>();
            ClickedSelectedObject = prefab;
            roadBase.roadSo = RoadsSo;
            
            var scale = roadBase.transform.localScale;
            scale.x = RoadsSo.roadChange;
            roadBase.transform.localScale = scale;

            createdRoadBases ??= new List<RoadBase>();
            createdRoadBases.Add(roadBase);

            Undo.RegisterCreatedObjectUndo(prefab, $"Spawn {roadBase.name}");
            Selection.activeGameObject = prefab;
        }

    }
}