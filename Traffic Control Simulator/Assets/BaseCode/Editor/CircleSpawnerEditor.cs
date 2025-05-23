using UnityEditor;
using UnityEngine;

namespace BaseCode.Editor
{
    [CustomEditor(typeof(CircleSpawner))]
    public class CircleSpawnerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            CircleSpawner spawner = (CircleSpawner)target;

            if (GUILayout.Button("Spawn Circle")) 
                spawner.SpawnCircle();

            if (GUILayout.Button("Clear Children")) 
                spawner.ClearChildren();
        }
    }
}
