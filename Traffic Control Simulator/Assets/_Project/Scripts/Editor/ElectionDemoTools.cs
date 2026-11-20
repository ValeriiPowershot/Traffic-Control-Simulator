using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using TrafficControl.Elections;
namespace TrafficControl.EditorTools
{
    public static class ElectionDemoTools
    {
        public const string ScenePath = "Assets/_Project/Sandbox/Scenes/UI/ElectionDemo.unity";
        [MenuItem("Traffic Control/UI/Open Election Demo")]
        public static void Open()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            EditorSceneManager.OpenScene(ScenePath);
        }
        // Rebuilds are explicit; normal scene opening preserves layout edits.
        [MenuItem("Traffic Control/UI/Create Election Demo Copy")]
        public static void CreateCopy()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            string target=AssetDatabase.GenerateUniqueAssetPath(ScenePath);
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            var root=new GameObject("Election presentation"); var demo=root.AddComponent<ElectionDemo>();
            demo.RegularFont=AssetDatabase.LoadAssetAtPath<Font>("Assets/Le Tai's Asset/TrueShadow/Demo/Symbols/Fonts/Ubuntu-Regular.ttf");
            demo.BoldFont=AssetDatabase.LoadAssetAtPath<Font>("Assets/Le Tai's Asset/TrueShadow/Demo/Symbols/Fonts/Ubuntu-Bold.ttf");
            demo.BuildInterface();
            var camera=new GameObject("Election camera").AddComponent<Camera>(); camera.clearFlags=CameraClearFlags.SolidColor;
            camera.tag="MainCamera"; camera.gameObject.AddComponent<AudioListener>();
            camera.backgroundColor=new Color(0.95f,0.94f,0.91f); camera.orthographic=true;
            EditorSceneManager.SaveScene(scene,target); Selection.activeGameObject=root;
        }
    }
}
