using UnityEditor;
using UnityEngine;

namespace Gley.UrbanAssets.Editor
{
    public class GleyUtilities
    {
        static Camera sceneCamera;

        public static bool IsPointInViewWithValidation(Vector3 position)
        {
            if (!SetCamera())
            {
                return false;
            }
            return IsPointInView(position);
        }

        public static bool IsPointInView(Vector3 position)
        {
            Vector3 screenPosition = sceneCamera.WorldToViewportPoint(position);
            if (screenPosition.x > 1 || screenPosition.x < 0 || screenPosition.y > 1 || screenPosition.y < 0 || screenPosition.z < 0)
            {
                return false;
            }
            return true;
        }

        public static bool SetCamera()
        {
            if (sceneCamera == null)
            {
                if (SceneView.lastActiveSceneView == null)
                {
                    return false;
                }
                sceneCamera = SceneView.lastActiveSceneView.camera;
            }
            return true;
        }


        public static void TeleportSceneCamera(Vector3 cam_position, float height = 1)
        {
            var scene_view = SceneView.lastActiveSceneView;
            if (scene_view != null)
            {
                scene_view.Frame(new Bounds(cam_position, Vector3.one * height), false);
            }
        }
       
    }
}
