using UnityEditor;
using UnityEngine;

namespace Gley.UrbanAssets.Editor
{
    public class Drawer : UnityEditor.Editor
    {
        private readonly bool showDebugMessages = false;

        protected GUIStyle style;
        protected bool cameraMoved;
        private Data data;
        private Quaternion arrowSide1 = Quaternion.Euler(0, -25, 0);
        private Quaternion arrowSide2 = Quaternion.Euler(0, 25, 0);
        private Vector3 direction;
        
        
        protected virtual void Initialize(Data data)
        {
            if (showDebugMessages)
            {
                Debug.Log("Initialize " + this);
            }
            this.data = data;
            data.onModified += Refresh;
            style = new GUIStyle();
            SceneCameraTracker.onSceneCameraMoved += CameraMoved;
            SceneCameraTracker.TriggerSceneCameraMovedEvent();
        }


        private void Refresh()
        {
            SceneCameraTracker.TriggerSceneCameraMovedEvent();
            SceneView.RepaintAll();
        }


        private void CameraMoved()
        {
            cameraMoved = true;
        }


        protected void DrawTriangle(Vector3 start, Vector3 end)
        {
            direction = (start - end).normalized;
            Handles.DrawPolyLine(end, end + arrowSide1 * direction, end + arrowSide2 * direction, end);
        }


        private void OnEnable()
        {
            if (showDebugMessages)
            {
                Debug.Log("OnEnable " + this);
            }
        }


        protected virtual void OnDestroy()
        {
            if (showDebugMessages)
            {
                Debug.Log("OnDestroy " + this);
            }
            data.onModified -= Refresh;
            SceneCameraTracker.onSceneCameraMoved -= CameraMoved;
        }
    }
}
