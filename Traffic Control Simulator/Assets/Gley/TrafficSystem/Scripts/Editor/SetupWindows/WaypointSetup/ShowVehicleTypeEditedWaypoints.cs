using Gley.UrbanAssets.Editor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class ShowVehicleTypeEditedWaypoints : ShowWaypointsTrafficBase
    {
        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            waypointsOfInterest = trafficWaypointData.GetVehicleEditedWaypoints();
            return this;
        }


        public override void DrawInScene()
        {
            trafficWaypointDrawer.ShowVehicleEditedWaypoints(editorSave.editorColors.waypointColor, editorSave.editorColors.agentColor);
            base.DrawInScene();
        }


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));
            base.ScrollPart(width, height);
            GUILayout.EndScrollView();
        }
    }
}
