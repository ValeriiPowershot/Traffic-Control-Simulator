using Gley.UrbanAssets.Editor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class ShowDisconnectedWaypoints : ShowWaypointsTrafficBase
    {
        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            waypointsOfInterest = trafficWaypointData.GetDisconnectedWaypoints();
            return this;
        }

        public override void DrawInScene()
        {
            trafficWaypointDrawer.ShowDisconnectedWaypoints(editorSave.editorColors.waypointColor);
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
