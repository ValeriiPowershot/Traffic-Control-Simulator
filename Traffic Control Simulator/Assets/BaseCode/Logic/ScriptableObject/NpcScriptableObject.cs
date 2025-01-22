using UnityEngine;

namespace BaseCode.Logic.ScriptableObject
{
    [CreateAssetMenu(fileName = "NpcScriptableObject", menuName = "So/NpcScriptableObject", order = 0)]
    public class NpcScriptableObject : UnityEngine.ScriptableObject
    {
        public float moveSpeed = 5f; // Movement speed
        public float runSpeed = 10f; // Movement speed
        public float stopDistance = 0.1f; // Distance threshold to consider the target reached

        public GameObject prefab;
    }
}