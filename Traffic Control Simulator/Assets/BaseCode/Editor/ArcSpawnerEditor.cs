using UnityEditor;
using UnityEngine;

namespace BaseCode.Editor
{
    [CustomEditor(typeof(ArcSpawner))]
    public class ArcSpawnerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ArcSpawner spawner = (ArcSpawner)target;

            if (GUILayout.Button("Spawn Arc"))
            {
                spawner.SpawnArc();
            }

            if (GUILayout.Button("Clear Points"))
            {
                spawner.ClearChildren();
            }
        }
    }
}
