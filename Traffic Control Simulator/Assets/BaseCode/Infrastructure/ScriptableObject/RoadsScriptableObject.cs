using System;
using System.Collections.Generic;
using BaseCode.Logic.EntityHandler.Roads;
using UnityEngine;

namespace BaseCode.Infrastructure.ScriptableObject
{
    [CreateAssetMenu(fileName = "RoadsSo", menuName = "So/RoadsSo", order = 0)]
    public class RoadsScriptableObject : UnityEngine.ScriptableObject
    {
        public List<RoadBase> prefabs = new();
        public bool canOpenWindow;
        public float offset = 10f;
         
        public List<Tuple<Vector3, string>> directions =
            new List<Tuple<Vector3, string>> // Define offsets for each direction
            {
                new(Vector3.forward, "Forward"),
                new(Vector3.right, "Right"),
                new(Vector3.back, "Back"),
                new(Vector3.left, "Left"),
            };
    }
}