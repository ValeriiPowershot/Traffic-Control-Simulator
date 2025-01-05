using System;
using System.Collections.Generic;
using Script.Roads;
using UnityEngine;

namespace Script.ScriptableObject
{
    [CreateAssetMenu(fileName = "RoadsSo", menuName = "So/RoadsSo", order = 0)]
    public class RoadsScriptableObject : UnityEngine.ScriptableObject
    {
        public List<RoadBase> prefabs = new List<RoadBase>();
        public bool canOpenWindow = false;
        public float offset = 10f;
         
        public List<Tuple<Vector3, string>> directions =
            new List<Tuple<Vector3, string>> // Define offsets for each direction
            {
                new Tuple<Vector3, string>(Vector3.forward, "Forward"),
                new Tuple<Vector3, string>(Vector3.right, "Right"),
                new Tuple<Vector3, string>(Vector3.back, "Back"),
                new Tuple<Vector3, string>(Vector3.left, "Left"),
            };
    }
}