using UnityEditor;
using UnityEngine;

namespace Gley.UrbanAssets.Editor
{
    public class LayerOperations
    {
        public static T LoadOrCreateLayers<T>(string path) where T:ScriptableObject
        {
            T layerSetup = (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));
            if (layerSetup == null)
            {
                T asset = ScriptableObject.CreateInstance<T>();
                string folderPath = path.Remove(path.LastIndexOf('/'));
                FileCreator.CreateFolder(folderPath);
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                layerSetup = (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));
            }

            return layerSetup;
        }
    }
}
