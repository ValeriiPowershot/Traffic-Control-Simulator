using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Pinwheel.MeshToFile
{
    public class MeshSaverGUI : EditorWindow
    {
        private MeshFilter target;
        private string meshName;
        private string path;
        private MeshSaver.FileType fileType;
        private Vector2 scrollPos;

        private readonly string[] EDITOR_PREF_KEYS_PATH = new string[2] { "meshsaver", "path" };

        [MenuItem("Window/Mesh To File")]
        public static void ShowWindow()
        {
            MeshSaverGUI window = GetWindow<MeshSaverGUI>();
            window.titleContent = new GUIContent($"{VersionInfo.ProductName} {VersionInfo.Code}");
            window.minSize = new Vector2(600, 200);
            window.Show();
        }

        private void OnEnable()
        {
            path = EditorPrefs.GetString(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_KEYS_PATH), "Assets/");
        }

        private void OnDisable()
        {
            EditorPrefs.SetString(EditorCommon.GetProjectRelatedEditorPrefsKey(EDITOR_PREF_KEYS_PATH), path);
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            target = EditorGUILayout.ObjectField("Target", target, typeof(MeshFilter), true) as MeshFilter;
            meshName = EditorGUILayout.TextField("Mesh name", meshName);
            fileType = (MeshSaver.FileType)EditorGUILayout.EnumPopup("File type", fileType);
            EditorCommon.BrowseFolder("Path", ref path);
            GUI.enabled =
                target != null &&
                target.sharedMesh != null &&
                !string.IsNullOrEmpty(meshName) &&
                !string.IsNullOrEmpty(path);
            if (EditorCommon.RightAnchoredButton("Save"))
            {
                Material mat = null;
                MeshRenderer mr = target.GetComponent<MeshRenderer>();
                if (mr != null)
                    mat = mr.sharedMaterial;
                MeshSaver.Save(target.sharedMesh, mat, path, meshName, fileType);
            }
            GUI.enabled = true;
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();


            EditorGUI.DrawRect(EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins), new Color32(32, 32, 32, 255));
            LinkStripDrawer.Draw("", "mesh2file", "linkstrip");
            EditorGUILayout.EndVertical();
        }
    }
}