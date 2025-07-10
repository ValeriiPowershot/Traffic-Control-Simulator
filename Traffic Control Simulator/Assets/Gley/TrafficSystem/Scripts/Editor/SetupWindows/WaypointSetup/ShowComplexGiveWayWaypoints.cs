using Gley.UrbanAssets.Editor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class ShowComplexGiveWayWaypoints : ShowWaypointsTrafficBase
    {
        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            waypointsOfInterest = trafficWaypointData.GetComplexGiveWayWaypoints();
            return this;
        }

        public override void DrawInScene()
        {
            trafficWaypointDrawer.ShowComplexGiveWayWaypoints(editorSave.editorColors.waypointColor);
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
