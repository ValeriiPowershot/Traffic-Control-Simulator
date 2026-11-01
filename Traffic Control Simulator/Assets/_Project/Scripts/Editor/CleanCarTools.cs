using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TrafficControl.EditorTools
{
    public static class CleanCarTools
    {
        [MenuItem("Traffic Control/Experimental/Open Clean Controller Lab")]
        private static void Open()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) { Debug.LogWarning("Stop Play Mode first."); return; }
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene("Assets/_Project/Sandbox/Scenes/ControllerTests/CleanCarControllerTest.unity");
        }
    }
}
