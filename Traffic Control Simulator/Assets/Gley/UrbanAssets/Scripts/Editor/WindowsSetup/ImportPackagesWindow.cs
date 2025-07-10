using UnityEditor;
using UnityEngine;

namespace Gley.UrbanAssets.Editor
{
    public class ImportPackagesWindow : SetupWindowBase
    {
        private string message;

        protected override void TopPart()
        {
            EditorGUILayout.LabelField("Required Packages:");
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Burst");
            if(GUILayout.Button("Install"))
            {
                Gley.Common.ImportRequiredPackages.ImportPackage("com.unity.burst",UpdateMethod);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (GUILayout.Button("Install All"))
            {
                Gley.Common.ImportRequiredPackages.ImportPackage("com.unity.burst", UpdateMethod);
            }
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(message);

            base.TopPart();
        }


        private void UpdateMethod(string message)
        {
            this.message = message;
            if (message != "InProgress")
            {
                Debug.Log(message);
            }
        }
    }
}
