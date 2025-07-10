using UnityEditor;
#if UNITY_2019 || UNITY_2020
using UnityEditor.Experimental.SceneManagement;
#endif
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Gley.UrbanAssets.Editor
{
    public class GleyPrefabUtilities : UnityEditor.Editor
    {
        static string prefabStage;


        public static bool EditingInsidePrefab()
        {
            return (StageUtility.GetCurrentStageHandle() != StageUtility.GetMainStageHandle());
        }


        public static bool PrefabChanged()
        {
            if (PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
#if !UNITY_2019
                if (prefabStage != PrefabStageUtility.GetCurrentPrefabStage().assetPath)
                {
                    prefabStage = PrefabStageUtility.GetCurrentPrefabStage().assetPath;
                    return true;
                }
#else
                if (prefabStage != PrefabStageUtility.GetCurrentPrefabStage().prefabAssetPath)
                {
                    prefabStage = PrefabStageUtility.GetCurrentPrefabStage().prefabAssetPath;

                    return true;
                }
#endif
            }
            else
            {
                if (prefabStage != "")
                {
                    prefabStage = "";
                    return true;
                }
            }
            return false;
        }


        public static GameObject GetScenePrefabRoot()
        {
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                return prefabStage.prefabContentsRoot;
            }
            return null;
        }


        public static GameObject GetInstancePrefabRoot(GameObject go)
        {
            return PrefabUtility.GetOutermostPrefabInstanceRoot(go);
        }


        public static bool IsInsidePrefab(GameObject go)
        {
            GameObject prefab = PrefabUtility.GetOutermostPrefabInstanceRoot(go);
            if (prefab == null)
            {
                return false;
            }
            return true;
        }


        public static void DeleteGameObjectFromPrefab(GameObject prefabRoot, string gameObjectName)
        {
            ApplyPrefab(prefabRoot, GetPrefabPath(prefabRoot));
            string path = GetPrefabPath(prefabRoot);
            GameObject prefab = LoadPrefab(path);
            DestroyTransform(prefab.transform.FindDeepChild(gameObjectName));
            SavePrefab(prefab, path);
        }


        public static void ApplyPrefab(GameObject prefab, string path)
        {
            PrefabUtility.SaveAsPrefabAssetAndConnect(prefab, path, InteractionMode.AutomatedAction);
        }


        public static string GetPrefabPath(GameObject prefab)
        {
            GameObject parentObject = PrefabUtility.GetCorrespondingObjectFromSource(prefab);
            string path = AssetDatabase.GetAssetPath(parentObject);
            return path;
        }


        public static void DestroyTransform(Transform transformToDestroy)
        {
            if (transformToDestroy != null)
            {
                if (IsInsidePrefab(transformToDestroy.gameObject))
                {
                    if (EditingInsidePrefab())
                    {
                        DeleteGameObjectFromPrefab(GetScenePrefabRoot(), transformToDestroy.name);
                    }
                    else
                    {
                        DeleteGameObjectFromPrefab(GetInstancePrefabRoot(transformToDestroy.gameObject), transformToDestroy.name);
                    }
                }
                else
                {
                    DestroyImmediate(transformToDestroy.gameObject);
                }
            }
        }


        public static GameObject LoadPrefab(string path)
        {
            GameObject instantiatedObj = PrefabUtility.LoadPrefabContents(path); ;
            return instantiatedObj;
        }


        public static void SavePrefab(GameObject prefab, string path)
        {
            PrefabUtility.SaveAsPrefabAsset(prefab, path);
            PrefabUtility.UnloadPrefabContents(prefab);
        }


        public static void ClearAllChildObjects(Transform holder)
        {
            while (holder.childCount > 0)
            {
                if (IsInsidePrefab(holder.gameObject))
                {
                    if (EditingInsidePrefab())
                    {
                        DeleteGameObjectFromPrefab(GetScenePrefabRoot(), holder.GetChild(0).name);
                    }
                    else
                    {
                        DeleteGameObjectFromPrefab(GetInstancePrefabRoot(holder.gameObject), holder.GetChild(0).name);
                    }
                }
                else
                {
                    DestroyImmediate(holder.GetChild(0).gameObject);
                }
            }
        }


        public static void ApplyPrefabInstance(GameObject roadHolder)
        {
            if (IsInsidePrefab(roadHolder))
            {
                if (!EditingInsidePrefab())
                {
                    GameObject prefabRoot = GetInstancePrefabRoot(roadHolder);
                    ApplyPrefab(prefabRoot, GetPrefabPath(PrefabUtility.GetOutermostPrefabInstanceRoot(prefabRoot)));
                }
            }
        }


        public static void RevertToPrefab(Object componentToRevert)
        {
            PrefabUtility.RevertObjectOverride(componentToRevert, InteractionMode.AutomatedAction);
        }
    }
}
