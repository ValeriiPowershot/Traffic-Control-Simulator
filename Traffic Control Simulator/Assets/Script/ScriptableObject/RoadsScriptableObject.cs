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
        public List<Vector3> directions = new List<Vector3>()
        {
            Vector3.forward,
            Vector3.right,
            Vector3.back,
            Vector3.left
        };

    }
}