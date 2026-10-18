using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class MaterialBatchReplacer : MonoBehaviour
{
    [System.Serializable]
    public class MaterialGroup
    {
        public string groupName;
        public Material targetMaterial;

        [Tooltip("Objects whose materials will be replaced")]
        public List<GameObject> objects = new List<GameObject>();
    }

    [SerializeField]
    public List<MaterialGroup> groups = new List<MaterialGroup>();

#if UNITY_EDITOR
    [ContextMenu("Apply Materials")]
    public void ApplyMaterials()
    {
        foreach (var group in groups)
        {
            if (group.targetMaterial == null) continue;

            foreach (var obj in group.objects)
            {
                if (obj == null) continue;

                Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);

                foreach (var renderer in renderers)
                {
                    Undo.RecordObject(renderer, "Batch Material Replace");

                    Material[] mats = renderer.sharedMaterials;

                    for (int i = 0; i < mats.Length; i++)
                    {
                        mats[i] = group.targetMaterial;
                    }

                    renderer.sharedMaterials = mats;

                    EditorUtility.SetDirty(renderer);
                }
            }
        }

        Debug.Log("Materials applied to all groups.");
    }
#endif
}
