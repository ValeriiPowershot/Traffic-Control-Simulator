using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using System.Collections.Generic;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class ShowSpeedEditedWaypoints : ShowWaypointsTrafficBase
    {
        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            waypointsOfInterest = trafficWaypointData.GetSpeedEditedWaypoints();
            return this;
        }

        public override void DrawInScene()
        {
            trafficWaypointDrawer.ShowSpeedEditedWaypoints(editorSave.editorColors.waypointColor, editorSave.editorColors.speedColor);
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
