using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

namespace SmartTools
{
    public class SmartTools_SceneData : MonoBehaviour
    {
        public StoredCombo[] store = new StoredCombo[12];

        public const int selectionHistoryCapacity = 20;
        public List<StoredSelection> selectionHistory = new();
        public int selectionHistoryIndex;

        public Vector3 gridOffset;


        static public SmartTools_SceneData Init()
        {
            var sceneDataGO = new GameObject("##SmartTools_SceneData##");
            var sceneData = sceneDataGO.AddComponent<SmartTools_SceneData>();
            sceneData.gameObject.hideFlags = HideFlags.HideInHierarchy;
            sceneData.store = new StoredCombo[12];
            for (int i = 0; i < sceneData.store.Length; i++)
            {
                sceneData.store[i] = new StoredCombo();
                sceneData.store[i].cam = new StoredSceneCam();
                sceneData.store[i].sel = new StoredSelection();
                sceneData.store[i].sel.objects = new Object[0];
            }
            return sceneData;
        }



        public void ClearHistory()
        {
            selectionHistory.Clear();
            selectionHistoryIndex = 0;
        }

        public void CleanSelectionHistory()
        {
            for (int i = 0; i < selectionHistory.Count; i++)
            {
                if (!selectionHistory[i].CheckSel())
                {
                    selectionHistory.RemoveAt(i--);
                }
            }
        }


        [System.Serializable] public class StoredCombo
        {
            public StoredSelection sel;
            public StoredSceneCam cam;

            public void SaveSel() { if (sel == null) sel = new StoredSelection(); sel.SaveSel(); }
            public void LoadSel() { sel.LoadSel(); }
            public void ClearSel() { sel.ClearSel(); }
            public bool CheckSel() { return sel.CheckSel(); }
            public void SaveCam(SceneView sceneView) { if (cam == null) cam = new StoredSceneCam(); cam.SaveCam(sceneView); }
            public void LoadCam(ref SceneView sceneView) { cam.LoadCam(ref sceneView); }
            public void ClearCam() { cam.ClearCam(); }
        }

        [System.Serializable] public class StoredSelection
        {
            public bool isSaved = false;
            public Object[] objects;

            public void SaveSel()
            {
                objects = Selection.objects;
                isSaved = objects.Length > 0;
            }

            public void LoadSel()
            {
                if (!isSaved) return;
                Selection.objects = objects;
            }

            public void ClearSel()
            {
                isSaved = false;
                objects = new Object[0];
            }

            public bool CheckSel()
            { 
                int count = objects.Length;
                for (int i = 0; i < count; i++) if (objects[i] != null) { isSaved = true; return true; }
                isSaved = false;
                return false;
            }
        }

        [System.Serializable] public class StoredSceneCam
        {
            public bool isSaved = false;
            public Vector3 pivot;
            public float size;
            public Quaternion rotation;
            public bool orthographic;
            public bool in2DMode;

            public void SaveCam(SceneView sceneView)
            {
                isSaved = true;
                pivot = sceneView.pivot;
                size = sceneView.size;
                rotation = sceneView.rotation;
                orthographic = sceneView.orthographic;
                in2DMode = sceneView.in2DMode;
            }

            public void LoadCam(ref SceneView sceneView)
            {
                if (!isSaved) return;
                sceneView.pivot = pivot;
                sceneView.size = size;
                sceneView.rotation = rotation;
                sceneView.orthographic = orthographic;
                sceneView.in2DMode = in2DMode;
            }

            public void ClearCam()
            {
                isSaved = false;
                pivot = Vector3.zero;
                size = 0;
                rotation = Quaternion.identity;
                orthographic = false;
                in2DMode = false;
            }
        }

        public void CheckAllSelections()
        {
            int count = store.Length;
            for (int i = 0; i < count; i++) { store[i].CheckSel(); }
        }
    }
}

#endif
