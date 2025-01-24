using System;
using System.Collections.Generic;
using BaseCode.Logic.Npcs.Npc;
using BaseCode.Logic.Roads;
using UnityEngine;

namespace BaseCode.Logic.ScriptableObject
{
    [CreateAssetMenu(fileName = "RoadsSo", menuName = "So/RoadsSo", order = 0)]
    public class RoadsScriptableObject : UnityEngine.ScriptableObject
    {
        public List<RoadBase> roadPrefabs = new();
        public List<NpcScriptableObject> npcPrefabs = new();
        
        
        public bool canOpenWindow;
        public float offset = 10f;
        
        public readonly List<Tuple<Vector3, string>> directions =
            new() 
            {
                new Tuple<Vector3, string>(Vector3.forward, "Forward"),
                new Tuple<Vector3, string>(Vector3.right, "Right"),
                new Tuple<Vector3, string>(Vector3.back, "Back"),
                new Tuple<Vector3, string>(Vector3.left, "Left"),
            };
        
        [Header("Transparent Road Creater Settings")]
        public Color gizmoColor = new(0, 1, 0, 0.3f); // Transparent green
        public float offsetDistance = 2f; // Offset distance for each direction
        
        [Header("On Road Component's Gizmo Settings")]
        public bool canDrawRoadGizmo;
        public Color arrowColor;
        public float arrowLength;

        [Header("Road Config")]
        public int roadChange; // if it is -1, it will change to the left, if it is 1, it will change to the right :D
    
        
        public NpcScriptableObject GetNpc<T>() where T : NpcBase
        {
            foreach (var npcPrefab in npcPrefabs)
            {
                NpcBase component = npcPrefab.prefab.GetComponent<T>();

                if (component.GetType() == typeof(T))
                {
                    return npcPrefab;
                }
            }

            return null;
        }
    }
}