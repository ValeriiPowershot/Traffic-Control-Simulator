using Gley.UrbanAssets.Editor;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class RoadSetupWindow : SetupWindowBase
    {
        private string createRoad;
        private string connectRoads;
        private string viewRoads;

        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            createRoad = "Create Road";
            connectRoads = "Connect Roads";
            viewRoads = "View Roads";
            return this;
        }


        protected override void TopPart()
        {
            base.TopPart();
            EditorGUILayout.LabelField("Select action:");
            EditorGUILayout.Space();

            if (GUILayout.Button(createRoad))
            {
                window.SetActiveWindow(typeof(CreateRoadWindow), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button(connectRoads))
            {
                window.SetActiveWindow(typeof(ConnectRoadsWindow), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button(viewRoads))
            {
                window.SetActiveWindow(typeof(ViewRoadsWindow), true);
            }
        }
    }
}