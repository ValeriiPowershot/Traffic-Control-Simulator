using Gley.UrbanAssets.Editor;
using Gley.UrbanAssets.Internal;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class VehicleRoutesSetupWindow : TrafficSetupWindow
    {
        private readonly float scrollAdjustment = 104;
        private TrafficWaypointData trafficWaypointData;
        private TrafficWaypointDrawer waypointDrawer;
        private int nrOfVehicles;
       
      

        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            trafficWaypointData = CreateInstance<TrafficWaypointData>().Initialize();
            waypointDrawer = CreateInstance<TrafficWaypointDrawer>().Initialize(trafficWaypointData);
            waypointDrawer.onWaypointClicked += WaypointClicked;
            nrOfVehicles = System.Enum.GetValues(typeof(VehicleTypes)).Length;
            if (editorSave.agentRoutes.routesColor.Count < nrOfVehicles)
            {
                for (int i = editorSave.agentRoutes.routesColor.Count; i < nrOfVehicles; i++)
                {
                    editorSave.agentRoutes.routesColor.Add(Color.white);
                    editorSave.agentRoutes.active.Add(true);
                }
            }
            
            return this;
        }


        public override void DrawInScene()
        {
            for (int i = 0; i < nrOfVehicles; i++)
            {
                if (editorSave.agentRoutes.active[i])
                {
                    waypointDrawer.ShowWaypointsWithVehicle(i, editorSave.agentRoutes.routesColor[i]);
                }
            }

            base.DrawInScene();
        }


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));
            EditorGUILayout.LabelField("Vehicle Routes: ");
            for (int i = 0; i < nrOfVehicles; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(((VehicleTypes)i).ToString(), GUILayout.MaxWidth(150));
                editorSave.agentRoutes.routesColor[i] = EditorGUILayout.ColorField(editorSave.agentRoutes.routesColor[i]);
                Color oldColor = GUI.backgroundColor;
                if (editorSave.agentRoutes.active[i])
                {
                    GUI.backgroundColor = Color.green;
                }
                if (GUILayout.Button("View", GUILayout.MaxWidth(BUTTON_DIMENSION)))
                {
                    editorSave.agentRoutes.active[i] = !editorSave.agentRoutes.active[i];
                    SceneView.RepaintAll();
                }
                GUI.backgroundColor = oldColor;
                EditorGUILayout.EndHorizontal();
            }

            base.ScrollPart(width, height);
            EditorGUILayout.EndScrollView();
        }


        private void WaypointClicked(WaypointSettingsBase clickedWaypoint, bool leftClick)
        {
            window.SetActiveWindow(typeof(EditWaypointWindow), true);
        }


        public override void DestroyWindow()
        {
            waypointDrawer.onWaypointClicked -= WaypointClicked;
            DestroyImmediate(trafficWaypointData);
            DestroyImmediate(waypointDrawer);
            base.DestroyWindow();
        }
    }
}