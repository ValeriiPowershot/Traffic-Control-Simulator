using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TrafficControl.EditorTools
{
    public static class ExperimentalTrafficTools
    {
        [MenuItem("Traffic Control/Experimental/Open Own Controller Test")]
        private static void Open()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) { Debug.LogWarning("Stop Play Mode before opening the test scene."); return; }
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene("Assets/_Project/Sandbox/Scenes/ControllerTests/ArcadeControllerTest.unity");
        }
    }
}
