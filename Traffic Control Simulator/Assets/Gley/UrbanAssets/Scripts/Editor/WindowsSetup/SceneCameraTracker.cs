using UnityEditor;
using UnityEngine;

namespace Gley.UrbanAssets.Editor
{
    public class SceneCameraTracker
    {
        private Vector3 oldPivot;
        private float oldCameraDistance;
        private double time;
        private bool moved;

        internal delegate void SceneCameraMoved();
        internal static event SceneCameraMoved onSceneCameraMoved;
        internal static void TriggerSceneCameraMovedEvent()
        {
            if (onSceneCameraMoved != null)
            {
                onSceneCameraMoved();
            }
        }

        internal void MoveCheck()
        {
            if (moved)
            {
                if (EditorApplication.timeSinceStartup - time > 0.2)
                {
                    TriggerSceneCameraMovedEvent();
                    moved = false;
                }
            }

            if (SceneView.lastActiveSceneView != null)
            {
                if (oldPivot != SceneView.lastActiveSceneView.pivot || oldCameraDistance != SceneView.lastActiveSceneView.cameraDistance)
                {
                    oldPivot = SceneView.lastActiveSceneView.pivot;
                    oldCameraDistance = SceneView.lastActiveSceneView.cameraDistance;
                    time = EditorApplication.timeSinceStartup;
                    moved = true;
                }
            }
        }
    }
}