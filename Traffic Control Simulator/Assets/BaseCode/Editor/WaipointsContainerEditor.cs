using BaseCode.Logic.Ways;
using UnityEditor;
using UnityEngine;

namespace BaseCode.Editor
{
    [CustomEditor(typeof(WaypointContainer))]
    public class WaypointContainerEditor : UnityEditor.Editor
    {
        Transform parentTransform;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            WaypointContainer container = (WaypointContainer)target;

            EditorGUILayout.Space();
            parentTransform = (Transform)EditorGUILayout.ObjectField("Waypoints Parent", parentTransform, typeof(Transform), true);

            if (GUILayout.Button("Set Road Points From Parent"))
            {
                if (parentTransform != null)
                {
                    Undo.RecordObject(container, "Set Road Points");
                    container.SetRoadPointsFromParent(parentTransform);
                    EditorUtility.SetDirty(container);
                }
                else
                {
                    Debug.LogWarning("Parent transform is not assigned.");
                }
            }
        }
    }
}
