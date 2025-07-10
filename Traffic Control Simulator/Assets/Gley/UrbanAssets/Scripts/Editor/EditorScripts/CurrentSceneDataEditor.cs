using Gley.UrbanAssets.Internal;
using UnityEditor;

namespace Gley.UrbanAssets.Editor
{
    [CustomEditor(typeof(CurrentSceneData))]
    public class CurrentSceneDataEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}